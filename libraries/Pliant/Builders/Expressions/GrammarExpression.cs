using Pliant.Builders;
using Pliant.Grammars;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders.Expressions
{
    public class GrammarExpression
    {
        public GrammarModel GrammarModel { get; private set; }

        public GrammarExpression(
            ProductionExpression start, 
            IReadOnlyList<ProductionExpression> productions = null,
            IReadOnlyList<LexerRuleModel> ignore = null,
            IReadOnlyList<LexerRuleModel> trivia = null)
        {
            Initialize(start, productions, ignore, trivia);
        }
        
        public GrammarExpression(
            ProductionExpression start,
            IReadOnlyList<ProductionExpression> productions,
            IReadOnlyList<ILexerRule> ignore,
            IReadOnlyList<ILexerRule> trivia)
        {
            var ignoreModelList = ToLexerRuleModelList(ignore);
            var triviaModelList = ToLexerRuleModelList(trivia);

            Initialize(start, productions, ignoreModelList, triviaModelList);
        }

        private static List<LexerRuleModel> ToLexerRuleModelList(IReadOnlyList<ILexerRule> lexerRuleList)
        {
            if (lexerRuleList == null)
                return null;
            List<LexerRuleModel> modelList = null;
            for (var i = 0; i < lexerRuleList.Count; i++)
            {
                if (i == 0)
                    modelList = new List<LexerRuleModel>();
                modelList.Add(new LexerRuleModel(lexerRuleList[i]));
            }

            return modelList;
        }

        private void Initialize(ProductionExpression start, IReadOnlyList<ProductionExpression> productions, IReadOnlyList<LexerRuleModel> ignore, IReadOnlyList<LexerRuleModel> trivia)
        {
            GrammarModel = new GrammarModel
            {
                Start = start.ProductionModel
            };

            if (productions != null)
                foreach (var production in productions)
                    GrammarModel.Productions.Add(production.ProductionModel);

            if (ignore != null)
                foreach (var ignoreRule in ignore)
                {
                    GrammarModel.IgnoreSettings.Add(
                        new IgnoreSettingModel(ignoreRule));

                    GrammarModel.LexerRules.Add(
                        ignoreRule);
                }

            if (trivia != null)
                foreach (var triviaRule in trivia)
                {
                    GrammarModel.TriviaSettings.Add(
                        new TriviaSettingModel(triviaRule));
                    GrammarModel.LexerRules.Add(
                        triviaRule);
                }
        }

        public IGrammar ToGrammar()
        {
            return GrammarModel.ToGrammar();
        }
    }
}
