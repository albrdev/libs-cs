using System;
using System.Globalization;

namespace Libs.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceAt(this string self, int index, string replacement)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            int endIndex = index + replacement.Length;
            string result = self.Substring(0, index) + replacement;
            return endIndex < self.Length ? string.Concat(result, self.Substring(endIndex)) : result;
        }

        public static string ReplaceAt(this string self, int startIndex, int endIndex, string replacement)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return string.Concat(self.Substring(0, startIndex), replacement, self.Substring(startIndex + endIndex));
        }

        public static string ReplaceFirstOccurrence(this string self, string value, string replacement)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            int index = self.IndexOf(value);
            return index >= 0 ? self.ReplaceAt(index, replacement) : self;
        }

        public static string ReplaceLastOccurrence(this string self, string value, string replacement)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            int index = self.LastIndexOf(value);
            return index >= 0 ? self.ReplaceAt(index, replacement) : self;
        }

        public static string ToTitleCase(this string self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(self.ToLower());
        }

        public static string ToTitleCase(this string self, CultureInfo culture)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return culture.TextInfo.ToTitleCase(self.ToLower());
        }
    }
}
