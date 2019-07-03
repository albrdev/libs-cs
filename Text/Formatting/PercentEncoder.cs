using System;
using System.IO;
using System.Linq;

namespace Libs.Text.Formatting
{
    public class PercentEncoder : EscapeSequenceFormatter
    {
        public const char DefaultQualifier = '%';
        public const bool DefaultNumericSpaceEncoding = true;
        public const bool DefaultNumericBaseUpperCase = true;

        public bool NumericSpaceEncoding { get; set; } = DefaultNumericSpaceEncoding;
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
            if(Current == ' ' && !NumericSpaceEncoding)
            {
                QualifierEnabled = false;
                return '+'.ToString();
            }
            else if(m_GenDelims.Contains(Current) || m_SubDelims.Contains(Current) || Current == ' ')
            {
                QualifierEnabled = true;
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

        public string Escape(TextReader reader, bool numericSpaceEncoding = DefaultNumericSpaceEncoding, bool numericBaseUpperCase = DefaultNumericBaseUpperCase)
        {
            bool tmpNumericSpaceEncoding = NumericSpaceEncoding;
            NumericSpaceEncoding = numericSpaceEncoding;
            bool tmpNumericBase = NumericBaseUpperCase;
            NumericBaseUpperCase = numericBaseUpperCase;

            string result = Escape(reader);

            NumericSpaceEncoding = tmpNumericSpaceEncoding;
            NumericBaseUpperCase = tmpNumericBase;
            return result;
        }

        public PercentEncoder(bool numericSpaceEncoding = DefaultNumericSpaceEncoding, bool numericBaseUpperCase = DefaultNumericBaseUpperCase) : base(DefaultQualifier)
        {
            NumericSpaceEncoding = numericSpaceEncoding;
            NumericBaseUpperCase = numericBaseUpperCase;
        }
    }
}
