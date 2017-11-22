using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class WhitespaceLexerRule : DfaLexerRule
    {
        private static readonly IDfaState _start;
        private static readonly TokenType _staticTokenType = new TokenType(@"[\s]+");

        static WhitespaceLexerRule()
        {
            _start = new DfaState();
            var end = new DfaState(isFinal: true);
            var transition = new DfaTransition(
                new WhitespaceTerminal(),
                end);
            _start.AddTransition(transition);
            end.AddTransition(transition);
        }

        public WhitespaceLexerRule()
            : base(_start, _staticTokenType)
        {
        }
    }
}
