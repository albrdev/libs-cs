using System;
using System.IO;
using System.Text;
using Libs.Text.Parsing;

namespace Libs.Text.Formatting
{
    public abstract class EscapeSequenceFormatter : ParserBase
    {
        private char m_Qualifier;

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

        public string ProcessFormatting()
        {
            StringBuilder result = new StringBuilder();
            while(State)
            {
                string sequence = Format();
                if(sequence != null)
                {
                    result.Append(Qualifier + sequence);
                }
                else
                {
                    result.Append(Current);
                }

                Next();
            }

            return result.ToString();
        }

        public string ProcessParsing()
        {
            StringBuilder result = new StringBuilder();
            while(State)
            {
                if(Current == Qualifier)
                {
                    Next();
                    if(!State)
                        throw new System.FormatException($@"Could not read escape sequence identifier");

                    string chars = Parse();
                    if(chars == null)
                        throw new System.FormatException($@"Unexpected escape sequence: '{Current}'");

                    result.Append(chars);
                }
                else
                {
                    result.Append(Current);
                    Next();
                }
            }

            return result.ToString();
        }

        protected abstract string Format();
        protected abstract string Parse();

        public string Escape(string text)
        {
            string result;
            using(var reader = new StringReader(text))
            {
                result = Escape(new StringReader(text));
            }

            return result;
        }

        public string Escape(TextReader reader)
        {
            Begin(reader);
            string result = ProcessFormatting();

            Close();
            return result;
        }

        public string Unescape(string text)
        {
            string result;
            using(var reader = new StringReader(text))
            {
                result = Unescape(new StringReader(text));
            }

            return result;
        }

        public string Unescape(TextReader reader)
        {
            Begin(reader);
            string result = ProcessParsing();

            Close();
            return result;
        }

        public EscapeSequenceFormatter(char qualifier)
        {
            Qualifier = qualifier;
        }
    }
}
