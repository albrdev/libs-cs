using System;
using System.Collections.Generic;
using Xunit;
using Libs.Collections;
using Libs.Text.Parsing;
using static Libs.Text.Parsing.Operator;
using Libs.Text.Formatting;
using Xunit.Abstractions;

namespace UnitTests
{
    public class EscapeSequenceFormatterTest
    {
        private readonly ITestOutputHelper m_Output;

        public EscapeSequenceFormatterTest(ITestOutputHelper output)
        {
            m_Output = output;
        }

        private static ExtendedNativeEscapeSequenceFormatter s_ExtendedNativeEscapeSequenceFormatter = new ExtendedNativeEscapeSequenceFormatter();
        private static NativeEscapeSequenceFormatter s_NativeEscapeSequenceFormatter = new NativeEscapeSequenceFormatter();
        private static XMLEscapeSequenceFormatter s_XMLEscapeSequenceFormatter = new XMLEscapeSequenceFormatter();
        private static PercentEncoder s_PercentEncoder = new PercentEncoder();

        [Fact] // \U0001D162\\\"\'\0\x1B
        public void ExtendedNativeEscape_01()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Escape("\U0001D162\\\"\'\0\x1B");
            var cmp = @"\U0001d162\\\""\'\0\e";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // \\\t\'\v\"\n\\
        public void ExtendedNativeEscape_02()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Escape("\\\t\'\v\"\n\\");
            var cmp = @"\\\t\'\v\""\n\\";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // a\x0001a\x0002a\x0003a
        public void ExtendedNativeEscape_03()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Escape("a\x0001a\x0002a\x0003a", ExtendedNativeEscapeSequenceFormatter.NumericBaseType.Binary);
            var cmp = @"a\B00000001a\B00000010a\B00000011a";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // a\x0001a\u0002\x001aa\\a\\\na
        public void ExtendedNativeEscape_04()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Escape("a\x0001a\u0002\x001aa\\a\\\na", true);
            var cmp = @"a\x01a\x02\x1Aa\\a\\\na";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // \U0001d162\\\"\'\0\e
        public void ExtendedNativeUnescape_01()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Unescape(@"\U0001d162\\\""\'\0\e");
            var cmp = "\U0001D162\\\"\'\0\x1B";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // \\\t\'\v\"\n\\
        public void ExtendedNativeUnescape_02()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Unescape(@"\\\t\'\v\""\n\\");
            var cmp = "\\\t\'\v\"\n\\";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // a\B00000001a\B00000010a\B00000011a
        public void ExtendedNativeUnescape_03()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Unescape(@"a\B00000001a\B00000010a\B00000011a");
            var cmp = "a\x0001a\x0002a\x0003a";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // a\x01a\x02\x1Aa\\a\\\na
        public void ExtendedNativeUnescape_04()
        {
            var res = s_ExtendedNativeEscapeSequenceFormatter.Unescape(@"a\x01a\x02\x1Aa\\a\\\na");
            var cmp = "a\x0001a\u0002\x001aa\\a\\\na";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // abc"'<abc>&abc
        public void XMLEscape_01()
        {
            var res = s_XMLEscapeSequenceFormatter.Escape(@"abc""'<abc>&abc");
            var cmp = @"abc&quot;&apos;&lt;abc&gt;&amp;abc";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // abc&quot;&apos;&lt;abc&gt;&amp;abc
        public void XMLUnescape_01()
        {
            var res = s_XMLEscapeSequenceFormatter.Unescape(@"abc&quot;&apos;&lt;abc&gt;&amp;abc");
            var cmp = @"abc""'<abc>&abc";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // abc !*'();:@abc&=+$,/?#[]abc
        public void PercentEncoding_01()
        {
            var res = s_PercentEncoder.Escape(@"abc !*'();:@abc&=+$,/?#[]abc");
            var cmp = @"abc%20%21%2A%27%28%29%3B%3A%40abc%26%3D%2B%24%2C%2F%3F%23%5B%5Dabc";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // abc%20%21%2A%27%28%29%3B%3A%40abc%26%3D%2B%24%2C%2F%3F%23%5B%5Dabc
        public void PercentDecoding_01()
        {
            var res = s_PercentEncoder.Unescape(@"abc%20%21%2A%27%28%29%3B%3A%40abc%26%3D%2B%24%2C%2F%3F%23%5B%5Dabc");
            var cmp = @"abc !*'();:@abc&=+$,/?#[]abc";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }
    }
}
