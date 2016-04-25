using Pliant.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Forest
{
    public class HorspoolSet
    {
        private readonly IDictionary<int, IDictionary<IState, IState>> _states;

        public HorspoolSet()
        {
            _states = new Dictionary<int, IDictionary<IState,IState>>();
        }

        public bool Add(int index, IState state)
        {
            IDictionary<IState, IState> set = null;
            if(!_states.TryGetValue(index, out set))
            {
                set = new Dictionary<IState, IState>();
                _states.Add(index, set);
            }

            if (set.ContainsKey(state))
                return false;
            set.Add(state, state);
            return true;
        }

        public bool TryGetValue(int index, IState key, out IState state)
        {
            IDictionary<IState, IState> set = null;
            state = null;
            if (!_states.TryGetValue(index, out set))            
                return false;
            return set.TryGetValue(key, out state);
        }
    }
}
