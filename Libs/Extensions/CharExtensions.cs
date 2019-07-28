using System;

namespace Libs.Extensions
{
    public static class CharExtensions
    {
        public static bool IsBinDigit(char self) => self == '0' || self == '1';
        public static bool IsOctDigit(char self) => self >= '0' && self <= '7';
        public static bool IsHexDigit(char self) => (self >= '0' && self <= '9') || (self >= 'A' && self <= 'F') || (self >= 'a' && self <= 'f');

        public static bool IsQuote(char self) => self == '\'' || self == '\"';
    }
}
