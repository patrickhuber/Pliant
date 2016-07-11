using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class StateQueue : ReadWriteList<IState>
    {
        private readonly HashSet<IState> _set;

        public StateQueue()
        {
            _set = new HashSet<IState>();
        }

        public bool Enqueue(IState state)
        {
            const int SWITCH_SIZE = 20;
            if(Count > SWITCH_SIZE)
            {
                if (!_set.Add(state))
                    return false;
            }
            else
            {
                var duplicate = false;
                // search for duplicate
                for (var i = 0; i < Count; i++)
                {
                    if (duplicate = state.GetHashCode() == this[i].GetHashCode())
                        return false;
                }
            }

            Add(state);
            if (Count == SWITCH_SIZE)
                for (int i = 0; i < Count; i++)
                    _set.Add(this[i]);
            return true;
        }
    }
}