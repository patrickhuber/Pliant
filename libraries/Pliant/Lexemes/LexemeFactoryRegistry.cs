using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Lexemes
{
    public class LexemeFactoryRegistry : ILexemeFactoryRegistry
    {
        private readonly Dictionary<LexerRuleType, ILexemeFactory> _registry;
        private readonly List<ILexemeFactory> _smallNumberRegistry;
        private int _itemCount;
        private static readonly int Threshold = 4;

        public LexemeFactoryRegistry()
        {
            _registry = new Dictionary<LexerRuleType, ILexemeFactory>();
            _smallNumberRegistry = new List<ILexemeFactory>();
            _itemCount = 0;
        }

        public ILexemeFactory Get(LexerRuleType lexerRuleType)
        {
            ILexemeFactory lexemeFactory = null;
            if (_itemCount > Threshold)
            {
                if (!_registry.TryGetValue(lexerRuleType, out lexemeFactory))
                {
                    return null;
                }
            }
            else
            {
                for (int i = 0; i < _smallNumberRegistry.Count; i++)
                {
                    var item = _smallNumberRegistry[i];
                    if (item.LexerRuleType.Equals(lexerRuleType))
                        return item;
                }
            }
            return lexemeFactory;
        }

        public void Register(ILexemeFactory factory)
        {
            if (_itemCount > Threshold)
                _registry[factory.LexerRuleType] = factory;
            else
            {
                _smallNumberRegistry.Add(factory);
                _itemCount++;
            }
            if (_itemCount == Threshold)
                for (int i = 0; i < _smallNumberRegistry.Count; i++)
                {
                    var currentFactory = _smallNumberRegistry[i];
                    _registry.Add(currentFactory.LexerRuleType, currentFactory);
                }
        }
    }
}