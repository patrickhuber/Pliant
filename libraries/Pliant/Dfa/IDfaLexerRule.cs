using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Dfa
{
    public interface IDfaLexerRule : ILexerRule
    {
        IDfaState Start { get; }
    }
}
