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

        public LexerRule(IGrammar grammar, bool greedy = true)
        {
            Greedy = greedy;
            Grammar = grammar;
        }
    }
}
