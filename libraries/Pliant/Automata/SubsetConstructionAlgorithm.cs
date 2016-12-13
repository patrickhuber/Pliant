using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Automata
{
    public class SubsetConstructionAlgorithm : INfaToDfa
    {
        public IDfaState Transform(INfa nfa)
        {
            var processOnceQueue = new ProcessOnceQueue<NfaClosure>();

            var set = SharedPools.Default<SortedSet<INfaState>>().AllocateAndClear();
            foreach (var state in nfa.Start.Closure())
                set.Add(state);

            var start = new NfaClosure(set, nfa.Start.Equals(nfa.End));

            processOnceQueue.Enqueue(start);

            while (processOnceQueue.Count > 0)
            {
                var nfaClosure = processOnceQueue.Dequeue();
                var transitions = SharedPools
                    .Default<Dictionary<ITerminal, SortedSet<INfaState>>>()
                    .AllocateAndClear();

                for (int i = 0; i < nfaClosure.Closure.Length; i++)
                {
                    var state = nfaClosure.Closure[i];
                    for (var t = 0; t < state.Transitions.Count; t++)
                    {
                        var transition = state.Transitions[t];
                        switch (transition.TransitionType)
                        {
                            case NfaTransitionType.Edge:
                                var terminalTransition = transition as TerminalNfaTransition;
                                var terminal = terminalTransition.Terminal;

                                if (!transitions.ContainsKey(terminalTransition.Terminal))
                                    transitions[terminal] = SharedPools.Default<SortedSet<INfaState>>().AllocateAndClear();
                                transitions[terminal].Add(transition.Target);
                                break;
                        }
                    }
                }
                
                foreach (var terminal in transitions.Keys)
                {
                    var targetStates = transitions[terminal];
                    var closure = Closure(targetStates, nfa.End);
                    closure = processOnceQueue.EnqueueOrGetExisting(closure);
                    nfaClosure.State.AddTransition(
                        new DfaTransition(terminal, closure.State));
                    SharedPools.Default<SortedSet<INfaState>>().ClearAndFree(targetStates);
                }
                SharedPools
                    .Default<SortedSet<INfaState>>()
                    .ClearAndFree(nfaClosure.Set);
                SharedPools
                    .Default<Dictionary<ITerminal, SortedSet<INfaState>>>()
                    .ClearAndFree(transitions);
            }

            return start.State;
        }

        private static NfaClosure Closure(SortedSet<INfaState> states, INfaState endState)
        {
            var set = SharedPools.Default<SortedSet<INfaState>>().AllocateAndClear();
            var isFinal = false;
            foreach (var state in states)
                foreach (var item in state.Closure())
                {
                    if (item.Equals(endState))
                        isFinal = true;
                    set.Add(item);
                }

            return new NfaClosure(set, isFinal);
        }

        private class NfaClosure : IComparable<NfaClosure>, IComparable
        {
            private readonly int _hashCode;
            public NfaClosure(SortedSet<INfaState> closure, bool isFinal)
            {
                Set = closure;
                _hashCode = HashCode.Compute(closure);
                Closure = closure.ToArray();
                State = new DfaState(isFinal);
            }

            public SortedSet<INfaState> Set { get; private set; }

            public INfaState[] Closure { get; private set; }

            public IDfaState State { get; }

            public int CompareTo(object obj)
            {
                if (obj == null)
                    throw new ArgumentNullException();
                var nfaClosure = obj as NfaClosure;
                if (nfaClosure == null)
                    throw new ArgumentException("instance of NfaClosure expected.", nameof(obj));
                return CompareTo(nfaClosure);
            }

            public int CompareTo(NfaClosure other)
            {
                return GetHashCode().CompareTo(other.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                var nfaClosure = obj as NfaClosure;
                if (nfaClosure == null)
                    return false;
                return nfaClosure._hashCode.Equals(_hashCode);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }        
    }     
}