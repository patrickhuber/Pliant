using Pliant.Grammars;
using System.Collections.Generic;
using System;

namespace Pliant.Automata
{
    public class NfaState : INfaState
    {
        private IList<INfaTransition> _transitions;

        public NfaState()
        {
            _transitions = new List<INfaTransition>();
        }

        public IEnumerable<INfaTransition> Transitions
        {
            get { return _transitions; }
        }

        public void AddTransistion(INfaTransition transition)
        {
            _transitions.Add(transition);
        }

        public IDictionary<ITerminal, ISet<INfaState>> Closure()
        {
            // the working queue used to process states
            var queue = new Queue<INfaState>();

            // the hash set used to track visited states
            var set = new HashSet<INfaState>();

            // the dictionary used to index states by terminal
            var dictionary = new Dictionary<ITerminal, ISet<INfaState>>();

            // initialize by adding the curren state (this)
            set.Add(this);
            queue.Enqueue(this);

            // loop over items in the queue, adding newly discovered
            // items after null transitions
            while (queue.Count != 0)
            {
                var state = queue.Dequeue();
                foreach (var transition in state.Transitions)
                {
                    var target = transition.Target;
                    switch (transition.TransitionType)
                    {
                        case NfaTransitionType.Null:
                            if (set.Add(target))
                                queue.Enqueue(target);
                            break;

                        case NfaTransitionType.Terminal:
                            var terminalTransition = transition as TerminalNfaTransition;
                            var terminal = terminalTransition.Terminal;
                            ISet<INfaState> terminalStateSet = null;
                            if (dictionary.ContainsKey(terminal))
                                terminalStateSet = dictionary[terminal];
                            else
                            {
                                terminalStateSet = new HashSet<INfaState>();
                                dictionary[terminal] = terminalStateSet;
                            }
                            terminalStateSet.Add(target);
                            break;
                    }
                }
            }

            return dictionary;
        }
        
    }
    
}