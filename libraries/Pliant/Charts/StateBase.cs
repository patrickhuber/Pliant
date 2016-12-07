using System;
using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public abstract class StateBase : IState
    {
        public IDottedRule DottedRule { get; private set; }
        
        public int Origin { get; private set; }

        public ISymbol PreDotSymbol { get; private set; }

        public ISymbol PostDotSymbol { get; private set; }
        
        public abstract StateType StateType { get; }

        public IForestNode ParseNode { get; set; }
        
        public bool IsComplete
        {
            get { return DottedRule.Position == DottedRule.Production.RightHandSide.Count; }
        }

        protected StateBase(IDottedRule dottedRule, int origin)
        {
            DottedRule = dottedRule;
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            PostDotSymbol = GetPostDotSymbol(dottedRule.Position, dottedRule.Production);
            PreDotSymbol = GetPreDotSymbol(dottedRule.Position, dottedRule.Production);
        }

        protected StateBase(IProduction production, int position, int origin)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));

            DottedRule = new DottedRule(production, position);
            Origin = origin;
            PostDotSymbol = GetPostDotSymbol(position, production);
            PreDotSymbol = GetPreDotSymbol(position, production);
        }
        
        private static ISymbol GetPreDotSymbol(int position, IProduction production)
        {
            if (position == 0 || production.IsEmpty)
                return null;
            return production.RightHandSide[position - 1];
        }

        private static ISymbol GetPostDotSymbol(int position, IProduction production)
        {
            if (position >= production.RightHandSide.Count)
                return null;
            return production.RightHandSide[position];
        }
    }
}
