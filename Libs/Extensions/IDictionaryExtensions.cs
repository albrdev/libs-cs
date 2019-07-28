using System;
using System.Collections.Generic;

namespace Libs.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key) where TValue : new()
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            if(!self.TryGetValue(key, out var value))
            {
                value = new TValue();
                self.Add(key, value);
            }

            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TValue> valueGenerator)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            if(!self.TryGetValue(key, out var value))
            {
                value = valueGenerator();
                self.Add(key, value);
            }

            return value;
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in items)
            {
                self.Add(element.Key, element.Value);
            }
        }

        public static int RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<TKey> keys)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            int count = 0;
            foreach(var element in keys)
            {
                if(self.Remove(element))
                {
                    count++;
                }
            }

            return count;
        }

        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> self, Action<KeyValuePair<TKey, TValue>> action)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            foreach(var element in self)
            {
                action(element);
            }
        }
    }
}
