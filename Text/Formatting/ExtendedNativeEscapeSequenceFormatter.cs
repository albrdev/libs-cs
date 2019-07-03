using System;
using System.Collections.Generic;
using System.IO;

namespace Libs.Text.Formatting
{
    public class ExtendedNativeEscapeSequenceFormatter : NativeEscapeSequenceFormatter
    {
        public enum NumericBaseType
        {
            Binary = 2,
            Octal = 8,
            Decimal = 10,
            Hexadecimal = 16
        }

        public const NumericBaseType DefaultNumericBase = NumericBaseType.Hexadecimal;

        public NumericBaseType NumericBase { get; set; } = DefaultNumericBase;

        private static Dictionary<char, char> s_EscapeSequences = new Dictionary<char, char>
        {
            { '\x1B', 'e' }
        };

        private static Dictionary<char, char> s_UnescapeSequences = new Dictionary<char, char>
        {
            { 'e', '\x1B' }
        };

        protected override string Format()
        {
            char tmpChar;
            if(s_EscapeSequences.TryGetValue(Current, out tmpChar))
            {
                return tmpChar.ToString();
            }

            if(!char.IsControl(Current) || NumericBase == NumericBaseType.Hexadecimal)
                return base.Format();

            switch(NumericBase)
            {
                case NumericBaseType.Binary:
                    return $"B{System.Convert.ToString(Current, (int)NumericBase).PadLeft(8, '0')}";
                case NumericBaseType.Octal:
                    return $"o{System.Convert.ToString(Current, (int)NumericBase).PadLeft(3, '0')}";
                case NumericBaseType.Decimal:
                    return $"d{System.Convert.ToString(Current, (int)NumericBase).PadLeft(3, '0')}";
                /*case NumericBaseType.Hexadecimal:
                    identifier = 'x';
                    break;*/
                default:
                    throw new System.ArgumentException($@"Numeric base not supported: '{NumericBase}'");
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
                case 'd':
                {
                    Next();
                    string value = Next(3);
                    if(value.Length != 3)
                        throw new System.FormatException();

                    return ((char)System.Convert.ToByte(value, 10)).ToString();
                }
                case 'B':
                {
                    Next();
                    string value = Next(8);
                    if(value.Length != 8)
                        throw new System.FormatException();

                    return ((char)System.Convert.ToByte(value, 2)).ToString();
                }
                case 'o':
                {
                    Next();
                    string value = Next(3);
                    if(value.Length != 3)
                        throw new System.FormatException();

                    return ((char)System.Convert.ToByte(value, 8)).ToString();
                }
                default:
                    return base.Parse();
            }
        }

        public string Escape(string text, NumericBaseType numericBase = DefaultNumericBase, bool upperCaseNumericCodes = NativeEscapeSequenceFormatter.DefaultUpperCaseNumericCodes)
        {
            return Escape(new StringReader(text), numericBase, upperCaseNumericCodes);
        }

        public string Escape(TextReader reader, NumericBaseType numericBase = DefaultNumericBase, bool upperCaseNumericCodes = NativeEscapeSequenceFormatter.DefaultUpperCaseNumericCodes)
        {
            NumericBaseType tmpNumericBase = NumericBase;
            NumericBase = numericBase;

            string result = base.Escape(reader, upperCaseNumericCodes);

            NumericBase = tmpNumericBase;
            return result;
        }

        public ExtendedNativeEscapeSequenceFormatter(NumericBaseType numericBase = DefaultNumericBase, bool upperCaseNumericCodes = NativeEscapeSequenceFormatter.DefaultUpperCaseNumericCodes) : base(upperCaseNumericCodes)
        {
            NumericBase = numericBase;
        }
    }
}
