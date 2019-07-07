using System;
using Xunit;
using Xunit.Abstractions;
using UnitTests.TestPriority;

namespace UnitTests
{
    [TestCaseOrderer("UnitTests.TestPriority.PriorityOrderer", "UnitTests")]
    public class TextFormatterTest : IClassFixture<TextFormatterTestFixture>
    {
        private readonly ITestOutputHelper m_Output;
        private static TextFormatterTestFixture m_Fixture;

        public TextFormatterTest(TextFormatterTestFixture fixture, ITestOutputHelper output)
        {
            m_Fixture = fixture;
            m_Output = output;
        }

        [Fact] // abc 123 def
        public void RegularString()
        {
            var res = m_Fixture.TextFormatter.Format(@"abc 123 def");
            var cmp = $@"abc 123 def";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // abc $${123()} $$$${def}
        public void EscapedFunction()
        {
            var res = m_Fixture.TextFormatter.Format(@"abc $${123()} $$$${def}");
            var cmp = $@"abc ${{123()}} $${{def}}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${strargs("123\'\t\\\v\"\n456")}
        public void EscapedString()
        {
            var res = m_Fixture.TextFormatter.Format(@"${strargs(""123\'\t\\\v\""\n456"")}");
            var cmp = $@"{TextFormatterTestFixture.StrArgs("123\'\t\\\v\"\n456")}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // Array indices: ${array(0)}, ${array(2)}, ${array(4)}, ${sum(1, 2, 3, 4, 5)}
        public void FuctionCalls()
        {
            var res = m_Fixture.TextFormatter.Format(@"Array indices: ${array(0)}, ${array(2)}, ${array(4)}, ${sum(1, 2, 3, 4, 5)}");
            var cmp = $@"Array indices: {TextFormatterTestFixture.Array(0)}, {TextFormatterTestFixture.Array(2)}, {TextFormatterTestFixture.Array(4)}, {TextFormatterTestFixture.Sum(1, 2, 3, 4, 5)}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${argcnt(1, 2, 3)}${argcnt(1, 2)}
        public void FuctionCalls2()
        {
            var res = m_Fixture.TextFormatter.Format(@"${argcnt(1, 2, 3)}${argcnt(1, 2)}");
            var cmp = $@"{TextFormatterTestFixture.ArgumentCount(1, 2, 3)}{TextFormatterTestFixture.ArgumentCount(1, 2)}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${sum(1, 2, 3, 4, 5)}, ${argcnt(1, 2, 3, "abc", "def")} ${strlen("foo bar baz")})${strargs(123.456, "foo bar baz")}"${array(0)}" ${pi}, ${pi})${pi}${pi}"${pi}"
        public void FuctionCalls3()
        {
            var res = m_Fixture.TextFormatter.Format(@"${sum(1, 2, 3, 4, 5)}, ${argcnt(1, 2, 3, ""abc"", ""def"")} ${strlen(""foo bar baz"")})${strargs(123.456, ""foo bar baz"")}""${array(0)}"" ${pi}, ${pi})${pi}${pi}""${pi}""");
            var cmp = $@"{TextFormatterTestFixture.Sum(1, 2, 3, 4, 5)}, {TextFormatterTestFixture.ArgumentCount(1, 2, 3, "abc", "def")} {TextFormatterTestFixture.StringLength("foo bar baz")}){TextFormatterTestFixture.StrArgs(123.456, "foo bar baz")}""{TextFormatterTestFixture.Array(0)}"" {Math.PI}, {Math.PI}){Math.PI}{Math.PI}""{Math.PI}""";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${sum(1, 2, 3, 4, sum(1, 2, 3, 4))} ${strlen("foo bar baz")}
        public void RecursiveFuctionCalls()
        {
            var res = m_Fixture.TextFormatter.Format(@"${sum(1, 2, 3, 4, sum(1, 2, 3, 4))} ${strlen(""foo bar baz"")}");
            var cmp = $@"{TextFormatterTestFixture.Sum(1, 2, 3, 4, TextFormatterTestFixture.Sum(1, 2, 3, 4))} {TextFormatterTestFixture.StringLength("foo bar baz")}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${sum(1, argcnt(1, 2, 3), strlen("foo bar baz"), pi)}
        public void RecursiveFuctionCalls2()
        {
            var res = m_Fixture.TextFormatter.Format(@"${sum(1, argcnt(1, 2, 3), strlen(""foo bar baz""), pi)}");
            var cmp = $@"{TextFormatterTestFixture.Sum(1, TextFormatterTestFixture.ArgumentCount(1, 2, 3), TextFormatterTestFixture.StringLength("foo bar baz"), Math.PI)}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // hej $$${sum(100, 123.456, -50, argcnt(4, 5, 6))} lol ${strargs("bla\"\n\t\\\"", 123)} lol $$bla $${bla}
        public void Final_01()
        {
            var res = m_Fixture.TextFormatter.Format(@"hej $$${sum(100, 123.456, -50, argcnt(4, 5, 6))} lol ${strargs(""bla\""\n\t\\\"""", 123)} lol $$bla $${bla}");
            var cmp = $@"hej $176.456 lol str=""{"bla\"\n\t\\\", 123"}"" lol $bla ${{bla}}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // hej ${sum(1, 2, 3) + argcnt(1, 2, 3)}bla${1 + "bla" + 2}$${bla}
        public void Final_02()
        {
            var res = m_Fixture.TextFormatter.Format(@"hej ${sum(1, 2, 3) + argcnt(1, 2, 3)}bla${1 + ""bla"" + 2}$${bla}");
            var cmp = $@"hej {$"{TextFormatterTestFixture.Sum(1, 2, 3)}{TextFormatterTestFixture.ArgumentCount(1, 2, 3)}"}bla{"1bla2"}${{bla}}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${1+-2}
        public void Test_01()
        {
            var res = m_Fixture.TextFormatter.Format(@"${1+-2}");
            var cmp = $@"1-2";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${1+-2}
        public void Test_02()
        {
            var res = m_Fixture.TextFormatter.Format(@"${pi + (""pi"" + pi + ""bla"")}");
            var cmp = $@"{Math.PI}pi{Math.PI}bla";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ${strlen("abcdefghijklmnopqrstuvwxyz") + strlen("0123456789")}
        public void Test_03()
        {
            var res = m_Fixture.TextFormatter.Format(@"${strlen(""abcdefghijklmnopqrstuvwxyz"") + strlen(""0123456789"")}");
            var cmp = $@"{TextFormatterTestFixture.StringLength("abcdefghijklmnopqrstuvwxyz")}{TextFormatterTestFixture.StringLength("0123456789")}";

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }
    }
}
