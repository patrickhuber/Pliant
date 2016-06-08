using Pliant.Builders.Models;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class SymbolExpression : BaseExpression
    {
        public SymbolModel SymbolModel { get; private set; }

        public SymbolExpression(SymbolModel symbolModel)
        {
            SymbolModel = symbolModel;
        }

        public SymbolExpression(ILexerRule symbol)
            : this(new LexerRuleModel(symbol))
        {
        }


    }
}
