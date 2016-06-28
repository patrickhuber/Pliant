using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public class DfaState : IDfaState
    {
        public bool IsFinal { get; private set; }

        private ReadWriteList<IDfaTransition> _transitions;

        public IReadOnlyList<IDfaTransition> Transitions { get { return _transitions; } }
        
        public DfaState()
            : this(false)
        {
        }

        public DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            _transitions = new ReadWriteList<IDfaTransition>();
        }

        public void AddTransition(IDfaTransition edge)
        {
            _transitions.Add(edge);
        }
    }
}