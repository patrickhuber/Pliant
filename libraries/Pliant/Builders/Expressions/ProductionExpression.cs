using System;
using System.Collections.Generic;
using Pliant.Builders.Models;
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
                foreach (var alteration in value.Alterations)
                    ProductionModel.Alterations.Add(
                        GetAlterationModelFromAlterationExpression(alteration));
            }
        }

        private static AlterationModel GetAlterationModelFromAlterationExpression(IList<BaseExpression> symbols)
        {
            var alterationModel = new AlterationModel();
            foreach (var symbol in symbols)
            {
                if (symbol is ProductionExpression)
                {
                    var productionExpression = symbol as ProductionExpression;
                    alterationModel.Symbols.Add(
                        productionExpression.ProductionModel.LeftHandSide);
                }
                else if (symbol is SymbolExpression)
                {
                    var symbolExpression = symbol as SymbolExpression;
                    alterationModel.Symbols.Add(
                        symbolExpression.SymbolModel);
                }
            }
            return alterationModel;
        }
    }
    
}
