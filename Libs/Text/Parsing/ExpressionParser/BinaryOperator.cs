using System;

namespace Libs.Text.Parsing
{
    public class BinaryOperator : Operator<string, BinaryOperator.EvaluationHandler>
    {
        public delegate object EvaluationHandler(object lhs, object rhs);

        public static implicit operator BinaryOperator((string Identifier, int Precedence, AssociativityType Associativity, EvaluationHandler Callback) value)
        {
            return new BinaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public static implicit operator BinaryOperator((string Identifier, int Precedence, int Associativity, EvaluationHandler Callback) value)
        {
            return new BinaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public BinaryOperator(string identifier, int precedence, AssociativityType associativity, EvaluationHandler callback) : base(identifier, precedence, associativity, callback) { }

        public BinaryOperator(string identifier, int precedence, int associativity, EvaluationHandler callback) : base(identifier, precedence, associativity, callback) { }
    }
}
