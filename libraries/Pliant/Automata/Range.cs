using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Automata
{
    public class Range
    {
        public char Min;
        public char Max;

        public Range(char min, char max)
        {
            Min = min;
            Max = max;
        }
    }
}
