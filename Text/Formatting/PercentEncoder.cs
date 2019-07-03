using System;
using System.IO;
using System.Linq;

namespace Libs.Text.Formatting
{
    public class PercentEncoder : EscapeSequenceFormatter
    {
        public const char DefaultQualifier = '%';
        public const bool DefaultNumericEncodedSpaces = true;
        public const bool DefaultUpperCaseNumericCodes = true;

        public bool NumericEncodedSpaces { get; set; } = DefaultNumericEncodedSpaces;
        public bool UpperCaseNumericCodes { get; set; } = DefaultUpperCaseNumericCodes;

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
            if(Current == ' ' && !NumericEncodedSpaces)
            {
                QualifierEnabled = false;
                return '+'.ToString();
            }
            else if(m_GenDelims.Contains(Current) || m_SubDelims.Contains(Current) || Current == ' ')
            {
                QualifierEnabled = true;
                return ((ushort)Current).ToString($"{(UpperCaseNumericCodes ? 'X' : 'x')}2");
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

        public string Escape(string text, bool numericSpaceEncoding = DefaultNumericEncodedSpaces, bool upperCaseNumericCodes = DefaultUpperCaseNumericCodes)
        {
            return Escape(new StringReader(text), numericSpaceEncoding, upperCaseNumericCodes);
        }

        public string Escape(TextReader reader, bool numericSpaceEncoding = DefaultNumericEncodedSpaces, bool upperCaseNumericCodes = DefaultUpperCaseNumericCodes)
        {
            bool tmpNumericEncodedSpaces = NumericEncodedSpaces;
            NumericEncodedSpaces = numericSpaceEncoding;
            bool tmpUpperCaseNumericCodes = UpperCaseNumericCodes;
            UpperCaseNumericCodes = upperCaseNumericCodes;

            string result = base.Escape(reader);

            NumericEncodedSpaces = tmpNumericEncodedSpaces;
            UpperCaseNumericCodes = tmpUpperCaseNumericCodes;
            return result;
        }

        public PercentEncoder(bool numericSpaceEncoding = DefaultNumericEncodedSpaces, bool upperCaseNumericCodes = DefaultUpperCaseNumericCodes) : base(DefaultQualifier)
        {
            NumericEncodedSpaces = numericSpaceEncoding;
            UpperCaseNumericCodes = upperCaseNumericCodes;
        }
    }
}
