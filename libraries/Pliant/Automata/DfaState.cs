using System.Collections.Generic;

namespace Pliant.Automata
{
    public class DfaState : IDfaState
    {
        private IList<IDfaTransition> _transitions;
        
        public bool IsFinal { get; private set; }

        public IEnumerable<IDfaTransition> Transitions { get { return _transitions; } }

        public DfaState()
            : this(false)
        { }

        public DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            _transitions = new List<IDfaTransition>();
        }

        public void AddTransition(IDfaTransition edge)
        {
            _transitions.Add(edge);
        }
    }
}
