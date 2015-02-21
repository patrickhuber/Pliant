using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earley
{
    public class ProductionBuilder
    {
        private string _name;
        private IList<ISymbol> _symbols;
        
        public ProductionBuilder(string name)
        {
            _name = name;
            _symbols = new List<ISymbol>();
        }

        public ProductionBuilder Terminal(char value)
        {
            _symbols.Add(new Terminal(value.ToString()));
            return this;
        }

        public ProductionBuilder NonTerminal(string name)
        {
            _symbols.Add(new NonTerminal(name));
            return this;
        }

        public IProduction GetProduction()
        {
            return new Production(_name, _symbols.ToArray());
        }
    }
}
