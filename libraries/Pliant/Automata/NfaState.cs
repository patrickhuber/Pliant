using System.Collections.Generic;
using Pliant.Collections;

namespace Pliant.Automata
{
    public class NfaState : INfaState
    {
        private ReadWriteList<INfaTransition> _transitions;

        public NfaState()
        {
            _transitions = new ReadWriteList<INfaTransition>();
        }

        public IReadOnlyList<INfaTransition> Transitions
        {
            get { return _transitions; }
        }

        public void AddTransistion(INfaTransition transition)
        {
            _transitions.Add(transition);
        }

        public IEnumerable<INfaState> Closure()
        {
            // the working queue used to process states 
            var queue = new ProcessOnceQueue<INfaState>();
            
            // initialize by adding the curren state (this)
            queue.Enqueue(this);

            // loop over items in the queue, adding newly discovered
            // items after null transitions
            while (queue.Count != 0)
            {
                var state = queue.Dequeue();
                for (var t = 0; t < state.Transitions.Count; t++)
                {
                    var transition = state.Transitions[t];
                    if (transition.TransitionType == NfaTransitionType.Null)
                        queue.Enqueue(transition.Target);
                }
            }

            return queue.Visited;
        }        
    }    
}