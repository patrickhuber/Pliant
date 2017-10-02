using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Charts
{
    interface IRelativeState
    {
        IDottedRule DottedRule { get; }
        int Offset { get; }        
    }
}
