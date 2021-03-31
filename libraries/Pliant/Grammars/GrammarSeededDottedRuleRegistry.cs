namespace Pliant.Grammars
{
    public class GrammarSeededDottedRuleRegistry : DottedRuleRegistry
    {
        public GrammarSeededDottedRuleRegistry(IGrammar grammar)
        {
            for (var p = 0; p < grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                for(var s = 0; s <= production.RightHandSide.Count; s++)
                {
                    var dottedRule = new DottedRule(production, s);
                    Register(dottedRule);
                }
            }
        }
    }
}
