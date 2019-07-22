using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Libs.Text.Formatting;

namespace Libs.Text.Parsing
{
    public class CommandParser : ParserBase, IEnumerable<KeyValuePair<string, CommandParser.CommandHandler>>, IEnumerable
    {
        public delegate object CommandHandler(params string[] args);

        private Dictionary<string, CommandHandler> m_Commands = new Dictionary<string, CommandHandler>();

        public EscapeSequenceFormatter EscapeSequenceFormatter { get; set; }

        private string ExtractIdentifier()
        {
            Skip(char.IsWhiteSpace);
            return State ? Next((chr) => char.IsLetterOrDigit(chr) || chr == '_' || chr == '.') : null;
        }

        private string ExtractArgument()
        {
            char prevChar = '\0';
            string result = Next((chr) =>
            {
                bool status = !char.IsWhiteSpace(Current) || (EscapeSequenceFormatter != null && prevChar == EscapeSequenceFormatter.Qualifier);
                prevChar = Current;
                return status;
            });

            return EscapeSequenceFormatter != null ? EscapeSequenceFormatter.Unescape(result) : result;
        }

        private string ExtractQuotedArgument()
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

        private string ParseArgument()
        {
            Skip(char.IsWhiteSpace);

            if(!State)
                return null;

            return (Current == '\'' || Current == '\"') ? ExtractQuotedArgument() : ExtractArgument();
        }

        private IEnumerable<string> ParseArguments()
        {
            string argument;
            while((argument = ParseArgument()) != null)
            {
                yield return argument;
            }
        }

        public object ProcessParsing()
        {
            string identifier = ExtractIdentifier();
            if(string.IsNullOrEmpty(identifier))
                throw new SyntaxException($@"No command specified");

            if(!m_Commands.TryGetValue(identifier, out var handler))
                throw new SyntaxException($@"Command not found: '{identifier}'");

            return handler(ParseArguments().ToArray());
        }

        public object Execute(string text)
        {
            object result;
            using(var reader = new StringReader(text))
            {
                result = Execute(reader);
            }

            return result;
        }

        public object Execute(TextReader reader)
        {
            Begin(reader);

            if(!State)
                return String.Empty;

            object result = ProcessParsing();

            Close();
            return result;
        }

        public void Add(string identifier, CommandHandler handler) => m_Commands.Add(identifier, handler);

        public IEnumerator<KeyValuePair<string, CommandHandler>> GetEnumerator() => m_Commands.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Commands).GetEnumerator();

        public CommandParser(EscapeSequenceFormatter escapeSequenceFormatter = null)
        {
            EscapeSequenceFormatter = escapeSequenceFormatter;
        }
    }
}
