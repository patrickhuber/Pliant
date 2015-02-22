using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class ProductionBuilder : Earley.IProductionBuilder
    {
        private IList<IProduction> _productions;

        public ProductionBuilder() 
        {
            _productions = new List<IProduction>();
        }
        
        public IProductionBuilder Production(string name, Action<IRuleBuilder> rules)
        {
            if (rules == null)
                _productions.Add(new Production(name));
            else
            {
                var ruleBuilder = new RuleBuilder();
                rules(ruleBuilder);
                foreach (var rule in ruleBuilder.GetRules())
                {
                    var production = new Production(name, rule.ToArray());
                    _productions.Add(production);
                }
            }
            return this;
        }

        public IList<IProduction> GetProductions()
        {
            return _productions;
        }
    }
}
