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
            //  S   -> W | C
            //  W   -> [\s]+
            //  C   -> [a-z]+
            var grammar = new GrammarBuilder("S", p=>p
                    .Production("S", r=>r
                        .Rule("W")
                        .Rule("C"))
                    .Production("W", r=>r
                        .Rule(new WhitespaceTerminal(), "whitespace"))
                    .Production("whitespace", r=>r
                        .Rule(new WhitespaceTerminal())
                        .Lambda())
                    .Production("C", r=>r
                        .Rule(new RangeTerminal('a','z'), "word"))
                    .Production("word", r=>r
                        .Rule(new RangeTerminal('a','z'))
                        .Lambda()))
                .GetGrammar();

            var lexer = new Lexer(grammar);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                lexer.Scan(c);
            }
        }
    }
}
