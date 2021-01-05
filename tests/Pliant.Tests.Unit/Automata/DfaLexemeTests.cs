using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Captures;
using Pliant.Grammars;
using Pliant.LexerRules;

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
            var whitespaceLexeme = new DfaLexeme(dfaLexerRule, randomWhitespace.AsCapture(), 0);
            for (int i = 0; i < randomWhitespace.Length; i++)
                Assert.IsTrue(whitespaceLexeme.Scan());
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
            var indentifierLexeme = new DfaLexeme(dfaLexerRule, wordInput.AsCapture(), 0);
            for (int i = 0; i < wordInput.Length; i++)
                Assert.IsTrue(indentifierLexeme.Scan());
        }

        [TestMethod]
        public void DfaLexemeGivenCharacerLexemeNumberShouldFail()
        {
            var numberInput = "0";
            var segment = numberInput.AsCapture();
            var dfa = new DfaState();
            var final = new DfaState(true);
            dfa.AddTransition(new DfaTransition(new RangeTerminal('a', 'z'), final));
            final.AddTransition(new DfaTransition(new RangeTerminal('a', 'z'), final));

            var dfaLexerRule = new DfaLexerRule(dfa, new TokenType("lowerCase"));
            var letterLexeme = new DfaLexeme(dfaLexerRule, segment, 0);
            Assert.IsFalse(letterLexeme.Scan());
            Assert.AreEqual(string.Empty, letterLexeme.Capture.ToString());
        }

        [TestMethod]
        public void DfaLexemeResetShouldResetLexemeValues()
        {
            var numberLexerRule = new NumberLexerRule();
            var whitespaceLexerRule = new WhitespaceLexerRule();

            const string numberInput = "0123456";
            var segment = numberInput.AsCapture();
            var lexeme = new DfaLexeme(numberLexerRule, segment, 0);
            for (var i = 0; i < numberInput.Length; i++)
            {
                var result = lexeme.Scan();
                if (!result)
                    Assert.Fail($"Did not recognize number {numberInput[i]}");
            }
            
            lexeme.Reset(whitespaceLexerRule, 0);
            Assert.AreEqual(string.Empty, lexeme.Capture.ToString());
            Assert.AreEqual(0, lexeme.Position);
            Assert.AreEqual(whitespaceLexerRule.LexerRuleType, lexeme.LexerRule.LexerRuleType);
            Assert.AreEqual(whitespaceLexerRule.TokenType, lexeme.TokenType);
        }
    }
}