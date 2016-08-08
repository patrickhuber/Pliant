using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.RegularExpressions;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Lexemes;

namespace Pliant.Tests.Unit.RegularExpressions
{
    /// <summary>
    /// Summary description for RegexToDfaTests
    /// </summary>
    [TestClass]
    public class RegexToDfaTests
    {
        [TestMethod]
        public void RegexToDfaShouldConvertCharacterRegexToDfa()
        {
            var pattern = "a";
            var regex = new RegexParser().Parse(pattern);
            var nfa = new ThompsonConstructionAlgorithm().Transform(regex);
            var dfa = new SubsetConstructionAlgorithm().Transform(nfa);
            Assert.IsNotNull(dfa);

            var lexerRule = new DfaLexerRule(dfa, "a");
            var lexeme = new DfaLexemeFactory().Create(lexerRule);
            Assert.IsTrue(lexeme.Scan('a'));
        }

        [TestMethod]
        public void RegexToDfaShouldConvertOptionalCharacterClassToDfa()
        {
            var pattern = @"[-+]?[0-9]";
            var regex = new RegexParser().Parse(pattern);
            var nfa = new ThompsonConstructionAlgorithm().Transform(regex);
            var dfa = new SubsetConstructionAlgorithm().Transform(nfa);
            Assert.IsNotNull(dfa);
            Assert.AreEqual(3, dfa.Transitions.Count);
            var lexerRule = new DfaLexerRule(dfa, pattern);
            AssertLexerRuleMatches(lexerRule, "+0");
            AssertLexerRuleMatches(lexerRule, "-1");
            AssertLexerRuleMatches(lexerRule, "9");
        }

        private static DfaLexemeFactory _factory = new DfaLexemeFactory();
         
        private static void AssertLexerRuleMatches(IDfaLexerRule lexerRule, string input)
        {
            var lexeme = _factory.Create(lexerRule);
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]), $"character '{input[i]}' not recognized at position {i}.");
            Assert.IsTrue(lexeme.IsAccepted(), $"input {input} not accepted.");
        }
    }
}
