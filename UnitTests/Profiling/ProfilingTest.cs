using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using UnitTests;

namespace ProfilingTests
{
    [MemoryDiagnoser]
    public class ProfilingTest
    {
        private static TextFormatterTestFixture TextFormatterTestFixture { get; set; } = new TextFormatterTestFixture();
        private static ExpressionParserTestFixture ExpressionParserTestFixture { get; set; } = new ExpressionParserTestFixture();

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ProfilingTest>();
        }

        [Benchmark(Baseline = true)]
        public void Run_01()
        {
            TextFormatterTestFixture.TextFormatter.Format(@"${""abcdefghijklmnopqrstuvwxyz"" + ""0123456789""}");
        }

        [Benchmark]
        public void Run_02()
        {
            ExpressionParserTestFixture.ArithmeticExpressionParser.Evaluate(@"""abcdefghijklmnopqrstuvwxyz"" + ""0123456789""");
        }
    }
}
