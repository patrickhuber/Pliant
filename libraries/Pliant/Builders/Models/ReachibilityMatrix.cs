using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders.Models
{
    internal class ReachibilityMatrix 
    {
        private IDictionary<ISymbol, ISet<NonTerminalModel>> _matrix;
        private IDictionary<ISymbol, ProductionModel> _lookup;

        public ReachibilityMatrix(IEnumerable<ProductionModel> productions)
            : this()
        {
            AddProductions(productions);
        }

        public ReachibilityMatrix()
        {
            _matrix = new Dictionary<ISymbol, ISet<NonTerminalModel>>();
            _lookup = new Dictionary<ISymbol, ProductionModel>();
        }
                
        public void AddProductions(IEnumerable<ProductionModel> productions)
        {
            foreach (var production in productions)
            {
                AddProduction(production);
            }
        }

        public void AddProduction(ProductionModel production)
        {
            if (!_matrix.ContainsKey(production.LeftHandSide.Value))
                _matrix[production.LeftHandSide.Value] = new HashSet<NonTerminalModel>();

            if (!_lookup.ContainsKey(production.LeftHandSide.Value))
                _lookup[production.LeftHandSide.Value] = production;

            foreach (var alteration in production.Alterations)
            {
                foreach (var symbol in alteration.Symbols)
                {
                    if (symbol.ModelType != SymbolModelType.NonTerminal
                        || symbol.ModelType != SymbolModelType.Reference)
                        continue;
                    AddProductionToNewOrExistingSymbolSet(production, symbol);
                }
            }
        }

        private void AddProductionToNewOrExistingSymbolSet(ProductionModel production, SymbolModel symbol)
        {
            ISet<NonTerminalModel> set = null;
            if (!_matrix.TryGetValue(symbol.Symbol, out set))
            {
                set = new HashSet<NonTerminalModel>();
                _matrix[symbol.Symbol] = set;
            }

            set.Add(production.LeftHandSide);
        }

        public void RemoveProduction(ProductionModel productionModel)
        {
            if (!_matrix.ContainsKey(productionModel.LeftHandSide.Value))
                _matrix.Remove(productionModel.LeftHandSide.Value);
            if (!_lookup.ContainsKey(productionModel.LeftHandSide.Value))
                _lookup.Remove(productionModel.LeftHandSide.Value);
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
            return _matrix.ContainsKey(nonTerminalModel.Value);
        }
    }
}
