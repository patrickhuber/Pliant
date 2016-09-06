using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

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
            return _frames.AddUnique(frame);
        }
    }
}
