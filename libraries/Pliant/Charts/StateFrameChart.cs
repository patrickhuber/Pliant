using System;
using Pliant.Grammars;
using System.Collections.Generic;
using Pliant.Collections;

namespace Pliant.Charts
{
    public class StateFrameChart
    {
        private List<StateFrameSet> _preComputedSets;

        public IReadOnlyList<StateFrameSet> FrameSets { get { return _preComputedSets; } }


        public StateFrameChart()
        {
            _preComputedSets = new List<StateFrameSet>();
        }

        public bool Enqueue(int index, StateFrame frame)
        {
            StateFrameSet preComputedSet = null;
            if (_preComputedSets.Count <= index)
            {
                preComputedSet = new StateFrameSet(index);
                _preComputedSets.Add(preComputedSet);
            }
            else
            {
                preComputedSet = _preComputedSets[index];
            }

            return preComputedSet.Enqueue(frame);
        }

        public void Clear()
        {
            _preComputedSets.Clear();
        }
    }
}
