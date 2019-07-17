using System;
using BenchmarkDotNet.Running;

namespace PerformanceTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }
    }
}
