using System;
using System.Linq.Expressions;

namespace Pliant.Builders.Expressions
{
    public class RuleExpression
    {
        public static implicit operator RuleExpression(ProductionExpression leftHandSide)
        {
            return new RuleExpression();
        }
    }
}