using System;

namespace Libs.Text.Formatting
{
    public class ValueNode : IEvaluable
    {
        protected object Value { get; set; }

        public virtual object Evaluate()
        {
            return Value;
        }

        internal ValueNode(object value)
        {
            Value = value;
        }
    }
}
