using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Chart
    {
        private ReadWriteList<IEarleySet> _earleySets;

        public Chart()
        {
            _earleySets = new ReadWriteList<IEarleySet>();
        }
        
        public bool Enqueue(int index, IState state)
        {
            if (_earleySets.Count <= index)
                _earleySets.Add(new EarleySet());
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
