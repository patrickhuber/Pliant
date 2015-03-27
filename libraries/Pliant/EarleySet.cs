using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class EarleySet : IEarleySet
    {
        private StateQueue _predictions;
        private StateQueue _scans;
        private StateQueue _completions;
        private StateQueue _transitions;

        public EarleySet()
        {
            _predictions = new StateQueue();
            _scans = new StateQueue();
            _completions = new StateQueue();
            _transitions = new StateQueue();
        }

        public IReadOnlyList<IState> Predictions { get { return _predictions; } }

        public IReadOnlyList<IState> Scans { get { return _scans; } }

        public IReadOnlyList<IState> Completions { get { return _completions; } }

        public IReadOnlyList<IState> Transitions { get { return _transitions; } }
        
        public bool Enqueue(IState state)
        {
            if (!state.IsComplete())
            {
                var currentSymbol = state.CurrentSymbol();
                if (currentSymbol.SymbolType == SymbolType.NonTerminal)
                    return _predictions.Enqueue(state);
                else
                    return _scans.Enqueue(state);
            }
            if (state.StateType == StateType.Transitive)
                return _transitions.Enqueue(state);

            return _completions.Enqueue(state);  
        }
    }
}
