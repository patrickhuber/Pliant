using Pliant.Builders.Models;
using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders.Expressions
{
    public class GrammarExpression
    {
        public GrammarModel GrammarModel { get; private set; }

        public GrammarExpression(
            ProductionExpression start, 
            IEnumerable<ProductionExpression> productions, 
            IEnumerable<LexerRuleModel> ignore)
        {
            GrammarModel = new GrammarModel();
            GrammarModel.Start = start.ProductionModel;
            foreach (var production in productions)
                GrammarModel.Productions.Add(production.ProductionModel);
            foreach (var ignoreRule in ignore)
                GrammarModel.IgnoreRules.Add(ignoreRule);
        }

        public IGrammar ToGrammar()
        {
            return GrammarModel.ToGrammar();
        }
    }
}
