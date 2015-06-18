using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Lexemes
{
    public interface ILexemeFactoryRegistry
    {
        ILexemeFactory Get(LexerRuleType lexerRuleType);
        void Register(ILexemeFactory factory);
    }
}
