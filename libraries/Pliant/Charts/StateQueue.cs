using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class StateQueue : ReadWriteList<IState>
    {
        private ISet<IState> _set;

        public StateQueue()
        {
            _set = new HashSet<IState>();
        }

        public bool Enqueue(IState state)
        {
            if (!_set.Add(state))
                return false;
            Add(state);
            return true;
        }
    }
}