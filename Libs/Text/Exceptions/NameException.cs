using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Libs.Text
{
    [Serializable]
    public class NameException : SyntaxException
    {
        public string Name { get; set; } = null;

        public override string AffixMessage
        {
            get { return Name != null ? $@"Name: {Name}" : null; }
        }

        public override IEnumerable<string> GetAffixMessages()
        {
            yield return base.AffixMessage;
            yield return AffixMessage;
        }

        public NameException() : base() { }

        public NameException(string message) : base(message) { }

        public NameException(string message, Exception innerException) : base(message, innerException) { }

        protected NameException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
