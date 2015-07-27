using System.Collections.Generic;

namespace Pliant.Automata
{
    public interface IDfaState
    {
        bool IsFinal { get; }
        IEnumerable<IDfaEdge> Edges { get; }

        void AddEdge(IDfaEdge edge);
    }
}