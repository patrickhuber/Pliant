using System;
using System.Collections.Generic;
using Pliant.Collections;

namespace Pliant.Grammars
{
    public class DottedRuleRegistry : IDottedRuleRegistry
    {
        private Dictionary<IProduction, Dictionary<int, IDottedRule>> _dottedRuleIndex;

        public DottedRuleRegistry()
        {
            _dottedRuleIndex = new Dictionary<IProduction, Dictionary<int, IDottedRule>>(
                new HashCodeEqualityComparer());
        }

        public void Register(IDottedRule dottedRule)
        {
            var positionIndex = _dottedRuleIndex.AddOrGetExisting(dottedRule.Production);
            positionIndex[dottedRule.Position] = dottedRule;
        }

        public IDottedRule Get(IProduction production, int position)
        {
            Dictionary<int, IDottedRule> positionIndex;
            if (!_dottedRuleIndex.TryGetValue(production, out positionIndex))
                return null;
            IDottedRule dottedRule;
            if (!positionIndex.TryGetValue(position, out dottedRule))
                return null;
            return dottedRule;
        }

        public IDottedRule GetNext(IDottedRule dottedRule)
        {
            return Get(dottedRule.Production, dottedRule.Position + 1);
        }

        private class HashCodeEqualityComparer : IEqualityComparer<IProduction>
        {
            public bool Equals(IProduction x, IProduction y)
            {
                return x.GetHashCode().Equals(y.GetHashCode());
            }

            public int GetHashCode(IProduction obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
