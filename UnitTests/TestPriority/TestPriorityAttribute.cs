using System;

namespace UnitTests.TestPriority
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityAttribute : System.Attribute
    {
        public int Priority { get; private set; } = 0;

        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
