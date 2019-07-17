using System;
using Libs.Text.Parsing;
using Libs.Text.Formatting;

namespace UnitTests
{
    public class CommandParserTestFixture : IDisposable
    {
        public EscapeSequenceFormatter EscapeSequenceFormatter { get; private set; }

        #region Custom methods
        private static object s_Setting = "foo";

        public static object GetSetting(params object[] args)
        {
            return s_Setting;
        }

        public static object SetSetting(params object[] args)
        {
            if(args.Length < 1)
                throw new System.Exception();

            s_Setting = args[0];
            return null;
        }

        public static object Setting(params object[] args)
        {
            if(args.Length <= 0)
                return s_Setting;

            s_Setting = args[0];
            return null;
        }
        #endregion

        public CommandParser CommandParser { get; set; }

        public CommandParserTestFixture()
        {
            EscapeSequenceFormatter = new ExtendedNativeEscapeSequenceFormatter();

            CommandParser = new CommandParser(EscapeSequenceFormatter)
            {
                { "get_setting", GetSetting },
                { "set_setting", SetSetting },
                { "setting", Setting }
            };
        }

        public void Dispose()
        {
        }
    }
}
