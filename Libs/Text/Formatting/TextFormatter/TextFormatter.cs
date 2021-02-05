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
            Identifier,
            OpeningBracket,
            ClosingBracket,
            ArgumentSeparator,
            ExpressionStart,
            ExpressionEnd
        }

        private char m_Qualifier;
        private (TokenType Type, object Object) m_CurrentToken = default;

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
        public IDictionary<string, FunctionNode.EvaluationHandler> Functions { get; set; }
        public EscapeSequenceFormatter EscapeSequenceFormatter { get; set; }

        private static bool IsNumber(char value) => char.IsNumber(value) || value == '.' || value == '-';
        private static bool IsIdentifier(char value) => char.IsLetter(value) || value == '_';
        private static bool IsIdentifier2(char value) => IsIdentifier(value) || char.IsDigit(value) || value == '.';

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
            m_CurrentToken = default;

            Skip(char.IsWhiteSpace);

            if(!State)
                return;

            switch(Current)
            {
                case '+':
                    m_CurrentToken.Type = TokenType.ConcatenationOperator;
                    break;
                case '\'':
                case '\"':
                    m_CurrentToken.Object = ExtractString();
                    m_CurrentToken.Type = TokenType.Value;
                    return;
                case '(':
                    m_CurrentToken.Type = TokenType.OpeningBracket;
                    break;
                case ')':
                    m_CurrentToken.Type = TokenType.ClosingBracket;
                    break;
                case ',':
                    m_CurrentToken.Type = TokenType.ArgumentSeparator;
                    break;
                case '{':
                    m_CurrentToken.Type = TokenType.ExpressionStart;
                    break;
                case '}':
                    m_CurrentToken.Type = TokenType.ExpressionEnd;
                    break;
                default:
                    if(IsNumber(Current))
                    {
                        m_CurrentToken.Object = ParseNumber(ExtractNumber());
                        m_CurrentToken.Type = TokenType.Value;
                    }
                    else if(IsIdentifier(Current))
                    {
                        m_CurrentToken.Object = ExtractIdentifier();
                        m_CurrentToken.Type = TokenType.Identifier;
                    }

                    return;
            }

            Next();
        }

        private IEvaluable ParseLeaf()
        {
            if(m_CurrentToken.Type == TokenType.Value)
            {
                ValueNode node = new ValueNode(m_CurrentToken.Object);
                NextToken();
                return node;
            }
            else if(m_CurrentToken.Type == TokenType.OpeningBracket)
            {
                NextToken();
                var node = ParseBranch();
                if(m_CurrentToken.Type != TokenType.ClosingBracket)
                    throw new SyntaxException($@"Missing matching closing bracket");

                NextToken();
                return node;
            }
            else if(m_CurrentToken.Type == TokenType.Identifier)
            {
                string identifier = (string)m_CurrentToken.Object;

                NextToken();
                if(m_CurrentToken.Type == TokenType.OpeningBracket)
                {
                    if(Functions == null || !Functions.TryGetValue(identifier, out FunctionNode.EvaluationHandler callback))
                        throw new SyntaxException($@"Unknown function '{identifier}'", Position - identifier.Length);

                    FunctionNode function = new FunctionNode((FunctionNode.EvaluationHandler)callback);

                    NextToken();
                    while(m_CurrentToken.Type != TokenType.ClosingBracket && m_CurrentToken.Type != TokenType.None)
                    {
                        function.Add(ParseBranch());

                        if(m_CurrentToken.Type == TokenType.ArgumentSeparator)
                        {
                            NextToken();
                        }
                    }

                    if(m_CurrentToken.Type != TokenType.ClosingBracket)
                        throw new SyntaxException($@"Missing matching closing bracket");

                    NextToken();
                    return function;
                }
                else
                {
                    if(Variables == null || !Variables.TryGetValue(identifier, out object value))
                        throw new SyntaxException($@"Unknown variable '{identifier}'", Position - identifier.Length);

                    return new ValueNode(value);
                }
            }
            else
            {
                throw new SyntaxException($@"Unexpected token: '{ m_CurrentToken.Type }'");
            }
        }

        private IEvaluable ParseBranch()
        {
            var lhs = ParseLeaf();
            while(m_CurrentToken.Type == TokenType.ConcatenationOperator)
            {
                NextToken();
                var rhs = ParseLeaf();
                lhs = new ConcatenationOperatorNode(lhs, rhs);
            }

            return lhs;
        }

        private IEvaluable ParseExpression()
        {
            NextToken();
            if(m_CurrentToken.Type != TokenType.ExpressionStart)
                throw new SyntaxException("Missing expression opening parenthesis");

            NextToken();
            var node = ParseBranch();

            if(m_CurrentToken.Type != TokenType.ExpressionEnd)
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

        public TextFormatter(char qualifier, IDictionary<string, object> variables, IDictionary<string, FunctionNode.EvaluationHandler> functions, EscapeSequenceFormatter escapeSequenceFormatter = null)
        {
            Qualifier = qualifier;

            Variables = variables;
            Functions = functions;

            EscapeSequenceFormatter = escapeSequenceFormatter;
        }
    }
}
