using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Builders.Models
{
    public class GrammarModel
    {
        public ICollection<ProductionModel> Productions { get; private set; }

        public ICollection<LexerRuleModel> IgnoreRules { get; private set; }

        public NonTerminalModel Start { get; set; }

        public GrammarModel()
        {
            Productions = new List<ProductionModel>();
            IgnoreRules = new List<LexerRuleModel>();
        }

        public IGrammar ToGrammar()
        {
            ValidateOrAssignStartSymbol();
            var productions = GetProductionsFromProductionsModel();
            var ignoreRules = GetIgnoreRulesFromIgnoreRulesModel();
            return new Grammar(Start.Value, productions, ignoreRules);
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

        private void ValidateOrAssignStartSymbol()
        {
            var reachibilityMatrix = new ReachibilityMatrix(Productions);
            if (StartSymbolExists())
                AssertStartProductionExistsForStartSymbol(reachibilityMatrix);
            else
                Start = reachibilityMatrix.GetStartSymbol();
        }

        private void AssertStartProductionExistsForStartSymbol(ReachibilityMatrix reachibilityMatrix)
        {
            if (!reachibilityMatrix.ProudctionExistsForSymbol(Start))
                throw new Exception("no start production found for start symbol");
        }

        private bool StartSymbolExists()
        {
            return Start != null;
        }
    }
}
