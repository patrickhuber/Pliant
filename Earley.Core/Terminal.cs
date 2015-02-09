using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class Terminal : Symbol
    {
        public Terminal(string value)
            : base(SymbolType.Terminal, value)
        { }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var terminal = obj as Terminal;
            if (terminal == null)
                return false;
            return base.Equals(obj);
        }
    }
}
