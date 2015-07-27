using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class DfaLexemeTests
    {
        [TestMethod]
        public void Test_DfaLexeme_That_Whitespace_One_Or_Many_Matches_Random_Whitespace()
        {
            var randomWhitespace = "\t\f\v \r\n";
            var dfa = new DfaState();
            var final = new DfaState(true);
            dfa.AddEdge(new DfaEdge(new WhitespaceTerminal(), final));
            final.AddEdge(new DfaEdge(new WhitespaceTerminal(), final));

            var whitespaceLexeme = new DfaLexeme(dfa, new TokenType("whitespace"));
            for (int i = 0; i < randomWhitespace.Length; i++)
                Assert.IsTrue(whitespaceLexeme.Scan(randomWhitespace[i]));
        }

        [TestMethod]
        public void Test_DfaLexeme_Given_Mixed_Case_Word_When_Identifier_Lexeme_Then_Matches_Input()
        {
            var wordInput = "t90vAriabl3";
            var dfa = new DfaState();
            var final = new DfaState(true);
            dfa.AddEdge(new DfaEdge(new RangeTerminal('a', 'z'), final));
            dfa.AddEdge(new DfaEdge(new RangeTerminal('A', 'Z'), final));
            final.AddEdge(new DfaEdge(new RangeTerminal('a', 'z'), final));
            final.AddEdge(new DfaEdge(new RangeTerminal('A', 'Z'), final));
            final.AddEdge(new DfaEdge(new DigitTerminal(), final));

            var indentifierLexeme = new DfaLexeme(dfa, new TokenType("Identifier"));
            for (int i = 0; i < wordInput.Length; i++)
                Assert.IsTrue(indentifierLexeme.Scan(wordInput[i]));
        }

        [TestMethod]
        public void Test_DfaLexeme_Given_Number_When_Character_Lexeme_Then_Failes_Match()
        {
            var numberInput = "0";
            var dfa = new DfaState();
            var final = new DfaState(true);
            dfa.AddEdge(new DfaEdge(new RangeTerminal('a', 'z'), final));
            final.AddEdge(new DfaEdge(new RangeTerminal('a', 'z'), final));
            var letterLexeme = new DfaLexeme(dfa, new TokenType("lowerCase"));
            Assert.IsFalse(letterLexeme.Scan(numberInput[0]));
            Assert.AreEqual(string.Empty, letterLexeme.Capture);
        }
    }
}
