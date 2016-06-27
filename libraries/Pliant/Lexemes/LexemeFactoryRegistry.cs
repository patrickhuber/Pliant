using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Lexemes
{
    public class LexemeFactoryRegistry : ILexemeFactoryRegistry
    {
        private readonly Dictionary<LexerRuleType, ILexemeFactory> _registry;

        public LexemeFactoryRegistry()
        {
            _registry = new Dictionary<LexerRuleType, ILexemeFactory>();
        }

        public ILexemeFactory Get(LexerRuleType lexerRuleType)
        {
            ILexemeFactory lexemeFactory = null;
            if (!_registry.TryGetValue(lexerRuleType, out lexemeFactory))
                return null;
            return lexemeFactory;
        }

        public void Register(ILexemeFactory factory)
        {
            _registry[factory.LexerRuleType] = factory;
        }
    }
}