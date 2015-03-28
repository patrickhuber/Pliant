using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class DottedRule : IDottedRule
    {
        private IProduction _production;

        public DottedRule(IProduction production, int position)
        {
            Position = position;
            _production = production;
        }

        public int Position { get; private set; }

        public ISymbol Symbol
        {
            get 
            {
                if (IsComplete)
                    return null;
                return _production.RightHandSide[Position];
            }
        }

        public bool IsComplete
        {
            get { return Position == _production.RightHandSide.Count; }
        }

        public IDottedRule NextRule()
        {
            if (IsComplete)
                return null;
            return new DottedRule(_production, Position + 1);
        }

        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ _production.GetHashCode();
                hash = hash * 16777619 ^ Position.GetHashCode();
                return hash;
            }
        }
    }
}