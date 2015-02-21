using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class GrammarBuilder
    {
        private IList<IProduction> _productions; 

        public GrammarBuilder()
        {
            _productions = new List<IProduction>();
        }

        public GrammarBuilder Production(string name, params Action<ProductionBuilder>[] builders)
        {
            if (builders != null)
            {
                foreach (var builder in builders)
                {
                    var productionBuilder = new ProductionBuilder(name);
                    if (builder != null)
                    {
                        builder(productionBuilder);
                    }
                    _productions.Add(productionBuilder.GetProduction());            
                }
            }
            else
            {
                _productions.Add(new Production(name));
            }
            return this;
        }
        
        public Grammar GetGrammar()
        {
            return new Grammar(_productions.ToArray());
        }
    }
}
