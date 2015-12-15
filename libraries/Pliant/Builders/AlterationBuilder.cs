namespace Pliant.Builders
{
    public class AlterationBuilder : IAlterationBuilder
    {
        public RuleBuilder RuleBuilder { get; private set; }

        public AlterationBuilder(RuleBuilder ruleBuilder)
        {
            RuleBuilder = ruleBuilder;
        }

        public IAlterationBuilder Or(params SymbolBuilder[] rules)
        {
            var newAlterations = new BaseBuilderList();
            foreach (var rule in rules)
                newAlterations.Add(rule);
            RuleBuilder.Data.Add(newAlterations);
            return new AlterationBuilder(RuleBuilder);
        }
    }
}