using System;
namespace Earley
{
    public interface IProductionBuilder
    {
        IProductionBuilder Production(string name, Action<IRuleBuilder> ruleBuilder);
    }
}
