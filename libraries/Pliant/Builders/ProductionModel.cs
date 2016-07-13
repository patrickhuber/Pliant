using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders
{
    public class ProductionModel : SymbolModel
    {
        public List<AlterationModel> Alterations { get; private set; }

        public NonTerminalModel LeftHandSide { get; set; }

        public override SymbolModelType ModelType
        {
            get { return SymbolModelType.Production; }
        }

        public override ISymbol Symbol
        {
            get { return LeftHandSide.NonTerminal; }
        }

        public ProductionModel()
        {
            Alterations = new List<AlterationModel>();
        }

        public ProductionModel(NonTerminalModel leftHandSide)
            : this()
        {
            LeftHandSide = leftHandSide;
            Alterations = new List<AlterationModel>();
        }

        public ProductionModel(INonTerminal leftHandSide)
            : this(new NonTerminalModel(leftHandSide))
        {
        }

        public ProductionModel(string leftHandSide)
            : this(new NonTerminal(leftHandSide))
        {
        }

        public ProductionModel(FullyQualifiedName fullyQualifiedName)
            : this(new NonTerminalModel(fullyQualifiedName))
        {
        }

        public IEnumerable<IProduction> ToProductions()
        {
            if (Alterations == null || Alterations.Count == 0)
                yield return new Production(LeftHandSide.NonTerminal);

            foreach (var alteration in Alterations)
            {
                var symbols = new List<ISymbol>();
                for (var s = 0; s < alteration.Symbols.Count; s++)
                {
                    var symbolModel = alteration.Symbols[s];
                    symbols.Add(symbolModel.Symbol);
                    if (symbolModel.ModelType == SymbolModelType.Reference)
                    {
                        var productionReferenceModel = symbolModel as ProductionReferenceModel;
                        for (var p = 0; p < productionReferenceModel.Grammar.Productions.Count; p++)
                            yield return productionReferenceModel.Grammar.Productions[p];
                    }
                }
                yield return new Production(LeftHandSide.NonTerminal, symbols);
            }
        }

        public void AddWithAnd(SymbolModel model)
        {
            if (Alterations.Count == 0)
                AddWithOr(model);
            else
                Alterations[Alterations.Count - 1].Symbols.Add(model);
        }

        public void AddWithOr(SymbolModel model)
        {
            var alterationModel = new AlterationModel();
            alterationModel.Symbols.Add(model);
            Alterations.Add(alterationModel);
        }

        public void Lambda()
        {
            Alterations.Add(new AlterationModel());
        }
    }
}
