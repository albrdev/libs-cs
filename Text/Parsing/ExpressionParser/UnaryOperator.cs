using System;

namespace Libs.Text.Parsing
{
    public class UnaryOperator : Operator<char, UnaryOperator.EvaluationHandler>
    {
        public delegate object EvaluationHandler(object value);

        public static implicit operator UnaryOperator((char Identifier, int Precedence, AssociativityType Associativity, EvaluationHandler Callback) value)
        {
            return new UnaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public static implicit operator UnaryOperator((char Identifier, int Precedence, int Associativity, EvaluationHandler Callback) value)
        {
            return new UnaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public UnaryOperator(char identifier, int precedence, AssociativityType associativity, EvaluationHandler callback) : base(identifier, precedence, associativity, callback) { }

        public UnaryOperator(char identifier, int precedence, int associativity, EvaluationHandler callback) : base(identifier, precedence, associativity, callback) { }
    }
}
