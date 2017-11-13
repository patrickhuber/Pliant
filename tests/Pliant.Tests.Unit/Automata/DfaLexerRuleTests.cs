using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class DfaLexerRuleTests
    {
        [TestMethod]
        public void DfaLexerRuleShouldApplyToCharacterIfFirstStateHasTransition()
        {
            var states = new DfaState[2];
            for (var i = 0; i < states.Length; i++)
                states[i] = new DfaState(i == states.Length - 1);

            var whitespaceToFinal = new DfaTransition(new WhitespaceTerminal(), states[1]);
            states[0].AddTransition(whitespaceToFinal);
            states[1].AddTransition(whitespaceToFinal);

            var dfaLexerRule = new DfaLexerRule(states[0], new TokenType(@"\s+"));

            Assert.IsTrue(dfaLexerRule.CanApply(' '));
            Assert.IsTrue(dfaLexerRule.CanApply('\t'));
            Assert.IsTrue(dfaLexerRule.CanApply('\r'));
            Assert.IsFalse(dfaLexerRule.CanApply('a'));
        }
    }
}
