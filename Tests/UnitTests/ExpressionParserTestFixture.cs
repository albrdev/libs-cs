using System;
using System.Collections.Generic;
using System.Linq;
using Libs.Collections;
using Libs.Text.Parsing;
using static Libs.Text.Parsing.Operator;
using Libs.Text.Formatting;

namespace UnitTests
{
    public class ExpressionParserTestFixture : IDisposable
    {
        public BinaryOperator AssignmentOperator { get; private set; }
        public EscapeSequenceFormatter EscapeSequenceFormatter { get; private set; }
        public Dictionary<string, Variable> CustomVariables { get; private set; }
        public List<object> Results { get; private set; }

        #region Custom methods
        public object Ans(params object[] args)
        {
            if(!Results.Any())
                throw new System.IndexOutOfRangeException();

            if(!args.Any())
                return Results.Last();

            int index = System.Convert.ToInt32(args[0]);
            if(index < 0)
                index = Results.Count + index;

            if(index < 0)
                index = 0;
            else if(index >= Results.Count)
                index = Results.Count - 1;

            return Results[index];
        }

        public static object PI(params object[] args)
        {
            return Math.PI;
        }

        public static object StringLength(params object[] args)
        {
            return args.Length > 0 ? args[0].ToString().Length : 0;
        }

        public static object Min(params object[] args)
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

        public static object Max(params object[] args)
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

        public static object Exponentiation(params object[] args)
        {
            double a = System.Convert.ToDouble(args[0]);
            double b = System.Convert.ToDouble(args[1]);

            double result = Math.Pow(Math.Abs(a), b);
            return a < 0.0 && ((long)b & 1) == 1 ? -result : result;
        }

        public static object Root(params object[] args)
        {
            double a = System.Convert.ToDouble(args[0]);
            double b = 1.0 / System.Convert.ToDouble(args[1]);

            double result = Math.Pow(Math.Abs(a), b);
            return a < 0.0 && ((long)b & 1) == 1 ? -result : result;
        }
        #endregion

        #region Arithmetic
        public ExtendedDictionary<char, UnaryOperator> ArithmeticUnaryOperators { get; private set; }
        public ExtendedDictionary<string, BinaryOperator> ArithmeticBinaryOperators { get; private set; }
        public ExtendedDictionary<string, Variable> ArithmeticVariables { get; private set; }
        public ExtendedDictionary<string, Function> ArithmeticFunctions { get; private set; }
        public BinaryOperator ArithmeticShorthandOperator { get; private set; }

        public ExpressionParser ArithmeticExpressionParser { get; private set; }
        #endregion

        #region Bitwise
        public ExtendedDictionary<string, Variable> BitwiseVariables { get; private set; }
        public ExtendedDictionary<string, Function> BitwiseFunctions { get; private set; }
        public ExtendedDictionary<char, UnaryOperator> BitwiseUnaryOperators { get; private set; }
        public ExtendedDictionary<string, BinaryOperator> BitwiseBinaryOperators { get; private set; }
        public BinaryOperator BitwiseShorthandOperator { get; private set; }

        public ExpressionParser BitwiseExpressionParser { get; private set; }
        #endregion

