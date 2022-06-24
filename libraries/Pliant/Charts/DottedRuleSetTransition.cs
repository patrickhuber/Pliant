using Pliant.Grammars;

namespace Pliant.Charts
{
    public class DottedRuleSetTransition
    {
        public ISymbol Symbol { get; private set; }
        public DottedRuleSet DottedRuleSet { get; private set; }
        public int Origin { get;  private set;}

        public DottedRuleSetTransition(ISymbol symbol, DottedRuleSet dottedRuleSet, int origin)
        {
            Symbol = symbol;
            DottedRuleSet = dottedRuleSet;
            Origin = origin;
        }
    }
}