using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

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

        public SyntaxException(int position) : base() { Position = position; }

        public SyntaxException(string message) : base(message) { }

        public SyntaxException(string message, int position) : base(message) { Position = position; }

        public SyntaxException(string message, Exception innerException) : base(message, innerException) { }

        public SyntaxException(string message, Exception innerException, int position) : base(message, innerException) { Position = position; }

        protected SyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(info == null)
                throw new ArgumentNullException();

            info.AddValue("Position", Position);
            base.GetObjectData(info, context);
        }
    }
}
