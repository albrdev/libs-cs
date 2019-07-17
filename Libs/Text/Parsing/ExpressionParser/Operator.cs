using System;

namespace Libs.Text.Parsing
{
    public abstract class Operator
    {
        [Flags]
        public enum AssociativityType
        {
            Left = 1,
            Right = 2
        }

        public int Precedence { get; private set; }
        public int Associativity { get; private set; }

        public Operator(int precedence, AssociativityType associativity) : this(precedence, (int)associativity) { }

        public Operator(int precedence, int associativity)
        {
            Precedence = precedence;
            Associativity = associativity;
        }
    }

    public abstract class Operator<T, U> : Operator, IIdentifiable<T> where U : System.Delegate
    {
        public T Identifier { get; private set; }
        internal U Callback { get; private set; }

        public override string ToString()
        {
            return Identifier.ToString();
        }

        public Operator(T identifier, int precedence, AssociativityType associativity, U callback) : this(identifier, precedence, (int)associativity, callback) { }

        public Operator(T identifier, int precedence, int associativity, U callback) : base(precedence, associativity)
        {
            Identifier = identifier;
            Callback = callback;
        }
    }
}
