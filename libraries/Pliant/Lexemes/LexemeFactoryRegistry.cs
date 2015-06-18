using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pliant.Grammars;

namespace Pliant.Lexemes
{
    public class LexemeFactoryRegistry : ILexemeFactoryRegistry
    {
        private IDictionary<LexerRuleType, ILexemeFactory> _registry;

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
