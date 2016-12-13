using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders
{
    internal class ReachibilityMatrix 
    {
        private Dictionary<ISymbol, UniqueList<NonTerminalModel>> _matrix;
        private Dictionary<ISymbol, ProductionModel> _lookup;
        
        public ReachibilityMatrix()
        {
            _matrix = new Dictionary<ISymbol, UniqueList<NonTerminalModel>>();
            _lookup = new Dictionary<ISymbol, ProductionModel>();
        }
             
        public void AddProduction(ProductionModel production)
        {
            if (!_matrix.ContainsKey(production.LeftHandSide.NonTerminal))
                _matrix[production.LeftHandSide.NonTerminal] = new UniqueList<NonTerminalModel>();

            if (!_lookup.ContainsKey(production.LeftHandSide.NonTerminal))
                _lookup[production.LeftHandSide.NonTerminal] = production;

            foreach (var alteration in production.Alterations)
            {
                for(var s = 0; s< alteration.Symbols.Count; s++)
                {
                    var symbol = alteration.Symbols[s];
                    if (symbol.ModelType != SymbolModelType.Production
                        || symbol.ModelType != SymbolModelType.Reference)
                        continue;
                    AddProductionToNewOrExistingSymbolSet(production, symbol);
                }
            }
        }

        private void AddProductionToNewOrExistingSymbolSet(ProductionModel production, SymbolModel symbol)
        {
            var set = _matrix.AddOrGetExisting(symbol.Symbol);
            set.Add(production.LeftHandSide);
        }

        public void RemoveProduction(ProductionModel productionModel)
        {
            if (!_matrix.ContainsKey(productionModel.LeftHandSide.NonTerminal))
                _matrix.Remove(productionModel.LeftHandSide.NonTerminal);
            if (!_lookup.ContainsKey(productionModel.LeftHandSide.NonTerminal))
                _lookup.Remove(productionModel.LeftHandSide.NonTerminal);
        }

        public void ClearProductions()
        {
            _matrix.Clear();
            _lookup.Clear();
        }
        
        public ProductionModel GetStartProduction()
        {
            foreach (var leftHandSide in _matrix.Keys)
            {
                var symbolsReachableByLeftHandSide = _matrix[leftHandSide];
                if (symbolsReachableByLeftHandSide.Count == 0)
                    return _lookup[leftHandSide];
            }
            return null;
        }

        public bool ProudctionExistsForSymbol(NonTerminalModel nonTerminalModel)
        {
            return _matrix.ContainsKey(nonTerminalModel.NonTerminal);
        }
    }
}
