using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public abstract class StateBase : IState
    {
        public IProduction Production { get; private set; }

        public int Origin { get; private set; }

        public ISymbol PreDotSymbol { get; private set; }

        public ISymbol PostDotSymbol { get; private set; }

        public int Position { get; private set; }

        public abstract StateType StateType { get; }

        public IForestNode ParseNode { get; set; }
        
        public bool IsComplete
        {
            get { return Position == Production.RightHandSide.Count; }
        }

        protected StateBase(IProduction production, int position, int origin)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            
            Production = production;
            Origin = origin;
            Position = position;
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
