using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class LeoReducer
    {
        private Chart _chart;
        private int _position;
        private IState _rule;

        public LeoReducer(Chart chart)
        {
            _chart = chart;
        }

        public void Update(int i, IState state)
        {
            var transitiveItem = _chart[i]
                .FirstOrDefault(x => 
                    x.StateType == StateType.Transitive
                    && (x as TransitionState).Recognized.Equals(state.Production.LeftHandSide));
            bool containsTransitiveItem = transitiveItem != null;
            if (containsTransitiveItem)
            {
                _rule = transitiveItem;
                _position = transitiveItem.Origin;
            }
            else
            {
                var derivedItem = GetDerivedItemIfExactlyOneExists(i, state);
                if (derivedItem != null)
                {
                    _rule = state;
                    _position = state.Origin;
                    Update(derivedItem.Origin, derivedItem);
                    var transitionState = new TransitionState(
                        state.Production.LeftHandSide,
                        _rule.Production,
                        _rule.Position,
                        _position);
                    _chart.EnqueueAt(i, transitionState);
                }
            }
        }

        private IState GetDerivedItemIfExactlyOneExists(int i, IState state)
        {
            int derivedItemCount = 0;
            IState derivedItem = null;
            foreach (var item in _chart[i])
            {
                bool isDerivedITem = !item.IsComplete()
                    && item.CurrentSymbol().Equals(state.Production.LeftHandSide);
                if (isDerivedITem)
                {
                    if (derivedItemCount > 1)
                    {
                        derivedItem = null;
                        break;
                    }
                    derivedItemCount++;
                    derivedItem = item;
                }
            }
            return derivedItem;
        }

        IState GetTopMostItem(int i, IState state)
        {
            return null;
        }
    }
}
