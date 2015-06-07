using Pliant.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
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
