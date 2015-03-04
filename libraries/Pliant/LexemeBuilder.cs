using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class LexemeBuilder : ILexemeBuilder
    {
        private IList<ILexeme> _lexemes;

        public LexemeBuilder()
        {
            _lexemes = new List<ILexeme>();
        }

        public ILexemeBuilder Lexeme(string name, Action<ITerminalBuilder> terminals)
        {
            var terminalBuilder = new TerminalBuilder();
            terminals(terminalBuilder);
            foreach (var terminal in terminalBuilder.GetTerminals())
            {
                var lexeme = new Lexeme(new NonTerminal(name), terminalBuilder.GetTerminals().ToArray());
                _lexemes.Add(lexeme);
            }            
            return this;
        }

        public IList<ILexeme> GetLexemes()
        {
            return _lexemes;
        }
    }
}
