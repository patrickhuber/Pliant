using Pliant.Utilities;
using System.Text;
using System;
using Pliant.Diagnostics;

namespace Pliant.Grammars
{
    public class DottedRule : IComparable<IDottedRule>, IDottedRule
    {
        private readonly int _hashCode;
        
        public IProduction Production { get; private set; }

        public int Position { get; private set; }

        public ISymbol PreDotSymbol { get; private set; }

        public ISymbol PostDotSymbol { get; private set; }

        public bool IsComplete { get; private set; }

        public DottedRule(IProduction production, int position)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));

            Production = production;
            Position = position;
            _hashCode = ComputeHashCode(Production, Position);
            PostDotSymbol = GetPostDotSymbol(position, production);
            PreDotSymbol = GetPreDotSymbol(position, production);
            IsComplete = IsCompleted(position, production);
        }
        
        private static int ComputeHashCode(IProduction production, int position)
        {
            return HashCode.Compute(production.GetHashCode(), position.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is DottedRule preComputedState))
                return false;
            return preComputedState.Production.Equals(Production)
                && preComputedState.Position == Position;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .Append($"{Production.LeftHandSide.Value} ->");
            
            const string Dot = "\u25CF";                        
            const string Space = " ";

            for (var p = 0; p < Production.RightHandSide.Count; p++)
            {
                if (p == Position)
                    stringBuilder.Append(Dot);
                else
                    stringBuilder.Append(Space);
                
                stringBuilder.Append(Production.RightHandSide[p]);
            }

            if (Position == Production.RightHandSide.Count)
                stringBuilder.Append(Dot);

            return stringBuilder.ToString();
        }
        
        private static bool IsCompleted(int position, IProduction production)
        {
            return position == production.RightHandSide.Count;
        }

        public int CompareTo(IDottedRule other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        private static ISymbol GetPreDotSymbol(int position, IProduction production)
        {
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

        public static bool operator ==(DottedRule left, IDottedRule right)
        {
            return left is null 
                ? right is null 
                : left.Equals(right);
        }

        public static bool operator !=(DottedRule left, IDottedRule right)
        {
            return !(left == right);
        }

        public static bool operator <(DottedRule left, IDottedRule right)
        {
            return left is null 
                ? right is object
                : left.CompareTo(right) < 0;
        }

        public static bool operator <=(DottedRule left, IDottedRule right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(DottedRule left, IDottedRule right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(DottedRule left, IDottedRule right)
        {
            return left is null 
                ? right is null 
                : left.CompareTo(right) >= 0;
        }
    }
}
