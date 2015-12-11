using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Automata
{
    public interface INfaToDfa
    {
        IDfaState Transform(INfa nfa);
    }
}
