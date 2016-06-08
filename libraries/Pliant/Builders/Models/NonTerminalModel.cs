﻿using Pliant.Grammars;

namespace Pliant.Builders.Models
{
    public class NonTerminalModel 
        : SymbolModel
    {
        public NonTerminalModel() { }

        public NonTerminalModel(INonTerminal value)
        {
            Value = value;
        }

        public NonTerminalModel(string value)
            : this(new NonTerminal(value))
        {
        }

        public NonTerminalModel(FullyQualifiedName fullyQualifiedName)
            : this(new NonTerminal(fullyQualifiedName.Namespace, fullyQualifiedName.Name))
        {
        }

        public override SymbolModelType ModelType
        {
            get { return SymbolModelType.NonTerminal; }
        }

        public override ISymbol Symbol { get { return Value; } }

        public INonTerminal Value { get; set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            var nonTerminalModel = obj as NonTerminalModel;
            if (null == nonTerminalModel)
                return false;
            return Value.Equals(nonTerminalModel.Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
