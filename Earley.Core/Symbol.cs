using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class Symbol : ISymbol
    {
        public SymbolType SymbolType { get; private set; }
        
        public string Value { get; private set; }
        
        public Symbol(SymbolType symbolType, string value)
        {
            SymbolType = symbolType;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            var symbol = obj as Symbol;
            if (symbol == null)
                return false;
            return Value == symbol.Value 
                && SymbolType == symbol.SymbolType;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ SymbolType.GetHashCode();
        }
    }
}
