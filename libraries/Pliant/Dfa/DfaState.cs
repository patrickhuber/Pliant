using System.Collections.Generic;

namespace Pliant.Dfa
{
    public class DfaState : IDfaState
    {
        private IList<IDfaEdge> _edges;
        
        public bool IsFinal { get; private set; }

        public IEnumerable<IDfaEdge> Edges { get { return _edges; } }

        public DfaState()
            : this(false)
        { }

        public DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            _edges = new List<IDfaEdge>();
        }

        public void AddEdge(IDfaEdge edge)
        {
            _edges.Add(edge);
        }
    }
}
