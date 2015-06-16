using Pliant.Collections;
using System.Linq;

namespace Pliant.Charts
{
    public class StateQueue : ReadWriteList<IState>
    {
        public bool Enqueue(IState state)
        {
            if (this.Any(x => x.Equals(state) && x.StateType == state.StateType))
                return false;
            this.Add(state);
            return true;
        }
    }
}
