using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class DfaLexemeTests
    {
        [TestMethod]
        public void DfaLexemeShouldMatchOneOrMoreRandomWhitespaceCharacters()
        {
            var randomWhitespace = "\t\f\v \r\n";
            var dfa = new DfaState();
            var final = new DfaState(true);
            dfa.AddTransition(new DfaTransition(new WhitespaceTerminal(), final));
            final.AddTransition(new DfaTransition(new WhitespaceTerminal(), final));

            var dfaLexerRule = new DfaLexerRule(dfa, new TokenType("whitespace"));
            var whitespaceLexeme = new DfaLexeme(dfaLexerRule);
            for (int i = 0; i < randomWhitespace.Length; i++)
                Assert.IsTrue(whitespaceLexeme.Scan(randomWhitespace[i]));
        }

        [TestMethod]
        public void DfaLexemeShouldMatchMixedCaseWord()
        {
            var wordInput = "t90vAriabl3";
            var dfa = new DfaState();
            var final = new DfaState(true);
            dfa.AddTransition(new DfaTransition(new RangeTerminal('a', 'z'), final));
            dfa.AddTransition(new DfaTransition(new RangeTerminal('A', 'Z'), final));
            final.AddTransition(new DfaTransition(new RangeTerminal('a', 'z'), final));
            final.AddTransition(new DfaTransition(new RangeTerminal('A', 'Z'), final));
            final.AddTransition(new DfaTransition(new DigitTerminal(), final));

            var dfaLexerRule = new DfaLexerRule(dfa, new TokenType("Identifier"));
            var indentifierLexeme = new DfaLexeme(dfaLexerRule);
            for (int i = 0; i < wordInput.Length; i++)
                Assert.IsTrue(indentifierLexeme.Scan(wordInput[i]));
        }

        [TestMethod]
        public void DfaLexemeGivenCharacerLexemeNumberShouldFail()
        {
            var numberInput = "0";
            var dfa = new DfaState();
            var final = new DfaState(true);
            dfa.AddTransition(new DfaTransition(new RangeTerminal('a', 'z'), final));
            final.AddTransition(new DfaTransition(new RangeTerminal('a', 'z'), final));

            var dfaLexerRule = new DfaLexerRule(dfa, new TokenType("lowerCase"));
            var letterLexeme = new DfaLexeme(dfaLexerRule);
            Assert.IsFalse(letterLexeme.Scan(numberInput[0]));
            Assert.AreEqual(string.Empty, letterLexeme.Capture);
        }
    }
}