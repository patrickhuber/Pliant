using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.RegularExpressions;
using Pliant.Automata;
using Pliant.Grammars;


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
            var dfa = CreateDfaFromRegexPattern(pattern);
            Assert.IsNotNull(dfa);

            var lexerRule = new DfaLexerRule(dfa, "a");
            var lexeme = new DfaLexemeFactory().Create(lexerRule, 0);
            Assert.IsTrue(lexeme.Scan('a'));
        }

        [TestMethod]
        public void RegexToDfaShouldConvertOptionalCharacterClassToDfa()
        {
            var pattern = @"[-+]?[0-9]";
            var dfa = CreateDfaFromRegexPattern(pattern);
            Assert.IsNotNull(dfa);
            Assert.AreEqual(3, dfa.Transitions.Count);
            var lexerRule = new DfaLexerRule(dfa, pattern);
            AssertLexerRuleMatches(lexerRule, "+0");
            AssertLexerRuleMatches(lexerRule, "-1");
            AssertLexerRuleMatches(lexerRule, "9");
        }

        [TestMethod]
        public void RegexToDfaShouldConvertSlasSToWhitespaceCharacterClass()
        {
            var pattern = @"\s";
            var dfa = CreateDfaFromRegexPattern(pattern);
            Assert.IsNotNull(dfa);
            Assert.AreEqual(1, dfa.Transitions.Count);
            Assert.AreEqual(0, dfa.Transitions[0].Target.Transitions.Count);
            var lexerRule = new DfaLexerRule(dfa, pattern);
            AssertLexerRuleMatches(lexerRule, " ");
            AssertLexerRuleMatches(lexerRule, "\t");
            AssertLexerRuleMatches(lexerRule, "\f");
        }
        
        private static IDfaState CreateDfaFromRegexPattern(string pattern)
        {
            var regex = new RegexParser().Parse(pattern);
            var nfa = new ThompsonConstructionAlgorithm().Transform(regex);
            var dfa = new SubsetConstructionAlgorithm().Transform(nfa);
            return dfa;
        }

        private static DfaLexemeFactory _factory = new DfaLexemeFactory();
         
        private static void AssertLexerRuleMatches(IDfaLexerRule lexerRule, string input)
        {
            var lexeme = _factory.Create(lexerRule, 0);
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]), $"character '{input[i]}' not recognized at position {i}.");
            Assert.IsTrue(lexeme.IsAccepted(), $"input {input} not accepted.");
        }
    }
}
