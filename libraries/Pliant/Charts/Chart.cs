using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class Chart : IChart
    {
        private ReadWriteList<IEarleySet> _earleySets;

        public Chart()
        {
            _earleySets = new ReadWriteList<IEarleySet>();
        }

        public bool Enqueue(int index, IState state)
        {
            if (_earleySets.Count <= index)
                _earleySets.Add(new EarleySet(index));

            var earleySet = _earleySets[index];
            return earleySet.Enqueue(state);
        }

        public IReadOnlyList<IEarleySet> EarleySets { get { return _earleySets; } }

        public int Count
        {
            get { return EarleySets.Count; }
        }
    }
}