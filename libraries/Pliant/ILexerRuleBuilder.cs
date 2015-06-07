using Pliant.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface ILexerRuleBuilder
    {
        ILexerRuleBuilder LexerRule(string name, ITerminal terminal);
        ILexerRuleBuilder LexerRule(ILexerRule lexerRule);
    }
}
