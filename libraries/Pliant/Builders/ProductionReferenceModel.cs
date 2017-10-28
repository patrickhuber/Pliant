using Pliant.Grammars;

namespace Pliant.Builders
{
    public class ProductionReferenceModel : SymbolModel
    {
        public IGrammar Grammar { get; private set; }

        public INonTerminal Reference { get; private set; }

        public override SymbolModelType ModelType
        {
            get { return SymbolModelType.Reference; }
        }

        public override ISymbol Symbol
        {
            get { return Reference; }
        }        

        public ProductionReferenceModel(IGrammar grammar)
        {
            Grammar = grammar;
            Reference = grammar.Start;
        }
    }
}
