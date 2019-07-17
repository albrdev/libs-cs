using System;
using Xunit;
using Xunit.Abstractions;
using UnitTests.TestPriority;

namespace UnitTests
{
    [TestCaseOrderer("UnitTests.TestPriority.PriorityOrderer", "Tests")]
    public class CommandParserTest : IClassFixture<CommandParserTestFixture>
    {
        private readonly ITestOutputHelper m_Output;
        private static CommandParserTestFixture m_Fixture;

        public CommandParserTest(CommandParserTestFixture fixture, ITestOutputHelper output)
        {
            m_Fixture = fixture;
            m_Output = output;
        }

        [Fact, TestPriority(1)] // get_setting
        public void GetSetting_01()
        {
            var res = m_Fixture.CommandParser.Execute(@"get_setting");
            var cmp = $@"foo";

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(3)] // get_setting
        public void GetSetting_02()
        {
            var res = m_Fixture.CommandParser.Execute(@"get_setting");
            var cmp = $@"bar";

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(2)] // set_setting bar
        public void SetSetting_01()
        {
            var res = m_Fixture.CommandParser.Execute(@"set_setting bar");
            object cmp = null;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(4)] // setting
        public void Setting_01()
        {
            var res = m_Fixture.CommandParser.Execute(@"setting");
            var cmp = $@"bar";

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(5)] // setting foo
        public void Setting_02()
        {
            var res = m_Fixture.CommandParser.Execute(@"setting foo");
            object cmp = null;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(6)] // setting
        public void Setting_03()
        {
            var res = m_Fixture.CommandParser.Execute(@"setting");
            var cmp = $@"foo";

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }
    }
}
