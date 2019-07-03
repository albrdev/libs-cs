using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;
using Xunit.Abstractions;
using Libs.Extensions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace UnitTests.TestPriority
{
    public class PriorityComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if(x == 0 && y == 0)
                return 0;

            if(x == 0)
                return 1;
            else if(x < 0)
                x = Math.Abs(int.MinValue - x);

            if(y == 0)
                return -1;
            else if(y < 0)
                y = Math.Abs(int.MinValue - y);

            return x.CompareTo(y);
        }
    }

    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<T> OrderTestCases<T>(IEnumerable<T> testCases) where T : ITestCase
        {
            var sortedTestCases = new SortedDictionary<int, List<T>>(new PriorityComparer());

            foreach(var testCase in testCases)
            {
                IAttributeInfo attribute = testCase.TestMethod.Method.GetCustomAttributes((typeof(TestPriorityAttribute))).FirstOrDefault();
                sortedTestCases.GetOrAdd(attribute != null ? attribute.GetNamedArgument<int>("Priority") : 0).Add(testCase);
            }

            foreach(var list in sortedTestCases.Values)
            {
                list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
                foreach(var testCase in list)
                {
                    yield return testCase;
                }
            }
        }
    }
}
