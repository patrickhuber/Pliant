using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for RegexTests
    /// </summary>
    [TestClass]
    public class RegexTests
    {
        IGrammar regexGrammar;
        public RegexTests()
        {
            regexGrammar = new GrammarBuilder("Regex", p => p
                    .Production("Regex", r => r
                        .Rule("Union")
                        .Rule("SimpleRegex"))
                    .Production("Union", r=>r
                        .Rule("Regex", '|', "SimpleRegex"))
                    .Production("SimpleRegex", r=>r
                        .Rule("Concatenation")
                        .Rule("BasicRegex"))
                    .Production("Concatenation", r=>r
                        .Rule("SimpleRegex", "BasicRegex"))
                    .Production("BasicRegex", r => r
                        .Rule("Star")
                        .Rule("Plus")
                        .Rule("ElementaryRegex"))
                    .Production("Star", r => r
                        .Rule("ElementaryRegex", '*'))
                    .Production("Plus", r => r
                        .Rule("ElementaryRegex", '+'))
                    .Production("ElementaryRegex", r=>r
                        .Rule("Group")
                        .Rule("Any")
                        .Rule("EndOfString")
                        .Rule("Character")
                        .Rule("Set"))
                    .Production("Group", r => r
                        .Rule('(', "Regex", ')'))
                    .Production("Any", r => r
                        .Rule('.'))
                    .Production("EndOfString", r => r
                        .Rule('$'))
                    .Production("Character", r => r
                        .Rule(new NegationTerminal(new MetaTerminal()))
                        .Rule('\\', new MetaTerminal()))
                    .Production("Set", r => r
                        .Rule("PositiveSet")
                        .Rule("NegativeSet"))
                    .Production("PositiveSet", r => r
                        .Rule('[', "SetItems", ']'))
                    .Production("NegativeSet", r => r
                        .Rule('[', '^', "SetItems", ']'))
                    .Production("SetItems", r => r
                        .Rule("SetItem")
                        .Rule("SetItem", "SetItems"))
                    .Production("SetItem", r => r
                        .Rule("Range")
                        .Rule("Character"))
                    .Production("Range", r => r
                        .Rule("Character", '-', "Character")))
                .GetGrammar();
        }

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
        public void Test_Regex_That_Parses_String_Literal()
        {
            var input = "";
            Assert.Fail();
        }
    }
}
