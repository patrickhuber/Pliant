using System.Collections.Generic;

namespace Pliant.Automata
{
    public interface INfaState
    {
        IReadOnlyList<INfaTransition> Transitions { get; }

        void AddTransistion(INfaTransition transition);

        IEnumerable<INfaState> Closure();
    }
}