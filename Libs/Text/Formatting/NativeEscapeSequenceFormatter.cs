using System;
using System.Collections.Generic;
using System.IO;

namespace Libs.Text.Formatting
{
    public class NativeEscapeSequenceFormatter : EscapeSequenceFormatter
    {
        public const char DefaultQualifier = '\\';
        public const bool DefaultUpperCaseNumericCodes = false;

        public bool UpperCaseNumericCodes { get; set; } = DefaultUpperCaseNumericCodes;

        private static Dictionary<char, char> s_EscapeSequences = new Dictionary<char, char>
        {
            { '\a', 'a' },
            { '\b', 'b' },
            { '\f', 'f' },
            { '\n', 'n' },
            { '\r', 'r' },
            { '\t', 't' },
            { '\v', 'v' },
            { '\'', '\'' },
            { '\"', '\"' },
            { '\\', '\\' },
            { '\0', '0' }
        };

        private static Dictionary<char, char> s_UnescapeSequences = new Dictionary<char, char>
        {
            { 'a', '\a' },
            { 'b', '\b' },
            { 'f', '\f' },
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
            { 'v', '\v' },
            { '\'', '\'' },
            { '\"', '\"' },
            { '\\', '\\' },
            { '0', '\0' }
        };

        protected override string Format()
        {
            char tmpChar;
            if(s_EscapeSequences.TryGetValue(Current, out tmpChar))
            {
                return tmpChar.ToString();
            }
            else if(char.IsControl(Current))
            {
                if(Current > byte.MaxValue)
                {
                    return $"x{System.Convert.ToUInt16(Current).ToString($"{(UpperCaseNumericCodes ? 'X' : 'x')}4")}";
                }
                else
                {
                    return $"x{System.Convert.ToUInt16(Current).ToString($"{(UpperCaseNumericCodes ? 'X' : 'x')}2")}";
                }
            }
            else if(char.IsHighSurrogate(Current))
            {
                tmpChar = Current;
                Next();
                if(!State)
                    throw new System.FormatException($@"Could not read low part of surrogate character pair: 'x{System.Convert.ToString(tmpChar, 16).PadLeft(4, '0')}'");

                if(!char.IsLowSurrogate(Current))
                    throw new System.FormatException($@"Invalid low part surrogate character: 'x{System.Convert.ToString(Current, 16).PadLeft(4, '0')}'");

                return $"U{char.ConvertToUtf32(tmpChar, Current).ToString($"{(UpperCaseNumericCodes ? 'X' : 'x')}8")}";
            }
            else
            {
                return null;
            }
        }

        protected override string Parse()
        {
            char tmpChar;
            if(s_UnescapeSequences.TryGetValue(Current, out tmpChar))
            {
                Next();
                return tmpChar.ToString();
            }

            switch(Current)
            {
                case 'x':
                {
                    Next();
                    string value = Next(2);
                    if(value.Length != 2)
                        throw new System.FormatException();

                    return ((char)System.Convert.ToByte(value, 16)).ToString();
                }
                case 'u':
                {
                    Next();
                    string value = Next(4);
                    if(value.Length != 4)
                        throw new System.FormatException();

                    return ((char)System.Convert.ToUInt16(value, 16)).ToString();
                }
                case 'U':
                {
                    Next();
                    string value = Next(8);
                    if(value.Length != 8)
                        throw new System.FormatException();

                    return char.ConvertFromUtf32(System.Convert.ToInt32(value, 16));
                }
                default:
                    return null;
            }
        }

        public string Escape(string text, bool upperCaseNumericCodes = DefaultUpperCaseNumericCodes)
        {
            return Escape(new StringReader(text), upperCaseNumericCodes);
        }

        public string Escape(TextReader reader, bool upperCaseNumericCodes = DefaultUpperCaseNumericCodes)
        {
            bool tmpUpperCaseNumericCodes = UpperCaseNumericCodes;
            UpperCaseNumericCodes = upperCaseNumericCodes;

            string result = base.Escape(reader);

            UpperCaseNumericCodes = tmpUpperCaseNumericCodes;
            return result;
        }

        public NativeEscapeSequenceFormatter(bool upperCaseNumericCodes = DefaultUpperCaseNumericCodes) : base(DefaultQualifier)
        {
            UpperCaseNumericCodes = upperCaseNumericCodes;
        }
    }
}
