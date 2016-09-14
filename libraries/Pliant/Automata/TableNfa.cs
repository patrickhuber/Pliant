using Pliant.Collections;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Automata
{
    public class TableNfa
    {
        private Dictionary<int, Dictionary<char, int>> _table;
        private HashSet<int> _finalStates;
        private Dictionary<int, UniqueList<int>> _nullTransitions;

        public TableNfa(int start)
        {
            Start = start;
            _table = new Dictionary<int, Dictionary<char, int>>();
            _finalStates = new HashSet<int>();
            _nullTransitions = new Dictionary<int, UniqueList<int>>();
        }

        public void AddTransition(int source, char character, int target)
        {
            var sourceTransitions = _table.AddOrGetExisting(source);
            sourceTransitions[character] = target;
        }

        public void AddNullTransition(int source, int target)
        {
            _nullTransitions
                .AddOrGetExisting(source)
                .Add(target);
        }

        public int Start { get; private set; }

        public void SetFinal(int state, bool isFinal)
        {
            if (isFinal)
                _finalStates.Add(state);
            else
                _finalStates.Remove(state);
        }

        public bool IsFinal(int state)
        {
            return _finalStates.Contains(state);
        }

        public TableDfa ToDfa()
        {
            var queuePool = SharedPools.Default<ProcessOnceQueue<Closure>>();
            var queue = queuePool.Allocate();
            queue.Clear();
            var start = new Closure(Start, _nullTransitions, _finalStates);

            queue.Enqueue(
                start);

            var tableDfa = new TableDfa(start.GetHashCode());

            while (queue.Count > 0)
            {
                var transitions = SharedPools
                       .Default<Dictionary<char, SortedSet<int>>>()
                       .AllocateAndClear();

                var nfaClosure = queue.Dequeue();
                var nfaClosureId = nfaClosure.GetHashCode();
                tableDfa.SetFinal(nfaClosureId, nfaClosure.IsFinal);

                for (int i = 0; i < nfaClosure.States.Length; i++)
                {
                    var state = nfaClosure.States[i];
                    Dictionary<char, int> characterTransitions = null;
                    if (!_table.TryGetValue(state, out characterTransitions))
                        continue;

                    foreach (var characterTransition in characterTransitions)
                    {
                        SortedSet<int> targets = null;
                        if (!transitions.TryGetValue(characterTransition.Key, out targets))
                        {
                            targets = SharedPools.Default<SortedSet<int>>().AllocateAndClear();
                            transitions.Add(characterTransition.Key, targets);
                        }

                        targets.Add(characterTransition.Value);
                    }
                }

                foreach (var targetSet in transitions)
                {
                    var closure = new Closure(targetSet.Value, _nullTransitions, _finalStates);
                    closure = queue.EnqueueOrGetExisting(closure);
                    var closureId = closure.GetHashCode();

                    tableDfa.AddTransition(nfaClosureId, targetSet.Key, closureId);
                    tableDfa.SetFinal(closureId, closure.IsFinal);

                    SharedPools.Default<SortedSet<int>>().ClearAndFree(targetSet.Value);
                }

                SharedPools
                       .Default<Dictionary<char, SortedSet<int>>>()
                       .ClearAndFree(transitions);
            }

            queuePool.Free(queue);
            return tableDfa;
        }

        private class Closure
        {
            private SortedSet<int> _set;

            private readonly int _hashCode;

            public int[] States { get; private set; }

            public bool IsFinal { get; private set; }

            public Closure(
                SortedSet<int> sources,
                Dictionary<int, UniqueList<int>> nullTransitions,
                HashSet<int> finalStates)
            {
                _set = sources;
                var queue = new ProcessOnceQueue<int>();
                foreach (var item in sources)
                    queue.Enqueue(item);
                CreateClosure(nullTransitions, finalStates, queue);
                _hashCode = ComputeHashCode(States);
            }

            public Closure(
                int source, 
                Dictionary<int, UniqueList<int>> nullTransitions,
                HashSet<int> finalStates)
            {
                _set = new SortedSet<int>();
                var queue = new ProcessOnceQueue<int>();
                queue.Enqueue(source);
                CreateClosure(nullTransitions, finalStates, queue);
                _hashCode = ComputeHashCode(States);
            }

            private void CreateClosure(Dictionary<int, UniqueList<int>> nullTransitions, HashSet<int> finalStates, ProcessOnceQueue<int> queue)
            {
                while (queue.Count > 0)
                {
                    var state = queue.Dequeue();
                    _set.Add(state);
                    if (finalStates.Contains(state))
                        IsFinal = true;

                    UniqueList<int> targetStates = null;
                    if (!nullTransitions.TryGetValue(state, out targetStates))
                        continue;

                    for (int i = 0; i < targetStates.Count; i++)
                        queue.Enqueue(targetStates[i]);
                }
                States = _set.ToArray();
            }

            private static int ComputeHashCode(int[] states)
            {
                var hashCode = 0;
                for (int i = 0; i < states.Length; i++)
                {
                    hashCode = HashCode.ComputeIncrementalHash(states[i].GetHashCode(), hashCode, i == 0);
                }
                return hashCode;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object obj)
            {
                if (((object)obj) == null)
                    return false;

                var closure = obj as Closure;
                if (((object)closure) == null)
                    return false;

                for (int i = 0; i < States.Length; i++)
                {
                    if (!closure._set.Contains(States[i]))
                        return false;
                }
                return true;
            }
        }
    }
}
