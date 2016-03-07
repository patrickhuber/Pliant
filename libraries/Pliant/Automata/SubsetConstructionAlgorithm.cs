using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public class SubsetConstructionAlgorithm : INfaToDfa
    {
        public IDfaState Transform(INfa nfa)
        {
            var startState = nfa.Start;            
            var visited = new HashSet<int>();
            var queue = new Queue<IEnumerable<INfaState>>();

            var startClosure = startState.Closure();

            // for each terminal in the closure, enqueue the set of states
            // reachable by that terminal if they haven't been processed yet.
            foreach (var terminal in startClosure.Keys)
            {
                var set = startClosure[terminal];

                if (set.Count == 0)
                    continue;

                var key = GetClosureKey(set);

                if (visited.Add(key))
                    queue.Enqueue(set);
            }

            while (queue.Count != 0)
            {
                var set = queue.Dequeue();
                var closure = Closure(set);
                foreach (var terminal in closure.Keys)
                {
                    var terminalSet = closure[terminal];
                    if (terminalSet.Count == 0)
                        continue;

                    var key = GetClosureKey(terminalSet);

                    if (visited.Add(key))
                        queue.Enqueue(terminalSet);
                }
            }

            throw new NotImplementedException();
        }

        public IDictionary<ITerminal, ISet<INfaState>> Closure(IEnumerable<INfaState> states)
        {
            var dictionary = new Dictionary<ITerminal, ISet<INfaState>>();
            
            // compute the aggregate over each state's closure
            foreach (var state in states)
            {
                var closure = state.Closure();
                foreach (var terminal in closure.Keys)
                {
                    var terminalClosure = closure[terminal];
                    if (!dictionary.ContainsKey(terminal))
                        dictionary[terminal] = new HashSet<INfaState>(terminalClosure);
                    else
                        foreach (var terminalClosureState in terminalClosure)
                            dictionary[terminal].Add(terminalClosureState);
                }
            }

            return dictionary;
        }

        public int GetClosureKey(IEnumerable<INfaState> closure)
        {
            return HashUtil.ComputeHash(closure);
        }

        private class ClosureDfaState : IDfaState
        {
            public bool IsFinal { get; private set; }
            public IEnumerable<INfaState> Closure { get; private set; }

            private IList<IDfaTransition> _transitions;

            public IEnumerable<IDfaTransition> Transitions
            {
                get { return _transitions; }
            }

            public ClosureDfaState(IEnumerable<INfaState> closure, bool isFinal)
            {
                Closure = closure;
                IsFinal = isFinal;
                _transitions = new List<IDfaTransition>();
            }

            public void AddTransition(IDfaTransition edge)
            {
                _transitions.Add(edge);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}