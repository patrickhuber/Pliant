using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public class LexemeFactoryRegistry : ILexemeFactoryRegistry
    {
        private readonly Dictionary<int, ILexemeFactory> _registry;        

        public LexemeFactoryRegistry()
        {
            _registry = new Dictionary<int, ILexemeFactory>();
        }

        public ILexemeFactory Get(LexerRuleType lexerRuleType)
        {
            if (!_registry.TryGetValue(lexerRuleType.GetHashCode(), out ILexemeFactory lexemeFactory))
                return null;
            return lexemeFactory;
        }

        public void Register(ILexemeFactory factory)
        {
            _registry.Add(factory.LexerRuleType.GetHashCode(), factory);
        }
    }
}