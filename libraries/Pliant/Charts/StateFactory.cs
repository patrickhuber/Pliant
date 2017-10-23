using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class StateFactory : IStateFactory
    {
        public IDottedRuleRegistry DottedRuleRegistry { get; private set; }

        public StateFactory(IDottedRuleRegistry dottedRuleRegistry)
        {
            DottedRuleRegistry = dottedRuleRegistry;
        }

        public IState NextState(IState state, IForestNode parseNode = null)
        {
            if (state.DottedRule.IsComplete)
                return null;
            var dottedRule = DottedRuleRegistry.Get(
                state.DottedRule.Production, 
                state.DottedRule.Position + 1);
            return parseNode == null 
                ? new NormalState(dottedRule, state.Origin)
                : new NormalState(dottedRule, state.Origin, parseNode);
        }

        public IState NewState(IProduction production, int position, int origin)
        {
            var dottedRule = DottedRuleRegistry.Get(production, position);
            return NewState(dottedRule, origin);
        }

        public IState NewState(IDottedRule dottedRule, int origin, IForestNode forestNode = null)
        {
            return forestNode == null
                ? new NormalState(dottedRule, origin)
                : new NormalState(dottedRule, origin, forestNode);
        }
    }
}
