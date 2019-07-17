using System;
using System.Collections.Generic;

namespace Libs.Collections
{
    public class ExtendedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private Func<TValue, TKey> m_KeyLookupCallback;

        public IEnumerator<TValue> this[IEnumerable<TKey> keys]
        {
            get
            {
                foreach(var key in keys)
                {
                    yield return base[key];
                }
            }
        }

        public IEnumerator<TValue> TryGetValues(IEnumerable<TKey> keys)
        {
            foreach(var key in keys)
            {
                TValue value;
                if(TryGetValue(key, out value))
                {
                    yield return value;
                }
            }
        }

        public void Add(TValue value) => base.Add(m_KeyLookupCallback(value), value);

        public ExtendedDictionary(Func<TValue, TKey> keyLookupCallback) : base()
        {
            m_KeyLookupCallback = keyLookupCallback ?? throw new System.ArgumentNullException();
        }

        public ExtendedDictionary() : base() { }
    }
}
