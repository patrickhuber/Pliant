using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Charts
{
    internal class PreComputedSet
    {
        private readonly UniqueList<StateFrame> _frames;

        public IReadOnlyList<StateFrame> Frames { get { return _frames; } }
        
        public int Location { get; private set; }

        public PreComputedSet(int location)
        {
            _frames = new UniqueList<StateFrame>();
            Location = location;
        }

        internal bool Enqueue(StateFrame frame)
        {
            var hasEnqueued = _frames.AddUnique(frame);
            return hasEnqueued;
        }
    }
}
