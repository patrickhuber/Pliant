using System.Collections.Generic;

namespace Pliant.Automata
{
    public class DfaState : IDfaState
    {
        public bool IsFinal { get; private set; }

        public List<IDfaTransition> Transitions { get; private set; }
        
        public DfaState()
            : this(false)
        { }

        public DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            Transitions = new List<IDfaTransition>();
        }

        public void AddTransition(IDfaTransition edge)
        {
            Transitions.Add(edge);
        }
    }
}