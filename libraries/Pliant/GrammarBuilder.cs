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
        private IList<ILexeme> _lexemes;
        private IList<string> _ignore;
        private string _start;

        public GrammarBuilder(
            string start, 
            Action<IProductionBuilder> productions, 
            Action<ILexemeBuilder> lexemes = null, 
            Action<IListBuilder<string>> ignore = null)
        {
            _start = start;
            
            var productionBuilder = new ProductionBuilder();
            productions(productionBuilder);
            _productions = productionBuilder.GetProductions();

            var lexemeBuilder = new LexemeBuilder();
            lexemes(lexemeBuilder);
            _lexemes = lexemeBuilder.GetLexemes();

            var ignoreBuilder = new ListBuilder<string>();
            ignore(ignoreBuilder);
            _ignore = ignoreBuilder.GetList();
        }
                
        public Grammar GetGrammar()
        {
            if (_start == null)
                throw new Exception("no start production specified");
            var startProduction = _productions.FirstOrDefault(x => x.LeftHandSide.Value == _start);
            if (startProduction == null)
                throw new Exception("no start production specified");
            var start = startProduction.LeftHandSide;
            var ignore = _lexemes
                .Where(x => _ignore.Contains(x.LeftHandSide.Value))
                .Select(l=>l.LeftHandSide);
            return new Grammar(start, _productions.ToArray(), _lexemes.ToArray(), ignore.ToArray());
        }
    }
}
