using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for PulseRecognizerTests
    /// </summary>
    [TestClass]
    public class PulseRecognizerTests
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
        public void Test_PulseRecognizer_That_Single_Token_Increments_Parse()
        {
            const int whitespace = 1;
            const int other = 2;
            
            //  S   -> W
            //  W   -> [\s]+
            var grammar = new GrammarBuilder("S", p=>p
                .Production("S", r=>r
                    .Rule("whitespace`"))
                .Production("whitespace`", r=>r
                    .Rule(new WhitespaceTerminal(), "whitespace"))
                .Production("whitespace", r=>r
                    .Rule(new WhitespaceTerminal())
                    .Lambda())                )
                .GetGrammar();
            var pulseRecognizer = new PulseRecognizer(grammar);

            const string input = "\t\f\r\n ";
            int i = 0;
            while (i < pulseRecognizer.Chart.Count)
            {
                var c = i==input.Length ? (char)0 : input[i];
                pulseRecognizer.Pulse(c);
                i++;
            }
            Assert.IsTrue(pulseRecognizer.IsAccepted());
        }
    }
}
