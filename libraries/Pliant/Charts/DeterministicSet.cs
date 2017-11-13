using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class DeterministicSet
    {
        private readonly UniqueList<DeterministicState> _states;
        private readonly Dictionary<ISymbol, CachedDottedRuleSetTransition> _transitions;

        public IReadOnlyList<DeterministicState> States { get { return _states; } }

        public IReadOnlyDictionary<ISymbol, CachedDottedRuleSetTransition> CachedTransitions { get { return _transitions; } }
        
        public int Location { get; private set; }

        public DeterministicSet(int location)
        {
            _states = new UniqueList<DeterministicState>();
            _transitions = new Dictionary<ISymbol, CachedDottedRuleSetTransition>();
            Location = location;
        }

        internal bool Enqueue(DeterministicState frame)
        {
            var hasEnqueued = _states.AddUnique(frame);
            return hasEnqueued;
        }

        public bool IsLeoUnique(ISymbol symbol)
        {
            return !CachedTransitions.ContainsKey(symbol);
        }

        public CachedDottedRuleSetTransition FindCachedDottedRuleSetTransition(ISymbol searchSymbol)
        {
            CachedDottedRuleSetTransition transition = null;
            if (_transitions.TryGetValue(searchSymbol, out transition))
                return transition;
            return null;
        }

        public void AddCachedTransition(CachedDottedRuleSetTransition cachedDottedRuleSetTransition)
        {
            _transitions.Add(cachedDottedRuleSetTransition.Symbol, cachedDottedRuleSetTransition);
        }
    }
}
