using System;

namespace Libs.Text.Formatting
{
    public abstract class BinaryOperatorNode : IEvaluable
    {
        internal IEvaluable LHS { get; set; }
        internal IEvaluable RHS { get; set; }

        public abstract object Evaluate();

        protected BinaryOperatorNode(IEvaluable lhs, IEvaluable rhs)
        {
            LHS = lhs;
            RHS = rhs;
        }

        protected BinaryOperatorNode() { }
    }
}
