using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class DeterministicState
    {
        public DottedRuleSet DottedRuleSet { get; private set; }

        public int Origin { get; private set; }

        private readonly int _hashCode;
                
        public DeterministicState(DottedRuleSet dottedRuleSet, int origin)
        {
            DottedRuleSet = dottedRuleSet;
            Origin = origin;
            
            _hashCode = ComputeHashCode(DottedRuleSet, Origin);  
        }
        
        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            var deterministicState = obj as DeterministicState;
            if (((object)deterministicState) == null)
                return false;
            return deterministicState.Origin == Origin && DottedRuleSet.Equals(deterministicState.DottedRuleSet);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
        
        private static int ComputeHashCode(DottedRuleSet dottedRuleSet)
        {
            return dottedRuleSet.GetHashCode();
        }

        private static int ComputeHashCode(DottedRuleSet dottedRuleSet, int origin)
        {
            return HashCode.Compute(dottedRuleSet.GetHashCode(), origin.GetHashCode());
        }
    }
}
