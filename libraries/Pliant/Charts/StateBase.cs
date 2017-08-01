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
            Assert.IsNotNull(dottedRule, nameof(dottedRule));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            DottedRule = dottedRule;
            Origin = origin;
            PostDotSymbol = GetPostDotSymbol(DottedRule);
            PreDotSymbol = GetPreDotSymbol(DottedRule);
        }
        
        private static ISymbol GetPreDotSymbol(int position, IProduction production)
        {
            if (position == 0 || production.IsEmpty)
                return null;
            return production.RightHandSide[position - 1];
        }

        private static ISymbol GetPreDotSymbol(IDottedRule dottedRule)
        {
            var production = dottedRule.Production;
            var position = dottedRule.Position;
            if (position == 0 || production.IsEmpty)
                return null;
            return production.RightHandSide[position - 1];
        }

        private static ISymbol GetPostDotSymbol(int position, IProduction production)
        {
            var productionRighHandSide = production.RightHandSide;
            if (position >= productionRighHandSide.Count)
                return null;
            return productionRighHandSide[position];
        }

        private static ISymbol GetPostDotSymbol(IDottedRule dottedRule)
        {
            var rightHandSide = dottedRule.Production.RightHandSide;
            if (dottedRule.Position >= rightHandSide.Count)
                return null;
            return rightHandSide[dottedRule.Position];
        }
    }
}
