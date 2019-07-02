using System;
using System.IO;
using System.Linq;

namespace Libs.Text.Formatting
{
    public class PercentEncoder : EscapeSequenceFormatter
    {
        public const char DefaultQualifier = '%';
        public const bool DefaultNumericBaseUpperCase = true;

        public bool NumericBaseUpperCase { get; set; } = DefaultNumericBaseUpperCase;

        private char[] m_GenDelims = new char[]
        {
            ':',
            '/',
            '?',
            '#',
            '[',
            ']',
            '@'
        };

        private char[] m_SubDelims = new char[]
        {
            '!',
            '$',
            '&',
            '\'',
            '(',
            ')',
            '*',
            '+',
            ',',
            ';',
            '='
        };

        protected override string Format()
        {
            if(m_GenDelims.Contains(Current) || m_SubDelims.Contains(Current) || Current == ' ')
            {
                return ((ushort)Current).ToString($"{(NumericBaseUpperCase ? 'X' : 'x')}2");
            }

            return null;
        }

        protected override string Parse()
        {
            string value = Next(2);
            if(value.Length != 2)
                throw new System.FormatException();

            return ((char)System.Convert.ToByte(value, 16)).ToString();
        }

        public string Escape(string text, bool numericBaseUpperCase)
        {
            return Escape(new StringReader(text), numericBaseUpperCase);
        }

        public string Escape(TextReader reader, bool numericBaseUpperCase)
        {
            bool tmpNumericBase = NumericBaseUpperCase;
            NumericBaseUpperCase = numericBaseUpperCase;

            string result = Escape(reader);

            NumericBaseUpperCase = tmpNumericBase;
            return result;
        }

        public PercentEncoder() : base(DefaultQualifier) { }
    }
}
