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
    public class ExpressionParserTest
    {
        private readonly ITestOutputHelper m_Output;

        public ExpressionParserTest(ITestOutputHelper output)
        {
            m_Output = output;
        }

        private static UnaryOperator s_AssignmentOperator = ('=', 1, AssociativityType.Right, (value) => value);
        private static EscapeSequenceFormatter s_EscapeSequenceFormatter = new NativeEscapeSequenceFormatter();

        #region Arithmetic
        private static object Min(params object[] args)
        {
            double result = double.MaxValue;
            foreach(var arg in args)
            {
                double tmp = System.Convert.ToDouble(arg);
                if(tmp < result)
                {
                    result = tmp;
                }
            }

            return result;
        }

        private static object Max(params object[] args)
        {
            double result = double.MinValue;
            foreach(var arg in args)
            {
                double tmp = System.Convert.ToDouble(arg);
                if(tmp > result)
                {
                    result = tmp;
                }
            }

            return result;
        }

        private static object Exponentiation(params object[] args)
        {
            double a = System.Convert.ToDouble(args[0]);
            double b = System.Convert.ToDouble(args[1]);

            double result = Math.Pow(Math.Abs(a), b);
            return a < 0.0 && ((long)b & 1) == 1 ? -result : result;
        }

        private static object Root(params object[] args)
        {
            double a = System.Convert.ToDouble(args[0]);
            double b = 1.0 / System.Convert.ToDouble(args[1]);

            double result = Math.Pow(Math.Abs(a), b);
            return a < 0.0 && ((long)b & 1) == 1 ? -result : result;
        }

        private static ExtendedDictionary<char, UnaryOperator> s_ArithmeticUnaryOperators = new ExtendedDictionary<char, UnaryOperator>((value) => value.Identifier)
        {
            ( '+', 1, AssociativityType.Right,  (value) => +System.Convert.ToDouble(value) ),
            ( '-', 1, AssociativityType.Right,  (value) => -System.Convert.ToDouble(value) )
        };

        private static ExtendedDictionary<string, BinaryOperator> s_ArithmeticBinaryOperators = new ExtendedDictionary<string, BinaryOperator>((value) => value.Identifier)
        {
            ( "+", 4, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) + System.Convert.ToDouble(rhs) ),
            ( "-", 4, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) - System.Convert.ToDouble(rhs) ),
            ( "*", 3, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) * System.Convert.ToDouble(rhs) ),
            ( "/", 3, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) / System.Convert.ToDouble(rhs) ),
            ( "%", 3, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) % System.Convert.ToDouble(rhs) ),
            ( "^", 2, AssociativityType.Right,  (lhs, rhs) => Exponentiation(lhs, rhs) ),

            ( "//", 3, AssociativityType.Right, (lhs, rhs) => Math.Truncate(System.Convert.ToDouble(lhs) / System.Convert.ToDouble(rhs)) ),
            ( "**", 2, AssociativityType.Left,  (lhs, rhs) => Exponentiation(lhs, rhs) )
        };

        private static BinaryOperator s_ArithmeticAbbreviationOperator = ("*", 3, AssociativityType.Right, (lhs, rhs) => System.Convert.ToDouble(lhs) * System.Convert.ToDouble(rhs));

        private static Dictionary<string, object> s_ArithmeticVariables = new Dictionary<string, object>
        {
            { "T",          1000000000000.0 },
            { "G",          1000000000.0 },
            { "M",          1000000.0 },
            { "k",          1000.0 },
            { "h",          100.0 },
            { "da",         10.0 },
            { "d",          0.1 },
            { "c",          0.01 },
            { "m",          0.001 },
            { "u",          0.000001 },
            { "n",          0.000000001 },
            { "p",          0.000000000001 },

            { "math.pi",    Math.PI },
            { "math.e",     Math.E },
            { "phys.c",     299792458 },
            { "phys.au",    149597870700 }
        };

        private static ExtendedDictionary<string, Function> s_ArithmeticFunctions = new ExtendedDictionary<string, Function>((value) => value.Identifier)
        {
            ( "abs",        1,      (args) => Math.Abs(System.Convert.ToDouble(args[0])) ),
            ( "neg",        1,      (args) => -Math.Abs(System.Convert.ToDouble(args[0])) ),
            ( "pow",        2,      Exponentiation ),
            ( "root",       2,      Root ),
            ( "sqrt",       1,      (args) => Math.Sqrt(System.Convert.ToDouble(args[0])) ),
            ( "cbrt",       1,      (args) => Root(args[0], 3) ),
            ( "log",        1,      (args) => Math.Log(System.Convert.ToDouble(args[0])) ),
            ( "log10",      1,      (args) => Math.Log10(System.Convert.ToDouble(args[0])) ),
            ( "sin",        1,      (args) => Math.Sin(System.Convert.ToDouble(args[0])) ),
            ( "cos",        1,      (args) => Math.Cos(System.Convert.ToDouble(args[0])) ),
            ( "tan",        1,      (args) => Math.Tan(System.Convert.ToDouble(args[0])) ),
            ( "asin",       1,      (args) => Math.Asin(System.Convert.ToDouble(args[0])) ),
            ( "acos",       1,      (args) => Math.Acos(System.Convert.ToDouble(args[0])) ),
            ( "atan",       1,      (args) => Math.Atan(System.Convert.ToDouble(args[0])) ),
            ( "hsin",       1,      (args) => Math.Sinh(System.Convert.ToDouble(args[0])) ),
            ( "hcos",       1,      (args) => Math.Cosh(System.Convert.ToDouble(args[0])) ),
            ( "htan",       1,      (args) => Math.Tanh(System.Convert.ToDouble(args[0])) ),
            ( "min",        1, -1,  Min ),
            ( "max",        1, -1,  Max )
        };

        private static ExpressionParser s_ArithmeticExpressionParser = new ExpressionParser(s_ArithmeticUnaryOperators, s_ArithmeticBinaryOperators, s_ArithmeticVariables, s_ArithmeticFunctions, s_ArithmeticAbbreviationOperator, s_AssignmentOperator, s_EscapeSequenceFormatter);
        #endregion

        #region Bitwise
        private static Dictionary<string, object> m_BitwiseVariables = new Dictionary<string, object>
        {
            { "false", 0 },
            { "true", 1 }
        };

        private static ExtendedDictionary<string, Function> m_BitwiseFunctions = new ExtendedDictionary<string, Function>((value) => value.Identifier)
        {
        };

        private static ExtendedDictionary<char, UnaryOperator> m_BitwiseUnaryOperators = new ExtendedDictionary<char, UnaryOperator>((value) => value.Identifier)
        {
            ( '!', 1, AssociativityType.Right, (value) => System.Convert.ToInt32(value) != 0 ? 0 : 1 ),
            ( '~', 1, AssociativityType.Right, (value) => ~System.Convert.ToInt32(value) )
        };

        private static ExtendedDictionary<string, BinaryOperator> m_BitwiseBinaryOperators = new ExtendedDictionary<string, BinaryOperator>((value) => value.Identifier)
        {
            ( "+", 5, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) | System.Convert.ToInt32(rhs) ),
            ( "*", 3, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) & System.Convert.ToInt32(rhs) ),
            ( "^", 4, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) ^ System.Convert.ToInt32(rhs) ),

            ( "<<", 2, AssociativityType.Right, (lhs, rhs) => System.Convert.ToInt32(lhs) << System.Convert.ToInt32(rhs) ),
            ( ">>", 2, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) >> System.Convert.ToInt32(rhs) )
        };

        private static BinaryOperator s_BitwiseAbbreviationOperator = ("*", 3, AssociativityType.Right, (lhs, rhs) => System.Convert.ToInt32(lhs) & System.Convert.ToInt32(rhs));

        private static ExpressionParser s_BitwiseExpressionParser = new ExpressionParser(m_BitwiseUnaryOperators, m_BitwiseBinaryOperators, m_BitwiseVariables, m_BitwiseFunctions, s_BitwiseAbbreviationOperator, s_AssignmentOperator);
        #endregion

        [Fact] // 1 + 10^2^3
        public void Arithmetic_01()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"1 + 10^2^3");
            double cmp = 1 + (double)Exponentiation(10, Exponentiation(2, 3));

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 3 + --4
        public void Arithmetic_02()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"3 + --4");
            double cmp = 3 + -(-4);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 2math.pi
        public void Arithmetic_03()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"2math.pi");
            double cmp = 2 * Math.PI;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 3 + 4 * 2 / (1 - 5) ^ 2 ^ 3
        public void Arithmetic_04()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"3 + 4 * 2 / (1 - 5) ^ 2 ^ 3");
            double cmp = 3 + 4 * 2 / (double)Exponentiation((1 - 5), Exponentiation(2, 3));

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // sin(max(2, 3) / 3 * math.pi)
        public void Arithmetic_05()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"sin(max(2, 3) / 3 * math.pi)");
            double cmp = Math.Sin((double)Max(2, 3) / 3 * Math.PI);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 4 + 5 * (5 + 2)
        public void Arithmetic_06()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"4 + 5 * (5 + 2)");
            double cmp = 4 + 5 * (5 + 2);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla = (1 + 1) * 2")
        public void Arithmetic_07()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"bla = (1 + 1) * 2");
            double cmp = (1 + 1) * 2;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla * 5
        public void Arithmetic_08()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"bla * 5");
            double cmp = 2 * 5;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla2 = -(1 + 1) * 4 + -bla2
        public void Arithmetic_09()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"bla2 = -(1 + 1) * 4 + -bla2");
            double cmp = -(1 + 1) * 4 + -(-2);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // bla3 = (10^2^3) + bla3
        public void Arithmetic_10()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"bla3 = (10^2^3) + bla3");
            double cmp = (double)Exponentiation(10, Exponentiation(2, 3)) + (double)Exponentiation(10, Exponentiation(2, 3));

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 2^1da^-5
        public void Arithmetic_11()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"2^1da^-5");
            double cmp = (double)Exponentiation(2, Exponentiation((1 * 10), -5));

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // -(-5)
        public void Arithmetic_12()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"-(-5)");
            double cmp = -(-5);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // min(5, -5)
        public void Arithmetic_13()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"min(5, -5)");
            double cmp = (double)Min(5, -5);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 2 ** 3
        public void Arithmetic_14()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"2 ** 3");
            double cmp = (double)Exponentiation(2, 3);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 10 / 3
        public void Arithmetic_15()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"10 / 3");
            double cmp = 10.0 / 3.0;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 10 // 3
        public void Arithmetic_16()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"10 // 3");
            double cmp = Math.Truncate(10.0 / 3.0);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1da^3 * (10 - 2 * 2)
        public void Arithmetic_17()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"1da^3 * (10 - 2 * 2)");
            double cmp = (double)Exponentiation((1 * 10), 3) * (10 - 2 * 2);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (1 + 2) * 5 # comment
        public void Arithmetic_18()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"(1 + 2) * 5 # comment");
            double cmp = (1 + 2) * 5;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (phys.au / phys.c) / 60 # How long light travels from the Sun to Earth
        public void Arithmetic_19()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"(phys.au / phys.c) / 60 # How long light travels from the Sun to Earth");
            double cmp = (System.Convert.ToDouble(s_ArithmeticVariables["phys.au"]) / System.Convert.ToDouble(s_ArithmeticVariables["phys.c"])) / 60;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 384402000 / phys.c # How long light travels from the Moon to Earth
        public void Arithmetic_20()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"384402000 / phys.c # How long light travels from the Moon to Earth");
            double cmp = 384402000 / System.Convert.ToDouble(s_ArithmeticVariables["phys.c"]);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // -10^3
        public void Arithmetic_21()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"-10^3");
            double cmp = (double)Exponentiation(-10, 3);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // -(10^3)
        public void Arithmetic_22()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"-10^3");
            double cmp = -(double)Exponentiation(10, 3);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 + -(10^3)
        public void Arithmetic_23()
        {
            var res = s_ArithmeticExpressionParser.Evaluate(@"1 + -(10^3)");
            double cmp = 1 + -(double)Exponentiation(10, 3);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 + 0 * 1
        public void Bitwise_01()
        {
            var res = s_BitwiseExpressionParser.Evaluate(@"1 + 0 * 1");
            int cmp = 1 | 0 & 1;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 ^ (1 * 1) * 0
        public void Bitwise_02()
        {
            var res = s_BitwiseExpressionParser.Evaluate(@"1 ^ (1 * 1) * 0");
            int cmp = 1 ^ (1 & 1) & 0;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // !(0 + 0) ^ !(1 * 1)
        public void Bitwise_03()
        {
            var res = s_BitwiseExpressionParser.Evaluate(@"!(0 + 0) ^ !(1 * 1)");
            int cmp = ~(0 | 0) ^ ~(1 & 1);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 << 10
        public void Bitwise_04()
        {
            var res = s_BitwiseExpressionParser.Evaluate(@"1 << 10");
            int cmp = 1 << 10;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // (1 << 10) * 1023
        public void Bitwise_05()
        {
            var res = s_BitwiseExpressionParser.Evaluate(@"(1 << 10) * 1023");
            int cmp = (1 << 10) & 1023;

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }

        [Fact] // 1 ^ 1(1 + 0)
        public void Bitwise_06()
        {
            var res = s_BitwiseExpressionParser.Evaluate(@"1 ^ 1(1 + 0)");
            int cmp = 1 ^ 1 & (1 | 0);

            m_Output.WriteLine($@"""{ res }""");
            m_Output.WriteLine($@"""{ cmp }""");

            Assert.Equal(cmp, res);
        }
    }
}
