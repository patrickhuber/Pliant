using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders.Models
{
    internal class ReachibilityMatrix 
    {
        private IDictionary<NonTerminalModel, ISet<NonTerminalModel>> _matrix;
        
        public ReachibilityMatrix(IEnumerable<ProductionModel> productions)
        {
            _matrix = new Dictionary<NonTerminalModel, ISet<NonTerminalModel>>();
            foreach (var production in productions)
            {
                if (!_matrix.ContainsKey(production.LeftHandSide))
                    _matrix[production.LeftHandSide] = new HashSet<NonTerminalModel>();

                foreach (var alteration in production.Alterations)
                {
                    foreach (var symbol in alteration.Symbols)
                    {
                        if (symbol.ModelType != SymbolModelType.NonTerminal)
                            continue;

                        ISet<NonTerminalModel> set = null;
                        var nonTerminalModel = symbol as NonTerminalModel;
                        if (!_matrix.TryGetValue(nonTerminalModel, out set))
                        {
                            set = new HashSet<NonTerminalModel>();
                            _matrix[nonTerminalModel] = set;
                        }

                        set.Add(production.LeftHandSide);
                    }
                }                
            }
        }

        public NonTerminalModel GetStartSymbol()
        {
            foreach (var leftHandSide in _matrix.Keys)
            {
                var symbolsReachableByLeftHandSide = _matrix[leftHandSide];
                if (symbolsReachableByLeftHandSide.Count == 0)
                    return leftHandSide;
            }
            return null;
        }

        public bool ProudctionExistsForSymbol(NonTerminalModel nonTerminalModel)
        {
            return _matrix.ContainsKey(nonTerminalModel);
        }
    }
}
