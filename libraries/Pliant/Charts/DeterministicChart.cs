using System;
using Pliant.Grammars;
using System.Collections.Generic;
using Pliant.Collections;

namespace Pliant.Charts
{
    public class DeterministicChart
    {
        private List<DeterministicSet> _preComputedSets;

        public IReadOnlyList<DeterministicSet> Sets { get { return _preComputedSets; } }

        public DeterministicChart()
        {
            _preComputedSets = new List<DeterministicSet>();
        }

        public bool Enqueue(int index, DeterministicState state)
        {
            DeterministicSet preComputedSet = null;
            if (_preComputedSets.Count <= index)
            {
                preComputedSet = new DeterministicSet(index);
                _preComputedSets.Add(preComputedSet);
            }
            else
            {
                preComputedSet = _preComputedSets[index];
            }

            return preComputedSet.Enqueue(state);
        }

        public void Clear()
        {
            _preComputedSets.Clear();
        }
    }
}
