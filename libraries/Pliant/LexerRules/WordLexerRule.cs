using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class WordLexerRule : DfaLexerRule
    {
        private static readonly IDfaState _start;
        private static readonly TokenType _staticTokenType = new TokenType(@"[\w]+");

        static WordLexerRule()
        {
            _start = new DfaState();
            var end = new DfaState(isFinal: true);
            var transition = new DfaTransition(
                new WordTerminal(),
                end);
            _start.AddTransition(transition);
            end.AddTransition(transition);
        }

        public WordLexerRule()
            : base(_start, _staticTokenType)
        {
        }
    }
}
