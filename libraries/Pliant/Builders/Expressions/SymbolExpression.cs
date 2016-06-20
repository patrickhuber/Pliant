using Pliant.Builders;
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
    }
}
