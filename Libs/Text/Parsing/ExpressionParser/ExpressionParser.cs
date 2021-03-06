﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Libs.Text.Formatting;

namespace Libs.Text.Parsing
{
    public class ExpressionParser : ParserBase
    {
        public delegate object NumberConvertHandler(string value);
        public delegate object ArgumentValueHandler(object value);

        internal class FunctionParsingHelper
        {
            internal InternalFunction Function { get; private set; }

            internal int Balance { get; set; } = 0;

            public static FunctionParsingHelper operator ++(FunctionParsingHelper obj)
            {
                obj.Balance++;
                return obj;
            }

            public static FunctionParsingHelper operator --(FunctionParsingHelper obj)
            {
                obj.Balance--;
                return obj;
            }

            internal FunctionParsingHelper(InternalFunction function)
            {
                Function = function;
            }
        }

        public const char CommentIdentifier = '#';

        public IDictionary<char, UnaryOperator> UnaryOperators { get; set; } = null;
        public IDictionary<string, BinaryOperator> BinaryOperators { get; set; } = null;
        public IDictionary<string, Variable> Variables { get; set; } = null;
        public IDictionary<string, Function> Functions { get; set; } = null;
        public IDictionary<string, Variable> CustomVariables { get; set; } = null;
        public BinaryOperator AssignmentOperator { get; set; } = null;
        public BinaryOperator ShorthandOperator { get; set; } = null;
        public EscapeSequenceFormatter EscapeSequenceFormatter { get; set; } = null;
        public NumberConvertHandler NumberConverter
        {
            get => m_NumberConverter;
            set => m_NumberConverter = value ?? DefaultNumberConverter;
        }
        public ArgumentValueHandler ArgumentHandler { get; set; } = null;

        private Dictionary<string, Variable> m_TemporaryVariables = null;
        private NumberConvertHandler m_NumberConverter = DefaultNumberConverter;
        private readonly HashSet<System.Type> m_ValueTypes = new HashSet<Type> { typeof(int), typeof(double), typeof(string) };

        private static bool IsNumber(char value) { return char.IsNumber(value) || value == '.'; }
        private static bool IsIdentifier(char value) { return char.IsLetter(value) || value == '_'; }
        private static bool IsIdentifier2(char value) { return IsIdentifier(value) || char.IsDigit(value) || value == '.'; }

        private string ExtractNumber()
        {
            return Next(IsNumber);
        }

        private string ExtractString()
        {
            char delimiter = Current;
            Next();

            char prevChar = '\0';
            string result = Next((chr) =>
            {
                bool status = Current != delimiter || (EscapeSequenceFormatter != null && prevChar == EscapeSequenceFormatter.Qualifier);
                prevChar = Current;
                return status;
            });

            if(Current != delimiter)
                throw new SyntaxException($@"Missing matching closing quote", Position - (result.Length + 1));

            Next();
            return EscapeSequenceFormatter != null ? EscapeSequenceFormatter.Unescape(result) : result;
        }

        private string ExtractIdentifier()
        {
            string result = Next(IsIdentifier2);
            if(result.Length == 0)
                throw new SyntaxException($@"Empty identifier", Position);

            return result;
        }

        private static object DefaultNumberConverter(string value)
        {
            try
            {
                return System.Convert.ToInt32(value);
            }
            catch(System.FormatException)
            {
                return System.Convert.ToDouble(value);
            }
        }

        private static bool IsCharToken(object token, params char[] cmp)
        {
            return token is char && cmp.Contains((char)token);
        }

        private bool IsValueToken(object token)
        {
            return token != null && m_ValueTypes.Contains(token.GetType());
        }

        private object PopValue(Stack<object> stack)
        {
            var result = stack.Pop();
            if(result is Variable tmp)
            {
                if(tmp.Value is null)
                    throw new SyntaxException($@"Use of unassigned variable '{tmp.Identifier}'");

                result = tmp.Value;
            }

            return ArgumentHandler != null ? ArgumentHandler(result) : result;
        }

