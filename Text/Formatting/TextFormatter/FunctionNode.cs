using System;
using System.Collections.Generic;
using System.Linq;

namespace Libs.Text.Formatting
{
    public class FunctionNode : Evaluable
    {
        public delegate object Evaluator(params object[] args);

        protected Evaluator Callback { get; set; }
        protected List<Evaluable> Arguments { get; set; } = new List<Evaluable>();

        protected IEnumerable<object> EvaluateArguments
        {
            get
            {
                foreach(var node in Arguments)
                {
                    yield return node.Evaluate();
                }
            }
        }

        public virtual object Evaluate()
        {
            return Callback(EvaluateArguments.ToArray());
        }

        internal void Add(params Evaluable[] arguments)
        {
            Add((IEnumerable<Evaluable>)arguments);
        }

        internal void Add(IEnumerable<Evaluable> arguments)
        {
            Arguments.AddRange(arguments);
        }

        internal void Add(Evaluable argument)
        {
            Arguments.Add(argument);
        }

        internal FunctionNode(Evaluator callback, params Evaluable[] arguments) : this(callback, (IEnumerable<Evaluable>)arguments) { }

        internal FunctionNode(Evaluator callback, IEnumerable<Evaluable> arguments) : this(callback)
        {
            Arguments = new List<Evaluable>(arguments);
        }

        internal FunctionNode(Evaluator callback)
        {
            Callback = callback;
        }
    }
}
