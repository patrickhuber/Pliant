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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

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
