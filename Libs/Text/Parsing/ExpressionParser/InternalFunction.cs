using System;

namespace Libs.Text.Parsing
{
    internal class InternalFunction : Function
    {
        internal int ArgumentCount { get; set; } = 0;

        internal bool HasValidArgumentCount
        {
            get { return (MinArgumentCount < 0 || ArgumentCount >= MinArgumentCount) && (MaxArgumentCount < 0 || ArgumentCount <= MaxArgumentCount); }
        }

        public override string ToString()
        {
            return $"{Identifier}({ArgumentCount})";
        }

        internal InternalFunction(Function function) : base(function.Identifier, function.MinArgumentCount, function.MaxArgumentCount, function.Callback) { }
    }
}
