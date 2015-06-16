using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Grammars
{
    public interface IGrammarLexerRule : ILexerRule
    {
        IGrammar Grammar { get; }
    }
}
