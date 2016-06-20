using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Pliant.Builders.Models
{
    public class GrammarModel
    {
        private ObservableCollection<ProductionModel> _productions;

        public ICollection<ProductionModel> Productions { get { return _productions; } }

        public ICollection<LexerRuleModel> IgnoreRules { get; private set; }

        public ProductionModel Start { get; set; }

        private ReachibilityMatrix _reachibilityMatrix;

        public GrammarModel()
        {
            _productions = new ObservableCollection<ProductionModel>();
            _productions.CollectionChanged += CollectionChanged;
            _reachibilityMatrix = new ReachibilityMatrix();
            IgnoreRules = new List<LexerRuleModel>();
        }

        public GrammarModel(ProductionModel start)
            : this()
        {
            Start = start;
        }
        
        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(object item in e.NewItems)
                        if(item is ProductionModel)
                            OnAddProduction(item as ProductionModel);
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.NewItems)
                        if (item is ProductionModel)
                            OnRemoveProduction(item as ProductionModel);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnResetProductions();
                    break;
            }
        }

        private void OnRemoveProduction(ProductionModel productionModel)
        {
            _reachibilityMatrix.RemoveProduction(productionModel);
        }

        private void OnAddProduction(ProductionModel productionModel)
        {
            _reachibilityMatrix.AddProduction(productionModel);
        }

        public void OnResetProductions()
        {
            Start = null;
            _reachibilityMatrix.ClearProductions();
        }

        public IGrammar ToGrammar()
        {
            if (StartSymbolExists())
            {
                if (ProductionsAreEmpty())
                    PopulateMissingProductionsFromStart(Start);
                AssertStartProductionExistsForStartSymbol(_reachibilityMatrix);
            }
            else
                Start = _reachibilityMatrix.GetStartProduction();

            var productions = GetProductionsFromProductionsModel();
            var ignoreRules = GetIgnoreRulesFromIgnoreRulesModel();

            return new Grammar(Start.LeftHandSide.NonTerminal, productions, ignoreRules);
        }

        private List<IProduction> GetProductionsFromProductionsModel()
        {
            var productions = new List<IProduction>();
            foreach (var productionModel in Productions)
                foreach (var production in productionModel.ToProductions())
                    productions.Add(production);
            return productions;
        }

        private List<ILexerRule> GetIgnoreRulesFromIgnoreRulesModel()
        {
            var ignoreRules = new List<ILexerRule>();
            foreach (var ignoreRuleModel in IgnoreRules)
                ignoreRules.Add(ignoreRuleModel.Value);
            return ignoreRules;
        }
        
        private void PopulateMissingProductionsFromStart(ProductionModel start)
        {
            var visited = new HashSet<INonTerminal>();
            PopulateMissingProductionsRecursively(start, visited);
        }

        private void PopulateMissingProductionsRecursively(ProductionModel production, ISet<INonTerminal> visited)
        {
            if (visited.Add(production.LeftHandSide.NonTerminal))
            {
                Productions.Add(production);
                foreach (var alteration in production.Alterations)
                    foreach (var symbol in alteration.Symbols)
                        if (symbol.ModelType == SymbolModelType.Production)
                            PopulateMissingProductionsRecursively(symbol as ProductionModel, visited);
            }
        }

        private void AssertStartProductionExistsForStartSymbol(ReachibilityMatrix reachibilityMatrix)
        {
            if (!reachibilityMatrix.ProudctionExistsForSymbol(Start.LeftHandSide))
                throw new Exception("no start production found for start symbol");
        }

        private bool StartSymbolExists()
        {
            return Start != null;
        }

        private bool ProductionsAreEmpty()
        {
            return Productions.Count == 0;
        }
    }
}
