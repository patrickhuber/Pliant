using Pliant.Grammars;

namespace Pliant.Builders
{
    public class NonTerminalModel 
        : SymbolModel
    {
        public NonTerminalModel() { }

        public NonTerminalModel(INonTerminal value)
        {
            NonTerminal = value;
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

        public override ISymbol Symbol { get { return NonTerminal; } }

        public INonTerminal NonTerminal { get; set; }

        public override int GetHashCode()
        {
            return NonTerminal.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            var nonTerminalModel = obj as NonTerminalModel;
            if (null == nonTerminalModel)
                return false;
            return NonTerminal.Equals(nonTerminalModel.NonTerminal);
        }

        public override string ToString()
        {
            return NonTerminal.ToString();
        }
    }
}
