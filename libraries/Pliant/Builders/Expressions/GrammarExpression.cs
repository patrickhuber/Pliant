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
            if (lexerRuleList is null)
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
                for(var p = 0; p<productions.Count;p++)
                    GrammarModel.Productions.Add(productions[p].ProductionModel);

            if (ignore != null)
                for(var i =0;i<ignore.Count;i++)
                {
                    var ignoreRule = ignore[i];
                    GrammarModel.IgnoreSettings.Add(
                        new IgnoreSettingModel(ignoreRule));

                    GrammarModel.LexerRules.Add(
                        ignoreRule);
                }

            if (trivia != null)
                for(var t =0; t<trivia.Count;t++)
                {
                    var triviaRule = trivia[t];
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
