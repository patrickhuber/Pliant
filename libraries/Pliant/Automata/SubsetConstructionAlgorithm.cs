using Pliant.Collections;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public class SubsetConstructionAlgorithm : INfaToDfa
    {
        public IDfaState Transform(INfa nfa)
        {
            var processOnceQueue = new ProcessOnceQueue<NfaClosure>();
            var start = new NfaClosure(new[] { nfa.Start }, nfa.Start.Equals(nfa.End));
            processOnceQueue.Enqueue(start);
            while (processOnceQueue.Count > 0)
            {
                var nfaClosure = processOnceQueue.Dequeue();
                var transitions = new Dictionary<ITerminal, ISet<INfaState>>();

                foreach (var state in nfaClosure.Closure)
                {
                    foreach (var transition in state.Transitions)
                    {
                        switch (transition.TransitionType)
                        {
                            case NfaTransitionType.Terminal:
                                var terminalTransition = transition as TerminalNfaTransition;
                                var terminal = terminalTransition.Terminal;
                                
                                if (!transitions.ContainsKey(terminalTransition.Terminal))                                
                                    transitions[terminal] = new HashSet<INfaState>();
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
                    nfaClosure.State.AddTransition(new DfaTransition(terminal, closure.State));
                }
            }

            return start.State;
        }

        private NfaClosure Closure(IEnumerable<INfaState> states, INfaState endState)
        {
            var set = new HashSet<INfaState>();
            bool isFinal = false;
            foreach (var state in states)
                foreach (var item in state.Closure())
                {
                    if (item.Equals(endState))
                        isFinal = true;
                    set.Add(item);
                }

            return new NfaClosure(set, isFinal);
        }

        private class NfaClosure
        {
            public IEnumerable<INfaState> Closure { get; }
            public IDfaState State { get; }

            private readonly int _hashCode;

            public NfaClosure(IEnumerable<INfaState> closure, bool isFinal)
            {
                Closure = closure;
                _hashCode = HashUtil.ComputeHash(closure);
                State = new DfaState(isFinal);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object obj)
            {
                if ((object)obj == null)
                    return false;
                var nfaClosure = obj as NfaClosure;
                if ((object)nfaClosure == null)
                    return false;
                return nfaClosure._hashCode.Equals(_hashCode);
            }
        }
    }     
}