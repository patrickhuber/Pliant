using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class NonTerminal : Symbol, INonTerminal
    {
        public NonTerminal(string value)
            : base(SymbolType.NonTerminal)
        {
            Value = value;
        }
        
        public string Value { get; private set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var nonTerminal = obj as INonTerminal;
            if (nonTerminal == null)
                return false;
            return Value.Equals(nonTerminal.Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
