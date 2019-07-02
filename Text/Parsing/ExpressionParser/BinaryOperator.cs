using System;

namespace Libs.Text.Parsing
{
    public class BinaryOperator : Operator<string, BinaryOperator.Evaluator>
    {
        public delegate object Evaluator(object lhs, object rhs);

        public static implicit operator BinaryOperator((string Identifier, int Precedence, AssociativityType Associativity, Evaluator Callback) value)
        {
            return new BinaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public static implicit operator BinaryOperator((string Identifier, int Precedence, int Associativity, Evaluator Callback) value)
        {
            return new BinaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public BinaryOperator(string identifier, int precedence, AssociativityType associativity, Evaluator callback) : base(identifier, precedence, associativity, callback) { }

        public BinaryOperator(string identifier, int precedence, int associativity, Evaluator callback) : base(identifier, precedence, associativity, callback) { }
    }
}
