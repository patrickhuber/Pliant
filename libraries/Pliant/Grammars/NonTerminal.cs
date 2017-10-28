using System;

namespace Pliant.Grammars
{
    public class NonTerminal : Symbol, INonTerminal
    {
        public FullyQualifiedName FullyQualifiedName { get; private set; }

        public string Value { get { return FullyQualifiedName.FullName; } }
        
        private readonly int _hashCode;

        public NonTerminal(string @namespace, string name)
            : this(new FullyQualifiedName(@namespace, name))
        {
        }

        public NonTerminal(string name)
            : this(string.Empty, name)
        {
        }
        
        public NonTerminal(FullyQualifiedName fullyQualifiedName)
            : base(SymbolType.NonTerminal)
        {
            FullyQualifiedName = fullyQualifiedName;

            // precompute to same time on property execution            
            _hashCode = ComputeHashCode(Value);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            
            var nonTerminal = obj as INonTerminal;
            if (nonTerminal == null)
                return false;

            return Value.Equals(nonTerminal.Value);
        }

        private static int ComputeHashCode(string value)
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}