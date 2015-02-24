using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class TerminalBuilder : ITerminalBuilder
    {
        private IList<ITerminal> _terminals;
        public TerminalBuilder()
        {
            _terminals = new List<ITerminal>();
        }


        public IList<ITerminal> GetTerminals() 
        { 
            return _terminals; 
        }

        public ITerminalBuilder Range(char start, char end)
        {
            _terminals.Add(new DelegateTerminal(character=> start <= character && character <= end));
            return this;
        }

        public ITerminalBuilder Digit()
        {
            var digitTerminal = new DelegateTerminal(c => char.IsDigit(c));
            _terminals.Add(digitTerminal);
            return this;
        }

        public ITerminalBuilder WhiteSpace()
        {
            var whitespaceTerminal = new DelegateTerminal(c => char.IsWhiteSpace(c));
            _terminals.Add(whitespaceTerminal);
            return this;
        }
    }
}
