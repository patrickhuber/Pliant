using Pliant.Grammars;
using Pliant.Regex;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant.Builders
{
    public class LexerRuleBuilder : ILexerRuleBuilder
    {
        private IList<ILexerRule> _lexerRules;

        public LexerRuleBuilder()
        {
            _lexerRules = new List<ILexerRule>();
        }
        
        public IList<ILexerRule> GetLexerRules()
        {
            return _lexerRules;
        }
        
        public ILexerRuleBuilder LexerRule(string name, ITerminal terminal)
        {
            var lexerRule = new TerminalLexerRule(terminal, new TokenType(name));
            _lexerRules.Add(lexerRule);
            return this;
        }

        public ILexerRuleBuilder LexerRule(ILexerRule lexerRule)
        {
            _lexerRules.Add(lexerRule);
            return this;
        }
    }
}
