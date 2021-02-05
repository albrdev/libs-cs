using System;
using System.Collections.Generic;
using System.Linq;

namespace Libs.Text.Formatting
{
    public class FunctionNode : IEvaluable
    {
        public delegate object EvaluationHandler(params object[] args);

        protected EvaluationHandler Callback { get; set; }
        protected List<IEvaluable> Arguments { get; set; } = new List<IEvaluable>();

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

        internal void Add(params IEvaluable[] arguments)
        {
            Add((IEnumerable<IEvaluable>)arguments);
        }

        internal void Add(IEnumerable<IEvaluable> arguments)
        {
            Arguments.AddRange(arguments);
        }

        internal void Add(IEvaluable argument)
        {
            Arguments.Add(argument);
        }

        internal FunctionNode(EvaluationHandler callback, params IEvaluable[] arguments) : this(callback, (IEnumerable<IEvaluable>)arguments) { }

        internal FunctionNode(EvaluationHandler callback, IEnumerable<IEvaluable> arguments) : this(callback)
        {
            Arguments = new List<IEvaluable>(arguments);
        }

        internal FunctionNode(EvaluationHandler callback)
        {
            Callback = callback;
        }
    }
}
