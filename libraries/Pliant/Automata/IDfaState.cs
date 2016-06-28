using System.Collections.Generic;

namespace Pliant.Automata
{
    public interface IDfaState
    {
        bool IsFinal { get; }

        List<IDfaTransition> Transitions { get; }

        void AddTransition(IDfaTransition edge);
    }
}