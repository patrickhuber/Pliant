using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void Test_Lexer_Scan_Emits_Three_Tokens()
        {
            var input = "three tokens";
            //  S   ->  S W
            //  S   ->  S L
            //  S   ->  <null>
            //  W   ->  W \s
            //  W   ->  \s
            //  L   ->  L [a-z]
            //  L   ->  [a-z]
            var grammar = new GrammarBuilder("S", p=>p
                    .Production("S", r=>r
                        .Rule("S", "W")
                        .Rule("S", "L")
                        .Lambda())
                    .Production("W", r=>r
                        .Rule(new WhitespaceTerminal(), "W`"))
                    .Production("W`", r=>r
                        .Rule(new WhitespaceTerminal(), "W`")
                        .Lambda())
                    .Production("L", r=>r
                        .Rule(new RangeTerminal('a', 'z'), "L`"))
                    .Production("L`", r=>r
                        .Rule(new RangeTerminal('a', 'z'), "L`")
                        .Lambda()))
                .GetGrammar();

            var lexer = new Lexer(grammar);

            int tokenCount = 0;
            var tokenObserver = new Observer<IToken>(t=>
            {
                tokenCount++;
            });
            lexer.Subscribe(tokenObserver);

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (!lexer.Scan(c))
                    break;
            }

            Assert.AreEqual(3, tokenCount);
        }
    }
}
