using System;
using BenchmarkDotNet.Attributes;

namespace PerformanceTests
{
    [MemoryDiagnoser]
    public class Test
    {
        //private static TextFormatterTestFixture TextFormatterTestFixture { get; set; } = new TextFormatterTestFixture();
        //private static ExpressionParserTestFixture ExpressionParserTestFixture { get; set; } = new ExpressionParserTestFixture();

        [Benchmark(Baseline = true)]
        public void Run_01()
        {
            //TextFormatterTestFixture.TextFormatter.Format(@"${""abcdefghijklmnopqrstuvwxyz"" + ""0123456789""}");
        }

        [Benchmark]
        public void Run_02()
        {
            //ExpressionParserTestFixture.ArithmeticExpressionParser.Evaluate(@"""abcdefghijklmnopqrstuvwxyz"" + ""0123456789""");
        }
    }
}
