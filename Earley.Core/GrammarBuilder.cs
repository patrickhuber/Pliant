using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class GrammarBuilder : Earley.IGrammarBuilder
    {
        private IList<IProduction> _productions; 

        public GrammarBuilder()
        {
            _productions = new List<IProduction>();
        }

        public GrammarBuilder(Action<IProductionBuilder> productions)
        {
            var productionBuilder = new ProductionBuilder();
            productions(productionBuilder);
            _productions = productionBuilder.GetProductions();
        }
                
        public IGrammarBuilder Production(string name, Action<IRuleBuilder> rules=null)
        {
            var productionBuilder = new ProductionBuilder();
            productionBuilder.Production(name, rules);
            var productions = productionBuilder.GetProductions();
            foreach (var production in productions)
                _productions.Add(production);
            return this;
        }

        public IGrammarBuilder Lexeme()
        {
            return this;
        }

        public Grammar GetGrammar()
        {
            return new Grammar(_productions.ToArray());
        }
    }
}
