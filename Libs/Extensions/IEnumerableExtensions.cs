using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Libs.Extensions
{
    public static class IEnumerableExtensions
    {
        #region IEnumerable
        public static bool Any(this IEnumerable self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.GetEnumerator().MoveNext();
        }

        public static int IndexOf(this IEnumerable self, object item)
        {
            int index = 0;
            foreach(var element in self)
            {
                if(object.Equals(element, item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static void ForEach(this IEnumerable self, Action<object> action)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in self)
            {
                action(element);
            }
        }

        /*public static object RandomElement(this IEnumerable self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            System.Random random = new System.Random();
            return self.ElementAt(random.Next(self.Count()));
        }

        public static object RandomElement(this IEnumerable self, System.Random random)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.ElementAt(random.Next(self.Count()));
        }

        public static IEnumerable Randomize(this IEnumerable self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            System.Random random = new System.Random();
            return self.OrderBy(e => random.Next());
        }

        public static IEnumerable Randomize(this IEnumerable self, System.Random random)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.OrderBy(e => random.Next());
        }*/
        #endregion

        #region IEnumerable<T>
        public static T SingleOrDefault2<T>(this IEnumerable<T> self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            using(IEnumerator<T> e = self.GetEnumerator())
            {
                if(!e.MoveNext())
                    return default;

                var result = e.Current;
                return !e.MoveNext() ? result : default;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> self, T item)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            int index = 0;
            foreach(var element in self)
            {
                if(object.Equals(element, item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static T Find<T>(this IEnumerable<T> self, Predicate<T> predicate)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in self)
            {
                if(predicate(element))
                {
                    return element;
                }
            }

            return default;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> self, T item)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.Where(e => !e.Equals(item));
        }

        public static T MaxBy<T>(this IEnumerable<T> self) where T : IComparable<T>
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            using(IEnumerator<T> e = self.GetEnumerator())
            {
                if(!e.MoveNext())
                    throw new System.ArgumentException();

                var result = e.Current;
                while(e.MoveNext())
                {
                    if(e.Current.CompareTo(result) > 0)
                    {
                        result = e.Current;
                    }
                }

                return result;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in self)
            {
                action(element);
            }
        }

        public static T RandomElement<T>(this IEnumerable<T> self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            System.Random random = new System.Random();
            return self.ElementAt(random.Next(self.Count()));
        }

        public static T RandomElement<T>(this IEnumerable<T> self, System.Random random)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.ElementAt(random.Next(self.Count()));
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            System.Random random = new System.Random();
            return self.OrderBy(e => random.Next());
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> self, System.Random random)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.OrderBy(e => random.Next());
        }
        #endregion
    }
}
