using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Libs.Text.Formatting;

namespace Libs.Text.Parsing
{
    public class ExpressionParser : ParserBase
    {
        public delegate object NumberConvertHandler(string value);

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

        public IDictionary<char, UnaryOperator> UnaryOperators { get; set; }
        public IDictionary<string, BinaryOperator> BinaryOperators { get; set; }
        public IDictionary<string, Variable> Variables { get; set; }
        public IDictionary<string, Function> Functions { get; set; }
        public BinaryOperator AbbreviationOperator { get; set; }
        public UnaryOperator AssignmentOperator { get; set; }
        public EscapeSequenceFormatter EscapeSequenceFormatter { get; set; }
        public NumberConvertHandler NumberConverter
        {
            get => m_NumberConverter;
            set => m_NumberConverter = value ?? DefaultNumberConverter;
        }

        private readonly Dictionary<string, Variable> m_AssignedVariables = new Dictionary<string, Variable>();
        private Dictionary<string, Variable> m_TemporaryVariables;
        private NumberConvertHandler m_NumberConverter = DefaultNumberConverter;
        private HashSet<System.Type> m_ValueTypes = new HashSet<Type> { typeof(int), typeof(double), typeof(string) };

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
                throw new SyntaxException($@"Missing matching closing quote") { Position = Position - (result.Length + 1) };

            Next();
            return result;
        }

