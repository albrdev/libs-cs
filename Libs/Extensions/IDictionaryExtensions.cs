using System;
using System.Collections.Generic;

namespace Libs.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key) where TValue : new()
        {
            if(self == null)
                throw new System.ArgumentNullException("self");

            if(!self.TryGetValue(key, out var value))
            {
                value = new TValue();
                self.Add(key, value);
            }

            return value;
        }
    }
}
