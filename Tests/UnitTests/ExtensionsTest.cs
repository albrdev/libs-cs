using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using UnitTests.TestPriority;
using Libs.Extensions;

namespace UnitTests
{
    [TestCaseOrderer("UnitTests.TestPriority.PriorityOrderer", "UnitTests")]
    public class ExtensionsTest : IClassFixture<ExtensionsTestFixture>
    {
        private readonly ITestOutputHelper m_Output;
        private static ExtensionsTestFixture m_Fixture;

        public ExtensionsTest(ExtensionsTestFixture fixture, ITestOutputHelper output)
        {
            m_Fixture = fixture;
            m_Output = output;
        }

        [Fact]
        public void Test_01()
        {
        }
    }
}
