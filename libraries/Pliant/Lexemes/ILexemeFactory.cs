using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Lexemes
{
    public interface ILexemeFactory
    {
        LexerRuleType LexerRuleType { get; }
        ILexeme Create(ILexerRule lexerRule);
    }
}
