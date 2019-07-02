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
