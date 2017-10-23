using Pliant.Builders;
using Pliant.Grammars;
using System.Collections.Generic;

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
            GrammarModel = new GrammarModel
            {
                Start = start.ProductionModel
            };

            if(productions != null)
                foreach (var production in productions)
                    GrammarModel.Productions.Add(production.ProductionModel);

            if(ignore != null)
                foreach (var ignoreRule in ignore)
                    GrammarModel.IgnoreRules.Add(ignoreRule);
            
            if (trivia != null)
                foreach (var triviaRule in trivia)
                    GrammarModel.TriviaRules.Add(triviaRule);
        }

        public GrammarExpression(
            ProductionExpression start,
            IReadOnlyList<ProductionExpression> productions,
            IReadOnlyList<ILexerRule> ignore,
            IReadOnlyList<ILexerRule> trivia)            
        {
            GrammarModel = new GrammarModel
            {
                Start = start.ProductionModel
            };

            if (productions != null)
                for (var p = 0; p < productions.Count; p++)
                {
                    var production = productions[p];
                    GrammarModel.Productions.Add(production.ProductionModel);
                }

            if (ignore != null)
                for (var i = 0; i < ignore.Count; i++)
                {
                    var ignoreRule = ignore[i];
                    GrammarModel.IgnoreRules.Add(new LexerRuleModel(ignoreRule));
                }

            if (trivia != null)
                for (var t = 0; t < trivia.Count; t++)
                {
                    var singleTrivia = trivia[t];
                    GrammarModel.TriviaRules.Add(new LexerRuleModel(singleTrivia));
                }
        }

        public IGrammar ToGrammar()
        {
            return GrammarModel.ToGrammar();
        }
    }
}
