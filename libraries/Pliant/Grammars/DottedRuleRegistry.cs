using System;
using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleRegistry : IDottedRuleRegistry
    {
        private Dictionary<int, Dictionary<int, IDottedRule>> _dottedRuleIndex;

        public DottedRuleRegistry()
        {
            _dottedRuleIndex = new Dictionary<int, Dictionary<int, IDottedRule>>();
        }

        public void Register(IDottedRule dottedRule)
        {
            var hashCode = dottedRule.Production.GetHashCode();
            if (!_dottedRuleIndex.TryGetValue(hashCode, out Dictionary<int, IDottedRule> positionIndex))
            {
                positionIndex = new Dictionary<int, IDottedRule>();
                _dottedRuleIndex[hashCode] = positionIndex;
            }            
            positionIndex[dottedRule.Position] = dottedRule;
        }

        public IDottedRule Get(IProduction production, int position)
        {
            var hashCode = production.GetHashCode();            
            if (!_dottedRuleIndex.TryGetValue(hashCode, out Dictionary<int, IDottedRule> positionIndex))
                return null;
            if (!positionIndex.TryGetValue(position, out IDottedRule dottedRule))
                return null;
            return dottedRule;
        }

        public IDottedRule GetNext(IDottedRule dottedRule)
        {
            return Get(dottedRule.Production, dottedRule.Position + 1);
        }        
    }
}
