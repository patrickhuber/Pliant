using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.RegularExpressions;
using Pliant.Automata;

namespace Pliant.Tests.Unit.RegularExpressions
{
    /// <summary>
    /// Summary description for RegexToDfaTests
    /// </summary>
    [TestClass]
    public class RegexToDfaTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }
        
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
    }
}
