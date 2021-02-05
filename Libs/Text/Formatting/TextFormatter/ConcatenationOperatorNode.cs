using System;

namespace Libs.Text.Formatting
{
    public class ConcatenationOperatorNode : BinaryOperatorNode
    {
        public override object Evaluate()
        {
            return $"{LHS.Evaluate()}{RHS.Evaluate()}";
        }

        internal ConcatenationOperatorNode(IEvaluable lhs, IEvaluable rhs) : base(lhs, rhs) { }

        internal ConcatenationOperatorNode() { }
    }
}
