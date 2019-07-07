using System;
using System.Collections.Generic;
using Libs.Text.Formatting;

namespace UnitTests
{
    public class TextFormatterTestFixture : IDisposable
    {
        public EscapeSequenceFormatter EscapeSequenceFormatter { get; private set; }

        #region Custom methods
        public static object Sum(params object[] args)
        {
            double result = 0D;
            foreach(var arg in args)
            {
                result += System.Convert.ToDouble(arg);
            }

            return result;
        }

        public static object StringLength(params object[] args)
        {
            return args.Length > 0 ? args[0].ToString().Length : 0;
        }

        public static object ArgumentCount(params object[] args)
        {
            return args.Length;
        }

        public static object StrArgs(params object[] args)
        {
            return $@"str=""{ string.Join(", ", args) }""";
        }

        public static object Array(params object[] args)
        {
            string[] names = { "First", "Second", "Third", "Fourth", "Fifth" };
            return names[System.Convert.ToInt32(args[0])];
        }
        #endregion

        private static Dictionary<string, object> m_Variables = new Dictionary<string, object>
        {
            { "pi", Math.PI }
        };

        private static Dictionary<string, FunctionNode.Evaluator> m_Functions = new Dictionary<string, FunctionNode.Evaluator>
        {
            { "sum",  Sum },
            { "strlen",  StringLength },
            { "argcnt",  ArgumentCount },
            { "strargs",  StrArgs },
            { "array", Array }
        };

        public TextFormatter TextFormatter { get; set; }

        public TextFormatterTestFixture()
        {
            EscapeSequenceFormatter = new NativeEscapeSequenceFormatter();

            m_Variables = new Dictionary<string, object>
            {
                { "pi", Math.PI }
            };
            m_Functions = new Dictionary<string, FunctionNode.Evaluator>
            {
                { "sum",  Sum },
                { "strlen",  StringLength },
                { "argcnt",  ArgumentCount },
                { "strargs",  StrArgs },
                { "array", Array }
            };

            TextFormatter = new TextFormatter('$', m_Variables, m_Functions, EscapeSequenceFormatter);
        }

        public void Dispose()
        {
        }
    }
}
