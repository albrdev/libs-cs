using System;

namespace Libs.Text.Parsing
{
    public class Function : IIdentifiable<string>
    {
        public delegate object EvaluationHandler(params object[] args);

        public string Identifier { get; private set; } = null;
        internal EvaluationHandler Callback { get; private set; } = null;
        internal int MinArgumentCount { get; private set; } = -1;
        internal int MaxArgumentCount { get; private set; } = -1;

        public static implicit operator Function((string Identifier, int MinArgumentCount, int MaxArgumentCount, EvaluationHandler Callback) value)
        {
            return new Function(value.Identifier, value.MinArgumentCount, value.MaxArgumentCount, value.Callback);
        }

        public static implicit operator Function((string Identifier, int MinArgumentCount, EvaluationHandler Callback) value)
        {
            return new Function(value.Identifier, value.MinArgumentCount, value.Callback);
        }

        public static implicit operator Function((string Identifier, EvaluationHandler Callback) value)
        {
            return new Function(value.Identifier, value.Callback);
        }

        public override string ToString()
        {
            return Identifier;
        }

        public Function(string identifier, int minArgumentCount, int maxArgumentCount, EvaluationHandler callback)
        {
            Identifier = identifier;
            Callback = callback;
            MinArgumentCount = minArgumentCount;
            MaxArgumentCount = maxArgumentCount;
        }

        public Function(string identifier, int argumentCount, EvaluationHandler callback)
        {
            Identifier = identifier;
            Callback = callback;
            MaxArgumentCount = MinArgumentCount = argumentCount;
        }

        public Function(string identifier, EvaluationHandler callback)
        {
            Identifier = identifier;
            Callback = callback;
        }
    }
}
