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

        public EarleySet(int location)
        {
            _predictions = new StateQueue();
            _scans = new StateQueue();
            _completions = new StateQueue();
            _transitions = new StateQueue();
            Location = location;
        }

        public IReadOnlyList<IState> Predictions { get { return _predictions; } }

        public IReadOnlyList<IState> Scans { get { return _scans; } }

        public IReadOnlyList<IState> Completions { get { return _completions; } }

        public IReadOnlyList<IState> Transitions { get { return _transitions; } }
        
        public bool Enqueue(IState state)
        {
            if (!state.DottedRule.IsComplete)
            {
                var currentSymbol = state.DottedRule.PostDotSymbol.Value;
                if (currentSymbol.SymbolType == SymbolType.NonTerminal)
                    return _predictions.Enqueue(state);
                else
                    return _scans.Enqueue(state);
            }
            if (state.StateType == StateType.Transitive)
                return _transitions.Enqueue(state);

            return _completions.Enqueue(state);  
        }
        
        public int Location { get; private set; }


        public ITransitionState FindTransitionState(ISymbol searchSymbol)
        {
            for (int t = 0; t < Transitions.Count; t++)
            {
                var transitionState = Transitions[t] as TransitionState;
                if (transitionState.Recognized.Equals(searchSymbol))
                    return transitionState;
            }
            return null;
        }

        public IState FindSourceState(ISymbol searchSymbol)
        {
            // TODO: speed up by using a index lookup
            var sourceItemCount = 0;
            IState sourceItem = null;
            for (int s = 0; s < Predictions.Count; s++)
            {
                var state = Predictions[s];
                if (state.IsSource(searchSymbol))
                {
                    bool moreThanOneSourceItemExists = sourceItemCount > 0;
                    if (moreThanOneSourceItemExists)
                        return null;
                    sourceItemCount++;
                    sourceItem = state;
                }
            }
            return sourceItem;
        }
    }
}
