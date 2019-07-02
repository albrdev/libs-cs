using System;

namespace Libs.Text.Formatting
{
    public class PercentEncoder : EscapeSequenceFormatter
    {
        public const char DefaultQualifier = '%';

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
            if(Array.IndexOf(m_GenDelims, Current) != -1 || Array.IndexOf(m_SubDelims, Current) != -1)
            {
                return System.Convert.ToString(Current, 16).PadLeft(2, '0');
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

        public PercentEncoder() : base(DefaultQualifier) { }
    }
}
