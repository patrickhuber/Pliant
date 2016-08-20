using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class Chart : IChart
    {
        private List<IEarleySet> _earleySets;

        public IReadOnlyList<IEarleySet> EarleySets { get { return _earleySets; } }

        public Chart()
        {
            _earleySets = new List<IEarleySet>();
        }

        public bool Enqueue(int index, IState state)
        {
            if (_earleySets.Count <= index)
                _earleySets.Add(new EarleySet(index));

            var earleySet = _earleySets[index];
            return earleySet.Enqueue(state);
        }

        public int Count
        {
            get { return EarleySets.Count; }
        }
    }
}