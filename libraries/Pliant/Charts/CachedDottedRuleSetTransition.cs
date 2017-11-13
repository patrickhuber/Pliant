using Pliant.Grammars;

namespace Pliant.Charts
{
    public class CachedDottedRuleSetTransition
    {
        public ISymbol Symbol { get; private set; }
        public DottedRuleSet DottedRuleSet { get; private set; }
        public int Origin { get;  private set;}

        public CachedDottedRuleSetTransition(ISymbol symbol, DottedRuleSet dottedRuleSet, int origin)
        {
            Symbol = symbol;
            DottedRuleSet = dottedRuleSet;
            Origin = origin;
        }
    }
}