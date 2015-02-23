using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class Terminal : Symbol, Earley.ITerminal
    {
        private char _character;

        public Terminal(char character)
            : base(SymbolType.Terminal)
        {
            _character = character;
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var terminal = obj as Terminal;
            if (terminal == null)
                return false;
            return terminal._character == _character;
        }

        public virtual bool IsMatch(char character)
        {
            return _character == character;
        }

        public override string ToString()
        {
            return _character.ToString();
        }
    }
}
