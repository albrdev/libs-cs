using System;

namespace Libs.Text.Parsing
{
    internal class InternalFunction : Function
    {
        internal int ArgumentCount { get; set; } = 0;

        internal bool AssertArgumentCount(int count)
        {
            return ((MinArgumentCount < 0 || count >= MinArgumentCount) && (MaxArgumentCount < 0 || count <= MaxArgumentCount)) || (MinArgumentCount == 0 && MaxArgumentCount == 0);
        }

        public override string ToString()
        {
            return $"{Identifier}({ArgumentCount})";
        }

        internal InternalFunction(Function function) : base(function.Identifier, function.MinArgumentCount, function.MaxArgumentCount, function.Callback) { }
    }
}
