using Pliant.Grammars;
using System;
using System.Linq.Expressions;

namespace Pliant.Builders.Expressions
{
    public class ProductionExpression : BaseExpression
    {
        public INonTerminal LeftHandSide { get; private set; }

        public ProductionExpression(INonTerminal leftHandSide)
        {
            LeftHandSide = leftHandSide;
        }

        public static implicit operator ProductionExpression(string leftHandSide)
        {
            return new ProductionExpression(new NonTerminal(leftHandSide));
        }
        
        public RuleExpression Rule { get; set; }
    }
    
}
