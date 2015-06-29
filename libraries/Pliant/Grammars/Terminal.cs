using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Grammars
{
    public class Terminal : Symbol, ITerminal
    {
        public char Character { get; private set; }

        public Terminal(char character)
            : base(SymbolType.Terminal)
        {
            Character = character;
        }
        
        public override int GetHashCode()
        {
            return HashUtil.ComputeHash(
                Character.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var terminal = obj as Terminal;
            if (terminal == null)
                return false;
            return terminal.Character == Character;
        }

        public virtual bool IsMatch(char character)
        {
            return Character == character;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
