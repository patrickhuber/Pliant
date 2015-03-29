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

        public ScanState(IState state, char character)
            : this(state.Production, 
            state.DottedRule.Position, 
            state.Origin, 
            character)
        { }

        public char Capture { get; private set; }
    }
}
