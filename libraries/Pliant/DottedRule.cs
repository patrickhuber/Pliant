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
        
        public int Position { get; private set; }
        public INullable<ISymbol> PostDotSymbol { get; private set; }
        public INullable<ISymbol> PreDotSymbol { get; private set; }

        public DottedRule(IProduction production, int position)
        {
            Position = position;
            _production = production;
            PostDotSymbol = new NullablePostDotWrapper(this);
            PreDotSymbol = new NullablePreDotWrapper(this);
        }

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

        public bool HasMoreTransitions
        {
            get { return !IsComplete; }
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

        public override bool Equals(object obj)
        {
            var dottedRule = obj as DottedRule;
            if(dottedRule == null)
                return false;
            return _production.Equals(dottedRule._production)
                && Position == dottedRule.Position;
        }

        private class NullablePostDotWrapper : INullable<ISymbol>
        {
            private DottedRule _dottedRule;

            public NullablePostDotWrapper(DottedRule dottedRule)
            {
                _dottedRule = dottedRule;
            }

            public ISymbol Value
            {
                get 
                {
                    if (!HasValue)
                        return null;
                    var production = _dottedRule._production;
                    return production.RightHandSide[_dottedRule.Position];
                }
            }

            public bool HasValue
            {
                get { return _dottedRule.Position < _dottedRule._production.RightHandSide.Count;}
            }
        }

        private class NullablePreDotWrapper : INullable<ISymbol>
        {
            private DottedRule _dottedRule;
                        
            public NullablePreDotWrapper(DottedRule dottedRule)
            {
                _dottedRule = dottedRule;
            }

            public ISymbol Value
            {
                get 
                {
                    if (!HasValue)
                        return null;
                    var production = _dottedRule._production;
                    var position = _dottedRule.Position;
                    return production.RightHandSide[position - 1];
                }
            }

            public bool HasValue
            {
                get { return _dottedRule.Position > 0 && !_dottedRule._production.IsEmpty; }
            }
        }
    }
}