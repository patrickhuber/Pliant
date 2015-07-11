using Pliant.Grammars;
using System.Diagnostics;

namespace Pliant.Charts
{
    public class Optimizer
    {
        private Chart _chart;
        
        public IState Transition { get; private set; }
        
        public Optimizer(Chart chart)
        { 
            _chart = chart;
        }

        public void Optimize(ISymbol searchSymbol, int index)
        {
            var earleySet = _chart.EarleySets[index];
            
            var transitiveState = earleySet.FindTransitionState(searchSymbol);
            if (transitiveState != null)
            {
                Transition = transitiveState;
                return;
            }
            
            var sourceState = earleySet.FindSourceState(searchSymbol);
            if (sourceState == null)
                return;
            
            var sourceStateNext = sourceState.NextState();
            if (!sourceStateNext.IsComplete)
                return;
            
            Transition = sourceStateNext;
            
            Optimize(
                sourceState.Production.LeftHandSide,
                sourceState.Origin);
            
            if (Transition == null)
                return;

            // TODO:
            // 1. create the parse node from the source state
            // 2. take the transition item's parse node and add it as the child 
            //      to the source state's parse node
            var transitionItem = new TransitionState(
                searchSymbol,
                Transition,
                sourceState,
                index);

            if (_chart.Enqueue(index, transitionItem))
                Log("Transition", index, transitionItem);
        }

        private void Log(string operation, int origin, IState state)
        {
            Debug.Write(string.Format("{0}\t{1}", origin, state));
            Debug.WriteLine(string.Format("\t # {0}", operation));
        }
    }
}
