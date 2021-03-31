using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class ProductionReferenceExpression : BaseExpression
    {
        public ProductionReferenceModel ProductionReferenceModel { get; private set; }

        public ProductionReferenceExpression(IGrammar grammar)
        {
            ProductionReferenceModel = new ProductionReferenceModel(grammar);
        }
    }
}
