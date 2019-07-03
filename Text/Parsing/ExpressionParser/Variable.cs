using System;

namespace Libs.Text.Parsing
{
    public class Variable : IIdentifiable<string>
    {
        private object m_Value;

        public string Identifier { get; private set; }

        public virtual object Value
        {
            get { return m_Value; }
            internal set { m_Value = value; }
        }

        public static implicit operator Variable((string Identifier, object Value) value)
        {
            return new Variable(value.Identifier, value.Value);
        }

        public override string ToString()
        {
            return Identifier;
        }

        public Variable(string identifier, object value)
        {
            Identifier = identifier;
            Value = value;
        }

        public Variable(string identifier) : this(identifier, null) { }
    }
}
