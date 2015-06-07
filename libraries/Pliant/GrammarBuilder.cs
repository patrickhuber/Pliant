using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class GrammarBuilder : IGrammarBuilder
    {
        private IList<IProduction> _productions;
        private IList<ILexerRule> _lexerRules;
        private IList<string> _actions;
        private string _start;

        public GrammarBuilder(
            string start, 
            Action<IProductionBuilder> productions, 
            Action<ILexerRuleBuilder> lexerRules = null, 
            Action<ICommandBuilder> action = null)
        {
            _start = start;
            
            var productionBuilder = new ProductionBuilder();
            productions(productionBuilder);
            _productions = productionBuilder.GetProductions();

            var lexerRuleBuilder = new LexerRuleBuilder();
            if(lexerRules != null)
                lexerRules(lexerRuleBuilder);
            _lexerRules = lexerRuleBuilder.GetLexerRules();

            var ignoreBuilder = new CommandBuilder();
            if(action != null)
                action(ignoreBuilder);
            _actions = ignoreBuilder.GetIgnoreList();
        }
                
        public Grammar GetGrammar()
        {
            if (_start == null)
                throw new Exception("no start production specified");
            var startProduction = _productions.FirstOrDefault(x => x.LeftHandSide.Value == _start);
            if (startProduction == null)
                throw new Exception("no start production found for start symbol");
            var start = startProduction.LeftHandSide;
            var ignore = _lexerRules
                .Where(x => _actions.Contains(x.TokenType.Id));

            return new Grammar(start, _productions.ToArray(), _lexerRules.ToArray(), ignore.ToArray());
        }
    }
}
