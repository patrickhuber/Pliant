using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class NumberLexerRule : DfaLexerRule
    {
        private const string _pattern = @"[-+]?[0-9]*[.]?[0-9]+";
        private static readonly IDfaState _start;
        private static readonly TokenType _staticTokenType = new TokenType(_pattern);

        static NumberLexerRule()
        {
            var states = new DfaState[5];
            for (int i = 0; i < states.Length; i++)            
                states[i] = new DfaState(i==4 || i== 2);            

            var zeroThroughNine = new RangeTerminal('0', '9');

            var plusOrMinusTo1 = new DfaTransition(new SetTerminal('+', '-'), states[1]);
            var dotTo3 = new DfaTransition(new CharacterTerminal('.'), states[3]);
            var zeroThroughNineTo2 = new DfaTransition(zeroThroughNine, states[2]);
            var zeroThroughNineTo4 = new DfaTransition(zeroThroughNine, states[4]);

            states[0].AddTransition(dotTo3);
            states[0].AddTransition(plusOrMinusTo1);
            states[0].AddTransition(zeroThroughNineTo2);

            states[1].AddTransition(dotTo3);
            states[1].AddTransition(zeroThroughNineTo2);

            states[2].AddTransition(zeroThroughNineTo2);
            states[2].AddTransition(dotTo3);

            states[3].AddTransition(zeroThroughNineTo4);

            states[4].AddTransition(zeroThroughNineTo4);

            _start = states[0];
        }

        public NumberLexerRule()
            : base(_start, _staticTokenType)
        {
        }
    }
}
