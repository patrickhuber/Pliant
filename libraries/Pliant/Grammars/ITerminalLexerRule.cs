using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Grammars
{
    public interface ITerminalLexerRule : ILexerRule
    {
        ITerminal Terminal { get; }
    }
}
