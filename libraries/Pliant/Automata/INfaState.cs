using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public interface INfaState
    {
        IEnumerable<INfaTransition> Transitions { get; }

        void AddTransistion(INfaTransition transition);

        IDictionary<ITerminal, ISet<INfaState>> Closure();
    }
}