        private string ExtractIdentifier()
        {
            string result = Next(IsIdentifier2);
            if(result.Length == 0)
                throw new NameException($@"Empty identifier") { Position = Position };

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

        public void ClearAssignedVariables() => m_AssignedVariables.Clear();

        private object PopArgument(Stack<object> stack)
        {
            var result = stack.Pop();
            return result is string && EscapeSequenceFormatter != null ? EscapeSequenceFormatter.Unescape((string)result) : result;
        }

        private Queue<object> ProcessParsing()
        {
            Queue<object> output = new Queue<object>();
            Stack<object> stack = new Stack<object>();
            Stack<FunctionParsingHelper> functions = new Stack<FunctionParsingHelper>();
            object lastToken = null;

            string unOps = UnaryOperators != null ? string.Concat(UnaryOperators.Keys) : string.Empty;
            string binOps = BinaryOperators != null ? string.Concat(BinaryOperators.Keys) : string.Empty;
            unOps += AssignmentOperator?.Identifier;

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
                    if(AssignmentOperator != null && Current == AssignmentOperator.Identifier)
                    {
                        if(!(lastToken is Variable))
                            throw new SyntaxException($@"Assignment of non-variable") { Position = Position };

                        lastToken = AssignmentOperator;
                        Next();
                    }
                    else if(lastToken == null || lastToken is Operator || IsCharToken(lastToken, '(', ','))
                    {
                        if(UnaryOperators.TryGetValue(Current, out var op))
                        {
                            lastToken = op;
                        }
                        else
                            throw new NameException($@"Invalid unary operator") { Name = Current.ToString(), Position = Position };

                        Next();
                    }
                    else
                    {
                        string identifier = Next(1);
                        identifier += Next((character) => binOps.Contains(character) && !unOps.Contains(character));
                        if(BinaryOperators.TryGetValue(identifier, out var op))
                        {
                            lastToken = op;
                        }
                        else
                            throw new NameException($@"Invalid binary operator") { Name = identifier, Position = Position - identifier.Length };
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
                    string identifier = ExtractIdentifier();

                    if(AbbreviationOperator != null && IsValueToken(lastToken))
                    {
                        stack.Push(AbbreviationOperator);
                    }

                    Skip(char.IsWhiteSpace);

                    if(State && Current == '(')
                    {
                        // If the token is a function token, then push it onto the stack.
                        if(Functions != null && Functions.TryGetValue(identifier, out var function))
                        {
                            lastToken = new InternalFunction(function);
                            functions.Push(new FunctionParsingHelper((InternalFunction)lastToken));
                            stack.Push(lastToken);
                        }
                        else
                            throw new NameException($@"Unknown function") { Name = identifier, Position = Position - identifier.Length };
                    }
                    else
                    {
                        if(AssignmentOperator != null && (State && Current == AssignmentOperator.Identifier))
                        {
                            if(Variables != null && Variables.ContainsKey(identifier))
                                throw new NameException($@"Assignment of reserved variable") { Name = identifier, Position = Position - identifier.Length };

                            var tmp = new Variable(identifier);
                            m_AssignedVariables[tmp.Identifier] = tmp;

                            lastToken = tmp;
                            output.Enqueue(lastToken);
                        }
                        else
                        {
                            if(Variables == null || !Variables.TryGetValue(identifier, out var variable))
                            {
                                if(m_TemporaryVariables == null || !m_TemporaryVariables.TryGetValue(identifier, out variable))
                                {
                                    if(!m_AssignedVariables.TryGetValue(identifier, out variable))
                                        throw new NameException($@"Unknown variable") { Name = identifier, Position = Position - identifier.Length };
                                }
                            }

                            lastToken = variable;
                            output.Enqueue(lastToken);
                        }

                    }
                }
                else
                {
                    switch(Current)
                    {
                        case '(':
                            if(IsValueToken(lastToken))
                            {
                                if(AbbreviationOperator != null && IsValueToken(lastToken))
                                {
                                    stack.Push(AbbreviationOperator);
                                }
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
                if(!(top is Operator)) throw new SyntaxException("No matching right parenthesis");

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
                if(IsValueToken(current))
                {
                    stack.Push(current);
                }
                else if(current is Variable)
                {
                    object value = ((Variable)current).Value;
                    stack.Push(value ?? current);
                }
                else if(current is UnaryOperator op)
                {
                    if(stack.Count < 1)
                        throw new NameException("Not enough unary operator arguments") { Name = op.Identifier.ToString() };

                    object a = PopArgument(stack);
                    if(AssignmentOperator != null && current == AssignmentOperator)
                    {
                        if(stack.Count < 1)
                            throw new NameException("No assignable variable provided");

                        object b = PopArgument(stack);
                        if(!(b is Variable variable))
                            throw new NameException("Token is not of variable type") { Name = b.GetType().Name };

                        variable.Value = op.Callback(a);
                        stack.Push(variable.Value);
                    }
                    else
                    {
                        stack.Push(op.Callback(a));
                    }
                }
                else if(current is BinaryOperator)
                {
                    if(stack.Count < 2)
                        throw new NameException("Not enough binary operator arguments") { Name = ((BinaryOperator)current).Identifier };

                    object b = PopArgument(stack);
                    object a = PopArgument(stack);
                    object tmp = ((BinaryOperator)current).Callback(a, b);
                    stack.Push(tmp);
                }
                else if(current is InternalFunction function)
                {
                    List<object> args = new List<object>();

                    if(!function.HasValidArgumentCount)
                        throw new NameException($@"Invalid number of function arguments provided ({function.ArgumentCount} ({function.MinArgumentCount} - {function.MaxArgumentCount}))") { Name = function.Identifier };
                    else if(stack.Count < function.ArgumentCount)
                        throw new NameException($@"Not enough function arguments available ({stack.Count}/{function.ArgumentCount})") { Name = function.Identifier };

                    for(int i = 0; i < function.ArgumentCount; i++)
                    {
                        args.Add(PopArgument(stack));
                    }

                    args.Reverse();
                    stack.Push(function.Callback(args.ToArray()));
                }
            }

            if(stack.Count > 1)
                throw new SyntaxException("Too many value tokens provided");

            return PopArgument(stack);
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

            m_TemporaryVariables = variables.Select(e => (Variable)e).ToDictionary((e) => e.Identifier);

            Queue<object> result = ProcessParsing();

            if(State && Current != CommentIdentifier)
                throw new SyntaxException("Unexpected characters at end of expression");

            Close();
            m_TemporaryVariables = null;
            return result;
        }

        public ExpressionParser(IDictionary<char, UnaryOperator> unaryOperators, IDictionary<string, BinaryOperator> binaryOperators, IDictionary<string, Variable> variables, IDictionary<string, Function> functions, BinaryOperator abbreviationOperator = null, UnaryOperator assignmentOperator = null, EscapeSequenceFormatter escapeSequenceFormatter = null)
        {
            UnaryOperators = unaryOperators;
            BinaryOperators = binaryOperators;

            Variables = variables;
            Functions = functions;

            AbbreviationOperator = abbreviationOperator;
            AssignmentOperator = assignmentOperator;
            EscapeSequenceFormatter = escapeSequenceFormatter;
        }
    }
}
