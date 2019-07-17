using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Libs.Text
{
    [Serializable]
    public class SyntaxException : ExceptionBase
    {
        public int Position { get; set; } = -1;

        public override string AffixMessage
        {
            get { return Position >= 0 ? $@"Position: {Position}" : null; }
        }

        public override IEnumerable<string> GetAffixMessages()
        {
            yield return base.AffixMessage;
            yield return AffixMessage;
        }

        public SyntaxException() : base() { }

        public SyntaxException(string message) : base(message) { }

        public SyntaxException(string message, Exception innerException) : base(message, innerException) { }

        protected SyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
