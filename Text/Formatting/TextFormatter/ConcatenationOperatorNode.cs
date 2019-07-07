using System;

namespace Libs.Text.Formatting
{
    public class ConcatenationOperatorNode : BinaryOperatorNode
    {
        public override object Evaluate()
        {
            return $"{LHS.Evaluate()}{RHS.Evaluate()}";
        }

        internal ConcatenationOperatorNode(Evaluable lhs, Evaluable rhs) : base(lhs, rhs) { }

        internal ConcatenationOperatorNode() { }
    }
}
