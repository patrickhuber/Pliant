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
        PulseRecognizer _pulseRecognizer;
        IGrammar _regexGrammar;

        [TestInitialize]
        public void Initialize_Regex_Tests()
        {
            _pulseRecognizer = new PulseRecognizer(_regexGrammar);
        }

        public RegexTests()
        {
            _regexGrammar = new RegexGrammar();            
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test_Regex_That_Parses_Single_Character()
        {
            var input = "a";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_String_Literal()
        {
            var input = "abc";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Optional_Character_Class()
        {
            var input = "a?";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Whitespace_Character_Class()
        {
            var input = @"[\s]+";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Range_Character_Class()
        {
            var input = @"[a-z]";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Any_Character_Class()
        {
            var input = @".*";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Fails_On_MisMatched_Parenthesis()
        {
            var input = "(a";
            Assert.IsTrue(_pulseRecognizer.Pulse(input[0]));
            Assert.IsTrue(_pulseRecognizer.Pulse(input[1]));
            Assert.IsFalse(_pulseRecognizer.IsAccepted());
        }

        [TestMethod]
        public void Test_Regex_That_Fails_On_MisMatched_Brackets()
        {
            var input = "[abc";
            foreach (var c in input)
                Assert.IsTrue(_pulseRecognizer.Pulse(c));
            Assert.IsFalse(_pulseRecognizer.IsAccepted());
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Empty_Group()
        {
            var input = "()";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Empty_String()
        {
            var input = "";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Negative_Character_Class()
        {
            var input = "[^abc]";
            Recognize(input);
        }

        private void Recognize(string input)
        {
            foreach (var c in input)
                Assert.IsTrue(_pulseRecognizer.Pulse(c),
                    string.Format("Line 0, Column {1} : Invalid Character '{0}'",
                        c,
                        _pulseRecognizer.Location));
            Assert.IsTrue(_pulseRecognizer.IsAccepted(), "input is not recognized.");
        }
    }
}
