using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class ScanState : State
    {
        public ScanState(IProduction production, int position, int origin, char capture)
            : base(production, position, origin)
        {
            Capture = capture;
        }

        public char Capture { get; private set; }
    }
}
