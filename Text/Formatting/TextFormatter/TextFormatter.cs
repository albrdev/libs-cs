using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Libs.Text.Parsing;

namespace Libs.Text.Formatting
{
    public class TextFormatter : ParserBase
    {
        private enum TokenType
        {
            None,
            Value,
            ConcatenationOperator,
            Symbol,
            OpeningBracket,
            ClosingBracket,
            ArgumentSeparator,
            ExpressionStart,
            ExpressionEnd
        }

        private char m_Qualifier;

        private (TokenType Type, object Object) CurrentToken = default;

        public char Qualifier
        {
            get { return m_Qualifier; }
            set
            {
                if(!char.IsSymbol(value) && !char.IsPunctuation(value))
                    throw new System.ArgumentException($@"Qualifier not supported: '{value}'");

                m_Qualifier = value;
            }
        }

        public IDictionary<string, object> Variables { get; set; }
        public IDictionary<string, FunctionNode.Evaluator> Functions { get; set; }
        public EscapeSequenceFormatter EscapeSequenceFormatter { get; set; }

        private static bool IsNumber(char value) { return char.IsNumber(value) || value == '.' || value == '-'; }
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
            return EscapeSequenceFormatter != null ? EscapeSequenceFormatter.Unescape(result) : result;
        }

        private string ExtractIdentifier()
        {
            string result = Next(IsIdentifier2);
            if(result.Length == 0)
                throw new NameException($@"Empty identifier") { Position = Position };

            return result;
        }

        private static object ParseNumber(string value)
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

        private void NextToken()
        {
            CurrentToken = default;

            Skip(char.IsWhiteSpace);

            if(!State)
                return;

            switch(Current)
            {
                case '+':
                    CurrentToken.Type = TokenType.ConcatenationOperator;
                    break;
                case '\'':
                case '\"':
                    CurrentToken.Object = ExtractString();
                    CurrentToken.Type = TokenType.Value;
                    return;
                case '(':
                    CurrentToken.Type = TokenType.OpeningBracket;
                    break;
                case ')':
                    CurrentToken.Type = TokenType.ClosingBracket;
                    break;
                case ',':
                    CurrentToken.Type = TokenType.ArgumentSeparator;
                    break;
                case '{':
                    CurrentToken.Type = TokenType.ExpressionStart;
                    break;
                case '}':
                    CurrentToken.Type = TokenType.ExpressionEnd;
                    break;
                default:
                    if(IsNumber(Current))
                    {
                        CurrentToken.Object = ParseNumber(ExtractNumber());
                        CurrentToken.Type = TokenType.Value;
                    }
                    else if(IsIdentifier(Current))
                    {
                        CurrentToken.Object = ExtractIdentifier();
                        CurrentToken.Type = TokenType.Symbol;
                    }

                    return;
            }

            Next();
        }

        private Evaluable ParseLeaf()
        {
            if(CurrentToken.Type == TokenType.Value)
            {
                ValueNode node = new ValueNode(CurrentToken.Object);
                NextToken();
                return node;
            }
            else if(CurrentToken.Type == TokenType.OpeningBracket)
            {
                NextToken();
                var node = ParseBranch();
                if(CurrentToken.Type != TokenType.ClosingBracket)
                    throw new SyntaxException($@"Missing matching closing bracket");

                NextToken();
                return node;
            }
            else if(CurrentToken.Type == TokenType.Symbol)
            {
                string identifier = (string)CurrentToken.Object;

                NextToken();
                if(CurrentToken.Type == TokenType.OpeningBracket)
                {
                    if(Functions == null || !Functions.TryGetValue(identifier, out FunctionNode.Evaluator callback))
                        throw new NameException($@"Unknown function") { Name = identifier, Position = Position - identifier.Length };

                    FunctionNode function = new FunctionNode((FunctionNode.Evaluator)callback);

                    NextToken();
                    while(CurrentToken.Type != TokenType.ClosingBracket && CurrentToken.Type != TokenType.None)
                    {
                        function.Add(ParseBranch());

                        if(CurrentToken.Type == TokenType.ArgumentSeparator)
                        {
                            NextToken();
                        }
                    }

                    if(CurrentToken.Type != TokenType.ClosingBracket)
                        throw new SyntaxException($@"Missing matching closing bracket");

                    NextToken();
                    return function;
                }
                else
                {
                    if(Variables == null || !Variables.TryGetValue(identifier, out object value))
                        throw new NameException($@"Unknown variable") { Name = identifier, Position = Position - identifier.Length };

                    return new ValueNode(value);
                }
            }
            else
            {
                throw new SyntaxException($@"Unexpected token: '{ CurrentToken.Type }'");
            }
        }

        private Evaluable ParseBranch()
        {
            var lhs = ParseLeaf();
            while(CurrentToken.Type == TokenType.ConcatenationOperator)
            {
                NextToken();
                var rhs = ParseLeaf();
                lhs = new ConcatenationOperatorNode(lhs, rhs);
            }

            return lhs;
        }

        private Evaluable ParseExpression()
        {
            NextToken();
            if(CurrentToken.Type != TokenType.ExpressionStart)
                throw new SyntaxException("Missing expression opening parenthesis");

            NextToken();
            var node = ParseBranch();

            if(CurrentToken.Type != TokenType.ExpressionEnd)
                throw new SyntaxException("Missing expression closing parenthesis");

            return node;
        }

        private string ProcessParsing()
        {
            StringBuilder result = new StringBuilder();
            while(State)
            {
                if(Current == Qualifier)
                {
                    int qualifierCount = Skip((character) => character == Qualifier);
                    result.Append(new string(Qualifier, qualifierCount / 2));
                    if((qualifierCount & 0x1) != 0)
                    {
                        result.Append(ParseExpression().Evaluate());
                    }
                }
                else
                {
                    result.Append(Current);
                    Next();
                }
            }

            return result.ToString();
        }

        public string Format(string text)
        {
            string result;
            using(var reader = new StringReader(text))
            {
                result = Format(reader);
            }

            return result;
        }

        public string Format(TextReader reader)
        {
            Begin(reader);

            if(!State)
                return String.Empty;

            string result = ProcessParsing();

            Close();
            return result;
        }

        public TextFormatter(char qualifier, IDictionary<string, object> variables, IDictionary<string, FunctionNode.Evaluator> functions, EscapeSequenceFormatter escapeSequenceFormatter = null)
        {
            Qualifier = qualifier;

            Variables = variables;
            Functions = functions;

            EscapeSequenceFormatter = escapeSequenceFormatter;
        }
    }
}
