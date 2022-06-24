using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class DeterministicSet
    {
        private readonly UniqueList<DeterministicState> _states;
        private readonly Dictionary<ISymbol, DottedRuleSetTransition> _transitions;

        public IReadOnlyList<DeterministicState> States { get { return _states; } }

        public IReadOnlyDictionary<ISymbol, DottedRuleSetTransition> Transitions { get { return _transitions; } }
        
        public int Location { get; private set; }

        public DeterministicSet(int location)
        {
            _states = new UniqueList<DeterministicState>();
            _transitions = new Dictionary<ISymbol, DottedRuleSetTransition>();
            Location = location;
        }

        internal bool Enqueue(DeterministicState deterministicState)
        {
            var hasEnqueued = _states.AddUnique(deterministicState);
            return hasEnqueued;
        }

        public bool IsLeoUnique(ISymbol symbol)
        {
            return !Transitions.ContainsKey(symbol);
        }

        /// <summary>
        /// Transitions are a table of symbols to a set of states.
        /// 
        /// The state set is either a single leo item or one or more earley items
        /// </summary>
        /// <param name="searchSymbol">the key for the transition</param>
        /// <returns></returns>
        public DottedRuleSetTransition FindTransition(ISymbol searchSymbol)
        {
            return _transitions.TryGetValue(searchSymbol, out DottedRuleSetTransition transition) ? transition : null;
        }

        public void AddTransition(DottedRuleSetTransition cachedDottedRuleSetTransition)
        {
            _transitions.Add(cachedDottedRuleSetTransition.Symbol, cachedDottedRuleSetTransition);
        }
    }
}
