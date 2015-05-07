using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class LexerRule : ILexerRule
    {
        public IGrammar Grammar { get; private set; }
        public bool Greedy { get; private set; }
        public TokenType TokenType { get; private set; }

        public LexerRule(INonTerminal leftHandSide, IGrammar grammar, bool greedy = true)
        {
            Greedy = greedy;
            Grammar = grammar;
            TokenType = new TokenType(leftHandSide.Value);
        }

        public SymbolType SymbolType
        {
            get { return SymbolType.LexerRule; }
        }

    }
}
