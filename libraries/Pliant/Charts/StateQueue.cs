using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class StateQueue : ReadWriteList<IState>
    {
        private readonly HashSet<IState> _set;

        private const int THRESHOLD = 20;

        public StateQueue()
        {
            _set = new HashSet<IState>();
        }

        public bool Enqueue(IState state)
        {
            if(Count > THRESHOLD)
            {
                if (!_set.Add(state))
                    return false;
            }
            else
            {
                // search for duplicate
                for (var i = 0; i < Count; i++)
                {
                    if (state.GetHashCode() == this[i].GetHashCode())
                        return false;
                }
            }

            Add(state);
            if (Count == THRESHOLD)
                for (int i = 0; i < Count; i++)
                    _set.Add(this[i]);
            return true;
        }
    }
}