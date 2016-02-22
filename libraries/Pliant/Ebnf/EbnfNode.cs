using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Ebnf
{
    public abstract class EbnfNode
    {
        public abstract EbnfNodeType NodeType { get; }
    }
}
