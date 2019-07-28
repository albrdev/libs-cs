using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Libs.Extensions
{
    public static class IListExtensions
    {
        public static int AddDistinctRange<T>(this IList<T> self, IEnumerable<T> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            int count = 0;
            foreach(var item in items)
            {
                if(self.AddDistinct(item))
                {
                    count++;
                }
            }

            return count;
        }

        public static bool AddDistinct<T>(this IList<T> self, T item)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            if(!self.Contains(item))
            {
                self.Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int RemoveRange<T>(this IList<T> self, IEnumerable<T> items)
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
