using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class StateQueue<TState> : ReadWriteList<TState>
        where TState : IState
    {
        private readonly HashSet<TState> _set;

        private const int THRESHOLD = 20;
        
        public StateQueue()
        {
            _set = new HashSet<TState>();
        }        

        public bool Enqueue(TState state)
        {           
            if(Count > THRESHOLD)
            {
                if (!_set.Add(state))
                    return false;
            }
            else
            {
                var hashCode = state.GetHashCode();
                // search for duplicate
                for (var i = 0; i < Count; i++)
                {
                    if (hashCode == this[i].GetHashCode())
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