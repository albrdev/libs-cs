using System;

namespace Libs.Text.Parsing
{
    public class UnaryOperator : Operator<char, UnaryOperator.Evaluator>
    {
        public delegate object Evaluator(object value);

        public static implicit operator UnaryOperator((char Identifier, int Precedence, AssociativityType Associativity, Evaluator Callback) value)
        {
            return new UnaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public static implicit operator UnaryOperator((char Identifier, int Precedence, int Associativity, Evaluator Callback) value)
        {
            return new UnaryOperator(value.Identifier, value.Precedence, value.Associativity, value.Callback);
        }

        public UnaryOperator(char identifier, int precedence, AssociativityType associativity, Evaluator callback) : base(identifier, precedence, associativity, callback) { }

        public UnaryOperator(char identifier, int precedence, int associativity, Evaluator callback) : base(identifier, precedence, associativity, callback) { }
    }
}
