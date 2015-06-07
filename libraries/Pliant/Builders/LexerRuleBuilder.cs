using Pliant.Grammars;
using Pliant.Regex;
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
            var production = new Production(name, terminal);
            var grammar = new Grammar(
                new NonTerminal(name), 
                new[] { production }, 
                new ILexerRule[] { }, 
                new ILexerRule[] { });
            var lexerRule = new LexerRule(new NonTerminal(name), grammar);
            return LexerRule(lexerRule);
        }

        public ILexerRuleBuilder LexerRule(ILexerRule lexerRule)
        {
            _lexerRules.Add(lexerRule);
            return this;
        }

        public ILexerRuleBuilder LexerRule(string name, string regularExpression)
        {
            var regexParser = new RegexParser();
            var grammar = regexParser.Parse(regularExpression);
            var lexerRule = new LexerRule(new NonTerminal(name), grammar);
            _lexerRules.Add(lexerRule);
            return this;
        }
    }
}
