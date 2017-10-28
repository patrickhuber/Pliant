using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Pliant.Builders
{
    public class GrammarModel
    {
        private ObservableCollection<ProductionModel> _productions;
        private List<LexerRuleModel> _lexerRules;

        private List<TriviaSettingModel> _triviaSettings;
        private List<IgnoreSettingModel> _ignoreSettings;

        public ICollection<ProductionModel> Productions { get { return _productions; } }
        
        public ICollection<TriviaSettingModel> TriviaSettings { get { return _triviaSettings; } }

        public ICollection<IgnoreSettingModel> IgnoreSettings { get { return _ignoreSettings; } }

        public ICollection<LexerRuleModel> LexerRules { get { return _lexerRules; } }

        ProductionModel _start;

        public ProductionModel Start
        {
            get { return _start; }
            set
            {
                if (value != null)
                {
                    StartSetting = new StartProductionSettingModel(value);
                }
                _start = value;
            }
        }

        public StartProductionSettingModel StartSetting { get; set; }

        private ReachibilityMatrix _reachibilityMatrix;

        public GrammarModel()
        {
            _reachibilityMatrix = new ReachibilityMatrix();
            
            _productions = new ObservableCollection<ProductionModel>();
            _productions.CollectionChanged += ProductionsCollectionChanged;
            _lexerRules = new List<LexerRuleModel>();

            _ignoreSettings = new List<IgnoreSettingModel>();
            _triviaSettings = new List<TriviaSettingModel>();
        }

        public GrammarModel(ProductionModel start)
            : this()
        {
            Start = start;
        }
        
        void ProductionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void OnResetProductions()
        {
            Start = null;
            _reachibilityMatrix.ClearProductions();
        }

        public IGrammar ToGrammar()
        {
            SetStartProduction();

            var productions = GetProductionsFromProductionsModel();
            var ignoreRules = GetIgnoreRulesFromIgnoreRulesModel();
            var triviaRules = GetTriviaRulesFromTriviaRulesModel();

            if (Start == null)
                throw new Exception("Unable to generate Grammar. The grammar definition is missing a Start production");

            if (Start.LeftHandSide == null)
                throw new Exception("Unable to generate Grammar. The grammar definition is missing a Left Hand Symbol to the Start production.");

            return new Grammar(
                Start.LeftHandSide.NonTerminal,
                productions,
                ignoreRules,
                triviaRules);
        }

        private void SetStartProduction()
        {
            if (StartSymbolExists())
            {
                if (ProductionsAreEmpty())
                    PopulateMissingProductionsFromStart(Start);
                AssertStartProductionExistsForStartSymbol(_reachibilityMatrix);
            }
            else if(StartSettingExists())
            {
                if (ProductionsAreEmpty())
                    throw new InvalidOperationException("Unable to determine start symbol. No productions exist and a start symbol was not specified.");
                AssertStartProductionexistsForStartSetting(_reachibilityMatrix);
                Start = FindProduction(StartSetting.Value);
            }
            else { Start = _reachibilityMatrix.GetStartProduction(); }
        }

        private ProductionModel FindProduction(string value)
        {
            for (var p = 0; p < _productions.Count; p++)
            {
                var productionModel = _productions[p];
                if (productionModel.LeftHandSide.NonTerminal.Value.Equals(value))
                    return productionModel;
            }
            return null;
        }

        private List<IProduction> GetProductionsFromProductionsModel()
        {
            var productions = new List<IProduction>();
            foreach (var productionModel in _productions)
                foreach (var production in productionModel.ToProductions())
                    productions.Add(production);
            return productions;
        }

        private List<ILexerRule> GetIgnoreRulesFromIgnoreRulesModel()
        {
            return GetLexerRulesFromSettings(_ignoreSettings);
        }

        private List<ILexerRule> GetTriviaRulesFromTriviaRulesModel()
        {
            return GetLexerRulesFromSettings(_triviaSettings);
        }

        private List<ILexerRule> GetLexerRulesFromSettings(IReadOnlyList<SettingModel> settings)
        {
            var lexerRules = new List<ILexerRule>();
            for (var i = 0; i < settings.Count; i++)
            {
                var setting = settings[i];
                var lexerRule = GetLexerRuleByName(setting.Value);
                if (lexerRule == null)
                    throw new Exception($"lexer rule {setting.Value} not found.");
                lexerRules.Add(lexerRule);
            }
            return lexerRules;
        }

        private ILexerRule GetLexerRuleByName(string value)
        {
            for (var i = 0; i < _lexerRules.Count; i++)
            {
                var lexerRuleModel = _lexerRules[i];
                var lexerRule = lexerRuleModel.Value;
                if (lexerRule.TokenType.Id.Equals(value))
                    return lexerRule;
            }
            return null;
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
                    for (var s =0; s< alteration.Symbols.Count; s++)
                    {
                        var symbol = alteration.Symbols[s];
                        if (symbol.ModelType == SymbolModelType.Production)
                            PopulateMissingProductionsRecursively(symbol as ProductionModel, visited);
                    }
            }
        }

        private void AssertStartProductionExistsForStartSymbol(ReachibilityMatrix reachibilityMatrix)
        {
            if (!reachibilityMatrix.ProudctionExistsForSymbol(Start.LeftHandSide))
                throw new Exception("no start production found for start symbol");
        }

        private void AssertStartProductionexistsForStartSetting(ReachibilityMatrix reachibilityMatrix)
        {
            if (!reachibilityMatrix.ProudctionExistsForSymbol(
                new NonTerminalModel(StartSetting.Value)))
                throw new Exception("no start production found for start symbol");
        }

        private bool StartSymbolExists()
        {
            return Start != null;
        }

        private bool StartSettingExists()
        {
            return StartSetting != null;
        }

        private bool ProductionsAreEmpty()
        {
            return Productions.Count == 0;
        }
    }
}
