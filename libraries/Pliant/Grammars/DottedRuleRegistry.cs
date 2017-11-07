using System;
using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleRegistry : IDottedRuleRegistry
    {
        private Dictionary<IProduction, Dictionary<int, IDottedRule>> _dottedRuleIndex;

        public DottedRuleRegistry()
        {
            _dottedRuleIndex = new Dictionary<IProduction, Dictionary<int, IDottedRule>>(
                new HashCodeEqualityComparer<IProduction>());
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
    }
}
