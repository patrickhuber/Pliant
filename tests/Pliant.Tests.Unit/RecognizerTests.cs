using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class RecognizerTests
    {
        // S -> S + M | M
        // M -> M * T | T
        // T -> 1 | 2 | 3 | 4
        private readonly Grammar expressionGrammar = new GrammarBuilder(
                "S", p=>p
                .Production("S", r=>r
                    .Rule("S", '+', "M")
                    .Rule("M"))
                .Production("M", r=>r
                    .Rule("M", '*', "T")
                    .Rule("T"))
                .Production("T", r=>r
                    .Rule(new DigitTerminal())))
            .GetGrammar();

        // A -> B C
        // B -> b
        // C -> c
        private readonly Grammar abcGrammar = new GrammarBuilder(
                "A", p => p
                    .Production("A", r=>r
                        .Rule("B", "C"))
                    .Production("B", r=>r
                        .Rule('b'))
                    .Production("C", r=>r
                        .Rule('c')))
            .GetGrammar();

        // A -> Aa
        // A -> 
        private readonly Grammar simpleRightRecursive = new GrammarBuilder("A", p => p
                .Production("A", r => r
                    .Rule('a', "A")
                    .Lambda()))
            .GetGrammar();

        [TestMethod]
        public void Test_Recognizer_That_A_B_C_Grammar_Parses_bc()
        {            
            var recognizer = new Recognizer(abcGrammar);
            var stringReader = new StringReader("bc");
            Assert.IsTrue(recognizer.Recognize(stringReader));
        }

        [TestMethod]
        public void Test_Recognizer_That_S_M_T_Grammar_Parses_2_sum_3_mul_4()
        {
            var recognizer = new Recognizer(expressionGrammar);
            var stringReader = new StringReader("2+3*4");
            Assert.IsTrue(recognizer.Recognize(stringReader));
        }

        [TestMethod]
        public void Test_Recognizer_That_Unmarked_Middle_Recursion_Parses()
        {
            const string input = "aaaaaaaaa";
            var grammar = new GrammarBuilder("S", p=>p
                    .Production("S", r=>r
                        .Rule('a', "S", 'a')
                        .Rule('a')))
                .GetGrammar();
            var recognizer = new Recognizer(grammar);
            Assert.IsTrue(recognizer.Recognize(new StringReader(input)));
        }

       
        [TestMethod]
        public void Test_PulseRecognizer_That_Invalid_Input_Exists_Parse()
        {
            var expressionGrammar = new GrammarBuilder(
                "S", p => p
                .Production("S", r => r
                    .Rule("S", '+', "M")
                    .Rule("M"))
                .Production("M", r => r
                    .Rule("M", '*', "T")
                    .Rule("T"))
                .Production("T", r => r
                    .Rule('1')
                    .Rule('2')
                    .Rule('3')
                    .Rule('4')))
            .GetGrammar();
            var recognizer = new Recognizer(expressionGrammar);
            Assert.IsFalse(recognizer.Recognize(new StringReader("1+b*3")));
        }
    }
}
