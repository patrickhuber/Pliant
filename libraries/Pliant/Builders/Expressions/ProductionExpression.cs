using System.Collections.Generic;
using Pliant.Builders;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class ProductionExpression : BaseExpression
    {
        public ProductionModel ProductionModel { get; private set; }

        public ProductionExpression(INonTerminal leftHandSide)
        {
            ProductionModel = new ProductionModel(leftHandSide);
        }

        public ProductionExpression(FullyQualifiedName fullyQualifiedName)
        {
            ProductionModel = new ProductionModel(fullyQualifiedName);
        }

        public static implicit operator ProductionExpression(string leftHandSide)
        {
            return new ProductionExpression(new NonTerminal(leftHandSide));
        }

        public static implicit operator ProductionExpression(FullyQualifiedName fullyQualifiedName)
        {
            return new ProductionExpression(fullyQualifiedName);
        }
        
        public RuleExpression Rule
        {
            set
            {
                ProductionModel.Alterations.Clear();
                if (((object)value) == null)
                    return;
                foreach (var alteration in value.Alterations)
                    ProductionModel.Alterations.Add(
                        GetAlterationModelFromAlterationExpression(alteration));
            }
        }

        private static AlterationModel GetAlterationModelFromAlterationExpression(List<BaseExpression> symbols)
        {
            var alterationModel = new AlterationModel();
            foreach (var symbol in symbols)
            {
                if (symbol is ProductionExpression)
                {
                    var productionExpression = symbol as ProductionExpression;
                    alterationModel.Symbols.Add(
                        productionExpression.ProductionModel);
                }
                else if (symbol is SymbolExpression)
                {
                    var symbolExpression = symbol as SymbolExpression;
                    alterationModel.Symbols.Add(
                        symbolExpression.SymbolModel);
                }
                else if (symbol is ProductionReferenceExpression)
                {
                    var productionReferenceExpression = symbol as ProductionReferenceExpression;
                    alterationModel.Symbols.Add(
                        productionReferenceExpression.ProductionReferenceModel);
                }
                else if (symbol is Expr)
                {
                    foreach (var symbolModel in GetSymbolModelListFromExpr(symbol as Expr))
                        alterationModel.Symbols.Add(symbolModel);
                }
            }
            return alterationModel;
        }

        private static IEnumerable<SymbolModel> GetSymbolModelListFromExpr(Expr expr)
        {
            foreach (var alteration in expr.Alterations)
                foreach (var expression in alteration)
                {
                    if (expression is SymbolExpression)
                    {
                        var symbolExpression = expression as SymbolExpression;
                        yield return symbolExpression.SymbolModel;
                    }
                }
        }
    }
    
}
