using System;
using System.Collections.Generic;

namespace Libs.Extensions
{
    public static class LinkedListExtensions
    {
        public static void AddRangeFirst<T>(this LinkedList<T> self, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in items)
            {
                self.AddFirst(element);
            }
        }

        public static void AddRangeLast<T>(this LinkedList<T> self, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in items)
            {
                self.AddLast(element);
            }
        }

        public static void AddRangeBefore<T>(this LinkedList<T> self, LinkedListNode<T> node, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in items)
            {
                self.AddBefore(node, element);
            }
        }

        public static void AddRangeBefore2<T>(this LinkedList<T> self, LinkedListNode<T> node, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in items)
            {
                node = self.AddBefore(node, element);
            }
        }

        public static void AddRangeAfter<T>(this LinkedList<T> self, LinkedListNode<T> node, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in items)
            {
                self.AddAfter(node, element);
            }
        }

        public static void AddRangeAfter2<T>(this LinkedList<T> self, LinkedListNode<T> node, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in items)
            {
                node = self.AddAfter(node, element);
            }
        }

        public static void RemoveRange<T>(this LinkedList<T> self, IEnumerable<LinkedListNode<T>> nodes)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var node in nodes)
            {
                self.Remove(node);
            }
        }

        public static int RemoveRange<T>(this LinkedList<T> self, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            int count = 0;
            foreach(var item in items)
            {
                if(self.Remove(item))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
