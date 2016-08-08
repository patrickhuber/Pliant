﻿using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Utilities;
using System;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public class SubsetConstructionAlgorithm : INfaToDfa
    {
        public IDfaState Transform(INfa nfa)
        {
            var processOnceQueue = new ProcessOnceQueue<NfaClosure>();

            var set = new HashSet<INfaState>();
            foreach(var state in nfa.Start.Closure())
                set.Add(state);

            var start = new NfaClosure(set, nfa.Start.Equals(nfa.End));
            processOnceQueue.Enqueue(start);

            while (processOnceQueue.Count > 0)
            {
                var nfaClosure = processOnceQueue.Dequeue();
                var transitions = SharedPools
                    .Default<Dictionary<ITerminal, HashSet<INfaState>>>()
                    .AllocateAndClear();

                foreach (var state in nfaClosure.Closure)
                {
                    for (var t = 0; t < state.Transitions.Count; t++)
                    {
                        var transition = state.Transitions[t];
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
                    nfaClosure.State.AddTransition(
                        new DfaTransition(terminal, closure.State));
                }

                SharedPools
                    .Default<Dictionary<ITerminal, HashSet<INfaState>>>()
                    .Free(transitions);
            }

            return start.State;
        }

        private static NfaClosure Closure(HashSet<INfaState> states, INfaState endState)
        {
            var set = new HashSet<INfaState>();
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

        private class NfaClosure
        {
            private readonly int _hashCode;
            public NfaClosure(HashSet<INfaState> closure, bool isFinal)
            {
                Closure = closure;
                _hashCode = HashCode.ComputeHash(closure);
                State = new DfaState(isFinal);
            }

            public HashSet<INfaState> Closure { get; private set; }

            public IDfaState State { get; }

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