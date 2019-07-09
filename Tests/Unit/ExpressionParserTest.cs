using System;
using Xunit;
using Xunit.Abstractions;
using UnitTests.TestPriority;

namespace UnitTests
{
    [TestCaseOrderer("UnitTests.TestPriority.PriorityOrderer", "UnitTests")]
    public class ExpressionParserTest : IClassFixture<ExpressionParserTestFixture>
    {
        private readonly ITestOutputHelper m_Output;
        private static ExpressionParserTestFixture m_Fixture;

        public ExpressionParserTest(ExpressionParserTestFixture fixture, ITestOutputHelper output)
        {
            m_Fixture = fixture;
            m_Output = output;
        }

        [Fact, TestPriority(1)] // 1 + 10^2^3
        public void Arithmetic_01()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"1 + 10^2^3");
            double cmp = 1 + (double)ExpressionParserTestFixture.Exponentiation(10, ExpressionParserTestFixture.Exponentiation(2, 3));

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            m_Fixture.ResultVariables.Add(res);
            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(2)] // 3 + --4
        public void Arithmetic_02()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"3 + --4");
            double cmp = 3 + -(-4);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            m_Fixture.ResultVariables.Add(res);
            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(3)] // 2math.pi
        public void Arithmetic_03()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"2math.pi");
            double cmp = 2 * Math.PI;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            m_Fixture.ResultVariables.Add(res);
            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(4)] // 3 + 4 * 2 / (1 - 5) ^ 2 ^ 3
        public void Arithmetic_04()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"3 + 4 * 2 / (1 - 5) ^ 2 ^ 3");
            double cmp = 3 + 4 * 2 / (double)ExpressionParserTestFixture.Exponentiation((1 - 5), ExpressionParserTestFixture.Exponentiation(2, 3));

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            m_Fixture.ResultVariables.Add(res);
            Assert.Equal(cmp, res);
        }

        [Fact, TestPriority(5)] // sin(max(2, 3) / 3 * math.pi)
        public void Arithmetic_05()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"sin(max(2, 3) / 3 * math.pi)");
            double cmp = Math.Sin((double)ExpressionParserTestFixture.Max(2, 3) / 3 * Math.PI);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            m_Fixture.ResultVariables.Add(res);
            Assert.Equal(cmp, res);
        }

        [Fact] // 4 + 5 * (5 + 2)
        public void Arithmetic_06()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"4 + 5 * (5 + 2)");
            double cmp = 4 + 5 * (5 + 2);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla = (1 + 1) * 2")
        public void Arithmetic_07()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"bla = (1 + 1) * 2");
            double cmp = (1 + 1) * 2;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla * 5
        public void Arithmetic_08()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"bla * 5");
            double cmp = 2 * 5;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla2 = -(1 + 1) * 4 + -bla2
        public void Arithmetic_09()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"bla2 = -(1 + 1) * 4 + -bla2");
            double cmp = -(1 + 1) * 4 + -(-2);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla3 = (10^2^3) + bla3
        public void Arithmetic_10()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"bla3 = (10^2^3) + bla3");
            double cmp = (double)ExpressionParserTestFixture.Exponentiation(10, ExpressionParserTestFixture.Exponentiation(2, 3)) + (double)ExpressionParserTestFixture.Exponentiation(10, ExpressionParserTestFixture.Exponentiation(2, 3));

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 2^1da^-5
        public void Arithmetic_11()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"2^1da^-5");
            double cmp = (double)ExpressionParserTestFixture.Exponentiation(2, ExpressionParserTestFixture.Exponentiation((1 * 10), -5));

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // -(-5)
        public void Arithmetic_12()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"-(-5)");
            double cmp = -(-5);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // min(5, -5)
        public void Arithmetic_13()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"min(5, -5)");
            double cmp = (double)ExpressionParserTestFixture.Min(5, -5);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 2 ** 3
        public void Arithmetic_14()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"2 ** 3");
            double cmp = (double)ExpressionParserTestFixture.Exponentiation(2, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 10 / 3
        public void Arithmetic_15()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"10 / 3");
            double cmp = 10.0 / 3.0;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 10 // 3
        public void Arithmetic_16()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"10 // 3");
            double cmp = Math.Truncate(10.0 / 3.0);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1da^3 * (10 - 2 * 2)
        public void Arithmetic_17()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"1da^3 * (10 - 2 * 2)");
            double cmp = (double)ExpressionParserTestFixture.Exponentiation((1 * 10), 3) * (10 - 2 * 2);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (1 + 2) * 5 # comment
        public void Arithmetic_18()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"(1 + 2) * 5 # comment");
            double cmp = (1 + 2) * 5;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (phys.au / phys.c) / 60 # How long light travels from the Sun to Earth
        public void Arithmetic_19()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"(phys.au / phys.c) / 60 # How long light travels from the Sun to Earth");
            double cmp = (System.Convert.ToDouble(m_Fixture.ArithmeticVariables["phys.au"].Value) / System.Convert.ToDouble(m_Fixture.ArithmeticVariables["phys.c"].Value)) / 60;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 384402000 / phys.c # How long light travels from the Moon to Earth
        public void Arithmetic_20()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"384402000 / phys.c # How long light travels from the Moon to Earth");
            double cmp = 384402000 / System.Convert.ToDouble(m_Fixture.ArithmeticVariables["phys.c"].Value);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // -10^3
        public void Arithmetic_21()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"-10^3");
            double cmp = (double)ExpressionParserTestFixture.Exponentiation(-10, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // -(10^3)
        public void Arithmetic_22()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"-10^3");
            double cmp = -(double)ExpressionParserTestFixture.Exponentiation(10, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 + -(10^3)
        public void Arithmetic_23()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"1 + -(10^3)");
            double cmp = 1 + -(double)ExpressionParserTestFixture.Exponentiation(10, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // x * x - y * y
        public void Arithmetic_24()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"x * x - y * y", ("x", 10), ("y", 5));
            double cmp = 10 * 10 - 5 * 5;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // ans(0) + ans(-4) - ans()
        public void Arithmetic_25()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"ans(0) + ans(-4) - ans()");
            double cmp = System.Convert.ToDouble(m_Fixture.Ans(0)) + System.Convert.ToDouble(m_Fixture.Ans(-4)) - System.Convert.ToDouble(m_Fixture.Ans());

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 + pi() * 2
        public void Arithmetic_26()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"1 + pi() * 2");
            double cmp = 1 + System.Convert.ToDouble(ExpressionParserTestFixture.PI()) * 2;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // "abc" + "123"
        public void Arithmetic_27()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"""abc"" + ""123""");
            var cmp = $@"abc123";

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // lol + (lol = 1)
        public void Arithmetic_28()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"lol + (lol = 1)");
            double cmp = 1 + 1;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // lol + (lol = max(1, 2, 3))
        public void Arithmetic_29()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"lol + (lol = max(1, 2, 3))");
            double cmp = 3 + (double)ExpressionParserTestFixture.Max(1, 2, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // lol + neg(lol = max(1, 2, 3))
        public void Arithmetic_30()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"lol + neg(lol = max(1, 2, 3))");
            double cmp = -3 + (double)ExpressionParserTestFixture.Max(1, 2, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // lol + -(lol = max(1, 2, 3))
        public void Arithmetic_31()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"lol + -(lol = max(1, 2, 3))");
            double cmp = 3 + -(double)ExpressionParserTestFixture.Max(1, 2, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // lol + (lol = -max(1, 2, 3))
        public void Arithmetic_32()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"lol + (lol = -max(1, 2, 3))");
            double cmp = -3 + -(double)ExpressionParserTestFixture.Max(1, 2, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // lol = 0
        public void Arithmetic_33()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"lol = 0");
            int cmp = 0;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 2min(2, 1, 3)
        public void Arithmetic_34()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"2min(2, 1, 3)");
            double cmp = 2 * (double)ExpressionParserTestFixture.Min(2, 1, 3);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 2math.pi
        public void Arithmetic_35()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"2math.pi");
            double cmp = 2 * System.Convert.ToDouble(m_Fixture.ArithmeticVariables["math.pi"].Value);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (1 + 1)3
        public void Arithmetic_36()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"(1 + 1)3");
            double cmp = (1 + 1) * 3;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // min(2, 1, 3)3
        public void Arithmetic_37()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"min(2, 1, 3)3");
            double cmp = (double)ExpressionParserTestFixture.Min(2, 1, 3) * 3;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (1 + 1)math.pi
        public void Arithmetic_38()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"(1 + 1)math.pi");
            double cmp = (1 + 1) * System.Convert.ToDouble(m_Fixture.ArithmeticVariables["math.pi"].Value);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // min(2, 1, 3)max(2, 3, 1)
        public void Arithmetic_39()
        {
            var res = m_Fixture.ArithmeticExpressionParser.Evaluate(@"min(2, 1, 3)max(2, 3, 1)");
            double cmp = (double)ExpressionParserTestFixture.Min(2, 1, 3) * (double)ExpressionParserTestFixture.Max(2, 3, 1);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 + 0 * 1
        public void Bitwise_01()
        {
            var res = m_Fixture.BitwiseExpressionParser.Evaluate(@"1 + 0 * 1");
            int cmp = 1 | 0 & 1;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 ^ (1 * 1) * 0
        public void Bitwise_02()
        {
            var res = m_Fixture.BitwiseExpressionParser.Evaluate(@"1 ^ (1 * 1) * 0");
            int cmp = 1 ^ (1 & 1) & 0;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // !(0 + 0) ^ !(1 * 1)
        public void Bitwise_03()
        {
            var res = m_Fixture.BitwiseExpressionParser.Evaluate(@"!(0 + 0) ^ !(1 * 1)");
            int cmp = ~(0 | 0) ^ ~(1 & 1);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 << 10
        public void Bitwise_04()
        {
            var res = m_Fixture.BitwiseExpressionParser.Evaluate(@"1 << 10");
            int cmp = 1 << 10;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (1 << 10) * 1023
        public void Bitwise_05()
        {
            var res = m_Fixture.BitwiseExpressionParser.Evaluate(@"(1 << 10) * 1023");
            int cmp = (1 << 10) & 1023;

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 ^ 1(1 + 0)
        public void Bitwise_06()
        {
            var res = m_Fixture.BitwiseExpressionParser.Evaluate(@"1 ^ 1(1 + 0)");
            int cmp = 1 ^ 1 & (1 | 0);

            m_Output.WriteLine($@"""{ cmp }""");
            m_Output.WriteLine($@"""{ res }""");

            Assert.Equal(cmp, res);
        }
    }
}
