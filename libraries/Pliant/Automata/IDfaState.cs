using System.Collections.Generic;

namespace Pliant.Automata
{
    public interface IDfaState
    {
        bool IsFinal { get; }

        IReadOnlyList<IDfaTransition> Transitions { get; }

        void AddTransition(IDfaTransition edge);
    }
}