        public ExpressionParserTestFixture()
        {
            AssignmentOperator = ("=", 1, AssociativityType.Right, (lhs, rhs) => (((Variable)lhs).Value = rhs));
            EscapeSequenceFormatter = new ExtendedNativeEscapeSequenceFormatter();
            CustomVariables = new Dictionary<string, Variable>();
            Results = new List<object>();

            ArithmeticUnaryOperators = new ExtendedDictionary<char, UnaryOperator>((value) => value.Identifier)
            {
                ( '+', 1, AssociativityType.Right,  (value) => +System.Convert.ToDouble(value) ),
                ( '-', 1, AssociativityType.Right,  (value) => -System.Convert.ToDouble(value) ),

                ( '!', 1, AssociativityType.Right,  (value) => { double tmp = System.Convert.ToDouble(value); return tmp == 0D || tmp == double.NaN; } )
            };

            ArithmeticBinaryOperators = new ExtendedDictionary<string, BinaryOperator>((value) => value.Identifier)
            {
                ( "+", 4, AssociativityType.Left,   (lhs, rhs) => lhs is string || rhs is string ? (object)$"{lhs}{rhs}" : (object)(System.Convert.ToDouble(lhs) + System.Convert.ToDouble(rhs)) ),
                ( "-", 4, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) - System.Convert.ToDouble(rhs) ),
                ( "*", 3, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) * System.Convert.ToDouble(rhs) ),
                ( "/", 3, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) / System.Convert.ToDouble(rhs) ),
                ( "%", 3, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) % System.Convert.ToDouble(rhs) ),
                ( "^", 2, AssociativityType.Right,  (lhs, rhs) => Exponentiation(lhs, rhs) ),

                ( "//", 3, AssociativityType.Right, (lhs, rhs) => Math.Truncate(System.Convert.ToDouble(lhs) / System.Convert.ToDouble(rhs)) ),
                ( "**", 2, AssociativityType.Right, (lhs, rhs) => Exponentiation(lhs, rhs) ),

                ( "==", 5, AssociativityType.Left,  (lhs, rhs) => System.Convert.ToDouble(lhs) == System.Convert.ToDouble(lhs) ),
                ( "!=", 5, AssociativityType.Left,  (lhs, rhs) => System.Convert.ToDouble(lhs) != System.Convert.ToDouble(lhs) ),
                ( "<", 5, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) < System.Convert.ToDouble(lhs) ),
                ( ">", 5, AssociativityType.Left,   (lhs, rhs) => System.Convert.ToDouble(lhs) > System.Convert.ToDouble(lhs) ),
                ( "<=", 5, AssociativityType.Left,  (lhs, rhs) => System.Convert.ToDouble(lhs) <= System.Convert.ToDouble(lhs) ),
                ( ">=", 5, AssociativityType.Left,  (lhs, rhs) => System.Convert.ToDouble(lhs) >= System.Convert.ToDouble(lhs) )
            };

            ArithmeticVariables = new ExtendedDictionary<string, Variable>((value) => value.Identifier)
            {
                ( "T",          1000000000000.0 ),
                ( "G",          1000000000.0 ),
                ( "M",          1000000.0 ),
                ( "k",          1000.0 ),
                ( "h",          100.0 ),
                ( "da",         10.0 ),
                ( "d",          0.1 ),
                ( "c",          0.01 ),
                ( "m",          0.001 ),
                ( "u",          0.000001 ),
                ( "n",          0.000000001 ),
                ( "p",          0.000000000001 ),

                ( "math.pi",    Math.PI ),
                ( "math.e",     Math.E ),
                ( "phys.c",     299792458 ),
                ( "phys.au",    149597870700 )
            };

            ArithmeticFunctions = new ExtendedDictionary<string, Function>((value) => value.Identifier)
            {
                ( "ans",        0, 1,   Ans ),
                ( "pi",         0,      PI ),
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
                ( "sinh",       1,      (args) => Math.Sinh(System.Convert.ToDouble(args[0])) ),
                ( "cosh",       1,      (args) => Math.Cosh(System.Convert.ToDouble(args[0])) ),
                ( "tanh",       1,      (args) => Math.Tanh(System.Convert.ToDouble(args[0])) ),
                ( "asinh",      1,      (args) => Math.Asinh(System.Convert.ToDouble(args[0])) ),
                ( "acosh",      1,      (args) => Math.Acosh(System.Convert.ToDouble(args[0])) ),
                ( "atanh",      1,      (args) => Math.Atanh(System.Convert.ToDouble(args[0])) ),
                ( "min",        1, -1,  Min ),
                ( "max",        1, -1,  Max ),
                ( "strlen",     1,      StringLength )
            };

            ArithmeticShorthandOperator = ("*", 3, AssociativityType.Right, (lhs, rhs) => System.Convert.ToDouble(lhs) * System.Convert.ToDouble(rhs));
            ArithmeticExpressionParser = new ExpressionParser(ArithmeticUnaryOperators, ArithmeticBinaryOperators, ArithmeticVariables, ArithmeticFunctions, CustomVariables, AssignmentOperator)
            {
                ShorthandOperator = ArithmeticShorthandOperator,
                EscapeSequenceFormatter = EscapeSequenceFormatter
            };

            BitwiseVariables = new ExtendedDictionary<string, Variable>((value) => value.Identifier)
            {
                ( "false", 0 ),
                ( "true", 1 )
            };

            BitwiseFunctions = new ExtendedDictionary<string, Function>((value) => value.Identifier)
            {
            };

            BitwiseUnaryOperators = new ExtendedDictionary<char, UnaryOperator>((value) => value.Identifier)
            {
                ( '!', 1, AssociativityType.Right, (value) => System.Convert.ToInt32(value) != 0 ? 0 : 1 ),
                ( '~', 1, AssociativityType.Right, (value) => ~System.Convert.ToInt32(value) )
            };

            BitwiseBinaryOperators = new ExtendedDictionary<string, BinaryOperator>((value) => value.Identifier)
            {
                ( "+", 5, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) | System.Convert.ToInt32(rhs) ),
                ( "*", 3, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) & System.Convert.ToInt32(rhs) ),
                ( "^", 4, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) ^ System.Convert.ToInt32(rhs) ),

                ( "<<", 2, AssociativityType.Right, (lhs, rhs) => System.Convert.ToInt32(lhs) << System.Convert.ToInt32(rhs) ),
                ( ">>", 2, AssociativityType.Left, (lhs, rhs) => System.Convert.ToInt32(lhs) >> System.Convert.ToInt32(rhs) )
            };

            BitwiseShorthandOperator = ("*", 3, AssociativityType.Right, (lhs, rhs) => System.Convert.ToInt32(lhs) & System.Convert.ToInt32(rhs));

            BitwiseExpressionParser = new ExpressionParser(BitwiseUnaryOperators, BitwiseBinaryOperators, BitwiseVariables, BitwiseFunctions, CustomVariables, AssignmentOperator)
            {
                ShorthandOperator = BitwiseShorthandOperator,
                EscapeSequenceFormatter = EscapeSequenceFormatter
            };
        }

        public void Dispose()
        {
        }
    }
}
