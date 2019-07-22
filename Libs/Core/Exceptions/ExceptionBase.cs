using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Libs
{
    [Serializable]
    public abstract class ExceptionBase : System.Exception
    {
        public override string Message
        {
            get
            {
                string[] affixMessages = GetAffixMessages().Where(e => e != null).ToArray();
                return $@"{base.Message}{(affixMessages.Any() ? $@" ({string.Join(", ", affixMessages)})" : string.Empty)}";
            }
        }

        public virtual string AffixMessage { get { return null; } }

        public virtual IEnumerable<string> GetAffixMessages() { yield return null; }

        public ExceptionBase() : base() { }

        public ExceptionBase(string message) : base(message) { }

        public ExceptionBase(string message, Exception innerException) : base(message, innerException) { }

        protected ExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