        private Queue<object> ProcessParsing()
        {
            Queue<object> output = new Queue<object>();
            Stack<object> stack = new Stack<object>();
            Stack<FunctionParsingHelper> functions = new Stack<FunctionParsingHelper>();
            object lastToken = null;

            string unOps = UnaryOperators != null ? string.Concat(UnaryOperators.Keys) : string.Empty;
            string binOps = BinaryOperators != null ? string.Concat(BinaryOperators.Keys) : string.Empty;
            binOps += AssignmentOperator?.Identifier;

            // While there are tokens to be read:
            // Read a token.
            while(State)
            {
                Skip(char.IsWhiteSpace);

                if(!State || Current == CommentIdentifier)
                {
                    break;
                }
                else if(IsNumber(Current))
                {
                    if(ShorthandOperator != null && IsCharToken(lastToken, ')'))
                    {
                        stack.Push(ShorthandOperator);
                    }

                    // If the token is a number, then add it to the output queue.
                    lastToken = NumberConverter(ExtractNumber());
                    m_ValueTypes.Add(lastToken.GetType());
                    output.Enqueue(lastToken);
                }
                else if(Current == '\'' || Current == '\"')
                {
                    lastToken = ExtractString();
                    output.Enqueue(lastToken);
                }
                else if(binOps.Contains(Current) || unOps.Contains(Current))
                {
                    if(lastToken == null || lastToken is Operator || IsCharToken(lastToken, '(', ','))
                    {
                        if(!UnaryOperators.TryGetValue(Current, out var op))
                            throw new SyntaxException($@"Invalid unary operator '{Current}'", Position);

                        lastToken = op;
                        Next();
                    }
                    else
                    {
                        string identifier = Next(1);
                        identifier += Next((character) => binOps.Contains(character) && !unOps.Contains(character));

                        if(AssignmentOperator != null && identifier == AssignmentOperator.Identifier)
                        {
                            lastToken = AssignmentOperator;
                        }
                        else
                        {
                            if(!BinaryOperators.TryGetValue(identifier, out var op))
                                throw new SyntaxException($@"Invalid binary operator '{identifier}'", Position - identifier.Length);

                            lastToken = op;
                        }
                    }

                    // If the token is an operator, o_1, then:
                    // while there is an operator token, o_2, at the top of the stack, and either o_1 is left-associative and its precedence is less than or equal to that of o_2, or o_1 if right associative, and has precedence less than that of o_2, then pop o_2 off the stack, onto the output queue;
                    Operator lastOp = (Operator)lastToken;
                    Operator tmpOp;
                    while(stack.Count > 0 && (tmpOp = (stack.Peek() as Operator)) != null)
                    {
                        if(lastOp.Precedence > tmpOp.Precedence || ((lastOp.Associativity & (int)Operator.AssociativityType.Left) != 0 && lastOp.Precedence >= tmpOp.Precedence))
                        {
                            output.Enqueue(stack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    // push o_1 onto the stack.
                    stack.Push(lastOp);
                }
                else if(IsIdentifier(Current))
                {
                    if(ShorthandOperator != null && (IsValueToken(lastToken) || IsCharToken(lastToken, ')')))
                    {
                        stack.Push(ShorthandOperator);
                    }

                    string identifier = ExtractIdentifier();
                    Skip(char.IsWhiteSpace);

                    if(State && Current == '(')
                    {
                        // If the token is a function token, then push it onto the stack.
                        if(Functions == null || !Functions.TryGetValue(identifier, out var function))
                            throw new SyntaxException($@"Unknown function '{identifier}'", Position - identifier.Length);

                        lastToken = new InternalFunction(function);
                        functions.Push(new FunctionParsingHelper((InternalFunction)lastToken));
                        stack.Push(lastToken);
                    }
                    else
                    {
                        if(Variables == null || !Variables.TryGetValue(identifier, out var variable))
                        {
                            if(m_TemporaryVariables == null || !m_TemporaryVariables.TryGetValue(identifier, out variable))
                            {
                                if(CustomVariables == null)
                                    throw new SyntaxException($@"Unknown variable '{identifier}'", Position - identifier.Length);
                                else if(!CustomVariables.TryGetValue(identifier, out variable))
                                {
                                    variable = new Variable(identifier);
                                    CustomVariables[variable.Identifier] = variable;
                                }
                            }
                        }

                        lastToken = variable;
                        output.Enqueue(lastToken);
                    }
                }
                else
                {
                    switch(Current)
                    {
                        case '(':
                            if(ShorthandOperator != null && IsValueToken(lastToken))
                            {
                                stack.Push(ShorthandOperator);
                            }

                            if(functions.Count > 0)
                            {
                                var fh = functions.Peek();
                                fh++;
                            }

                            lastToken = Current;
                            // If the token is a left parenthesis, then push it onto the stack.
                            stack.Push(lastToken);
                            break;
                        case ')':
                        {
                            if(functions.Count > 0)
                            {
                                var fh = functions.Peek();
                                fh--;

                                if(fh.Balance == 0)
                                {
                                    if(!IsCharToken(lastToken, '('))
                                    {
                                        fh.Function.ArgumentCount++;
                                    }

                                    functions.Pop();
                                }
                            }

                            // If the token is a right parenthesis:
                            // Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue.
                            // Pop the left parenthesis from the stack, but not onto the output queue.
                            object top = null;
                            while(stack.Count > 0 && !IsCharToken((top = stack.Pop()), '('))
                            {
                                output.Enqueue(top);
                            }

                            // If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                            if(!IsCharToken(top, '('))
                                throw new SyntaxException($@"Missing matching closing bracket");

                            // If the token at the top of the stack is a function token, pop it onto the output queue.
                            if(stack.Count > 0 && stack.Peek() is InternalFunction)
                            {
                                output.Enqueue(stack.Pop());
                            }

                            lastToken = Current;
                            break;
                        }
                        case ',':
                        {
                            if(functions.Count > 0)
                            {
                                var fh = functions.Peek();
                                fh.Function.ArgumentCount++;
                            }

                            // If the token is a function argument separator (e.g., a comma):
                            // Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue. If no left parentheses are encountered, either the separator was misplaced or parentheses were mismatched.
                            object top = null;
                            while(stack.Count > 0 && !IsCharToken((top = stack.Peek()), '('))
                            {
                                output.Enqueue(stack.Pop());
                            }

                            if(!IsCharToken(top, '('))
                                throw new SyntaxException($@"Missing matching opening bracket");

                            lastToken = Current;
                            break;
                        }
                    }

                    Next();
                }
            }

            // When there are no more tokens to read:
            // While there are still operator tokens in the stack:
            while(stack.Count > 0)
            {
                var top = stack.Pop();
                // If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
                if(!(top is Operator)) throw new SyntaxException($@"Missing matching closing bracket");

                // Pop the operator onto the output queue.
                output.Enqueue(top);
            }

            // Exit.
            return output;
        }

        private object ProcessEvaluation(Queue<object> postfix)
        {
            Stack<object> stack = new Stack<object>();
            while(postfix.Count > 0)
            {
                object current = postfix.Dequeue();
                if(IsValueToken(current) || current is Variable)
                {
                    stack.Push(current);
                }
                else if(current is UnaryOperator unOp)
                {
                    if(stack.Count < 1)
                        throw new SyntaxException($@"Insufficient arguments unary operator '{unOp.Identifier}'");

                    object a = PopValue(stack);
                    stack.Push(unOp.Callback(a));
                }
                else if(current is BinaryOperator binOp)
                {
                    if(stack.Count < 2)
                        throw new SyntaxException($@"Insufficient arguments for binary operator '{binOp.Identifier}'");

                    object b = PopValue(stack);
                    object a;
                    if(current == AssignmentOperator)
                    {
                        a = stack.Pop();
                        if(!(a is Variable variable))
                            throw new SyntaxException($@"Assignment of non-variable type ('{a.GetType().Name}')");
                        else if(Variables != null && Variables.ContainsKey(variable.Identifier))
                            throw new SyntaxException($@"Assignment of reserved variable '{variable.Identifier}'");
                    }
                    else
                    {
                        a = PopValue(stack);
                    }

                    stack.Push(binOp.Callback(a, b));
                }
                else if(current is InternalFunction function)
                {
                    List<object> args = new List<object>();

                    if(!function.HasValidArgumentCount)
                        throw new SyntaxException($@"Invalid number of arguments provided for function '{function.Identifier}' ({function.ArgumentCount} ({function.MinArgumentCount} - {function.MaxArgumentCount}))");
                    else if(stack.Count < function.ArgumentCount)
                        throw new SyntaxException($@"Insufficient arguments for function '{function.Identifier}' ({stack.Count}/{function.ArgumentCount})");

                    for(int i = 0; i < function.ArgumentCount; i++)
                    {
                        args.Add(PopValue(stack));
                    }

                    stack.Push(function.Callback(((IEnumerable<object>)args).Reverse().ToArray()));
                }
            }

            if(stack.Count > 1)
                throw new SyntaxException($@"Too many value tokens provided");

            return PopValue(stack);
        }

        public object Evaluate(string expression, params (string Identifier, object Value)[] variables)
        {
            return ProcessEvaluation(Parse(expression, variables));
        }

        public object Evaluate(TextReader reader, params (string Identifier, object Value)[] variables)
        {
            return ProcessEvaluation(Parse(reader, variables));
        }

        public Queue<object> Parse(string expression, params (string Identifier, object Value)[] variables)
        {
            Queue<object> result;
            using(var reader = new StringReader(expression))
            {
                result = Parse(reader, variables);
            }

            return result;
        }

        public Queue<object> Parse(TextReader reader, params (string Identifier, object Value)[] variables)
        {
            Begin(reader);

            if(!State)
                return null;

            m_TemporaryVariables = variables.Any() ? variables.Select(e => (Variable)e).ToDictionary((e) => e.Identifier) : null;
            Queue<object> result = ProcessParsing();
            m_TemporaryVariables = null;

            if(State && Current != CommentIdentifier)
                throw new SyntaxException($@"Unexpected characters at end of expression");

            Close();
            return result;
        }

        public ExpressionParser(IDictionary<char, UnaryOperator> unaryOperators, IDictionary<string, BinaryOperator> binaryOperators, IDictionary<string, Variable> variables, IDictionary<string, Function> functions, IDictionary<string, Variable> customVariables = null, BinaryOperator assignmentOperator = null)
        {
            UnaryOperators = unaryOperators;
            BinaryOperators = binaryOperators;

            Variables = variables;
            Functions = functions;

            CustomVariables = customVariables;

            AssignmentOperator = assignmentOperator;
        }
    }
}
