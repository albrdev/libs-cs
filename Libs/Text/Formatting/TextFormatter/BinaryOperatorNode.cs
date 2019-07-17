using System;

namespace Libs.Text.Formatting
{
    public abstract class BinaryOperatorNode : Evaluable
    {
        internal Evaluable LHS { get; set; }
        internal Evaluable RHS { get; set; }

        public abstract object Evaluate();

        protected BinaryOperatorNode(Evaluable lhs, Evaluable rhs)
        {
            LHS = lhs;
            RHS = rhs;
        }

        protected BinaryOperatorNode() { }
    }
}
