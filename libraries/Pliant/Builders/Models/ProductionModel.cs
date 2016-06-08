using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders.Models
{
    public class ProductionModel
    {
        public IList<AlterationModel> Alterations { get; private set; }

        public NonTerminalModel LeftHandSide { get; set; }

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
            foreach (var alteration in Alterations)
            {
                var symbols = new List<ISymbol>();
                foreach (var symbolModel in alteration.Symbols)
                    symbols.Add(symbolModel.Symbol);
                yield return new Production(LeftHandSide.Value, symbols);
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
