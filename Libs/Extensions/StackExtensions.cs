using System;
using System.Collections.Generic;

namespace Libs.Extensions
{
    public static class StackExtensions
    {
        public static void PushRange<T>(this Stack<T> self, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var item in items)
            {
                self.Push(item);
            }
        }

        public static IEnumerable<T> PopRange<T>(this Stack<T> self, int count)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            if(count < self.Count)
                throw new System.ArgumentOutOfRangeException($@"count");

            for(int i = 0; i < count; i++)
            {
                yield return self.Pop();
            }
        }

        public static void Drop<T>(this Stack<T> self, int count, Action<T> action)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            if(count < self.Count)
                throw new System.ArgumentOutOfRangeException($@"count");

            for(int i = 0; i < count; i++)
            {
                action(self.Pop());
            }
        }

        public static T Next<T>(this Stack<T> self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            self.Pop();
            return self.Peek();
        }
    }
}
