using Pliant.Collections;
using System.Linq;

namespace Pliant.Charts
{
    public class StateQueue : ReadWriteList<IState>
    {
        public bool Enqueue(IState state)
        {
            // PERF: Avoid Linq Any due to lambda allocation
            foreach (var value in this)
                if (value.Equals(state) && value.StateType == state.StateType)
                    return false;
            Add(state);
            return true;
        }
    }
}
