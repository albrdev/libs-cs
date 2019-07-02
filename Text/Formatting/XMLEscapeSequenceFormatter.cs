using System;
using System.Collections.Generic;

namespace Libs.Text.Formatting
{
    public class XMLEscapeSequenceFormatter : EscapeSequenceFormatter
    {
        public const char DefaultQualifier = '&';

        private const char k_SequenceTerminator = ';';

        private static Dictionary<char, string> s_EscapeSequences = new Dictionary<char, string>
        {
            { '\"', "quot" },
            { '\'', "apos" },
            { '<', "lt" },
            { '>', "gt" },
            { '&', "amp" }
        };

        private static Dictionary<string, char> s_UnescapeSequences = new Dictionary<string, char>
        {
            { "quot", '\"' },
            { "apos", '\'' },
            { "lt", '<' },
            { "gt", '>' },
            { "amp", '&' }
        };

        protected override string Format()
        {
            string sequence;
            if(s_EscapeSequences.TryGetValue(Current, out sequence))
            {
                return $"{sequence}{k_SequenceTerminator}";
            }

            return null;
        }

        protected override string Parse()
        {
            char chr;
            if(s_UnescapeSequences.TryGetValue(Next((character) => character != k_SequenceTerminator), out chr))
            {
                if(Current != k_SequenceTerminator)
                    throw new FormatException();

                Next();
                return chr.ToString();
            }

            return null;
        }

        public XMLEscapeSequenceFormatter() : base(DefaultQualifier) { }
    }
}
