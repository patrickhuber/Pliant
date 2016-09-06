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
            IEarleySet earleySet = null;
            if (_earleySets.Count <= index)
            {
                earleySet = new EarleySet(index);
                _earleySets.Add(earleySet);
            }
            else
            {
                earleySet = _earleySets[index];
            }

            return earleySet.Enqueue(state);
        }

        public int Count
        {
            get { return EarleySets.Count; }
        }
    }
}