using System;
using Pliant.Grammars;
using System.Collections.Generic;
using Pliant.Collections;

namespace Pliant.Charts
{
    internal class PreComputedChart
    {
        private List<PreComputedSet> _preComputedSets;

        internal IReadOnlyList<PreComputedSet> FrameSets { get { return _preComputedSets; } }


        public PreComputedChart()
        {
            _preComputedSets = new List<PreComputedSet>();
        }

        internal bool Enqueue(int index, StateFrame frame)
        {
            PreComputedSet preComputedSet = null;
            if (_preComputedSets.Count <= index)
            {
                preComputedSet = new PreComputedSet(index);
                _preComputedSets.Add(preComputedSet);
            }
            else
            {
                preComputedSet = _preComputedSets[index];
            }

            return preComputedSet.Enqueue(frame);
        }
    }
}
