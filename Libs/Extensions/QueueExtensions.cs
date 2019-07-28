using System;
using System.Collections.Generic;

namespace Libs.Extensions
{
    public static class QueueExtensions
    {
        public static void EnqueueRange<T>(this Queue<T> self, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var item in items)
            {
                self.Enqueue(item);
            }
        }

        public static IEnumerable<T> DequeueRange<T>(this Queue<T> self, int count)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            if(count < self.Count)
                throw new System.ArgumentOutOfRangeException($@"count");

            for(int i = 0; i < count; i++)
            {
                yield return self.Dequeue();
            }
        }

        public static void Drop<T>(this Queue<T> self, int count, Action<T> action)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            if(count < self.Count)
                throw new System.ArgumentOutOfRangeException($@"count");

            for(int i = 0; i < count; i++)
            {
                action(self.Dequeue());
            }
        }

        public static T Next<T>(this Queue<T> self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            self.Dequeue();
            return self.Peek();
        }
    }
}
