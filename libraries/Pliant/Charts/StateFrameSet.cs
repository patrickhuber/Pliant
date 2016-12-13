using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pliant.Charts
{
    public class StateFrameSet
    {
        private readonly UniqueList<StateFrame> _frames;
        private readonly Dictionary<ISymbol, CachedStateFrameTransition> _transitions;

        public IReadOnlyList<StateFrame> Frames { get { return _frames; } }

        public IReadOnlyDictionary<ISymbol, CachedStateFrameTransition> CachedTransitions { get { return _transitions; } }
        
        public int Location { get; private set; }

        public StateFrameSet(int location)
        {
            _frames = new UniqueList<StateFrame>();
            _transitions = new Dictionary<ISymbol, CachedStateFrameTransition>();
            Location = location;
        }

        internal bool Enqueue(StateFrame frame)
        {
            var hasEnqueued = _frames.AddUnique(frame);
            return hasEnqueued;
        }

        public bool IsLeoUnique(ISymbol symbol)
        {
            return !CachedTransitions.ContainsKey(symbol);
        }

        public CachedStateFrameTransition FindCachedStateFrameTransition(ISymbol searchSymbol)
        {
            CachedStateFrameTransition transition = null;
            if (_transitions.TryGetValue(searchSymbol, out transition))
                return transition;
            return null;
        }

        public void AddCachedTransition(CachedStateFrameTransition cachedStateFrameTransition)
        {
            _transitions.Add(cachedStateFrameTransition.Symbol, cachedStateFrameTransition);
        }
    }
}
