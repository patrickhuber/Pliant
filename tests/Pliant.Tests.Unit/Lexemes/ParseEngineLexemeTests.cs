using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParseEngineLexemeTests
    {
        [TestMethod]
        public void ParseEngineLexemeShouldConsumeWhitespace()
        {
            ProductionExpression 
                S = "S",
                W = "W";

            S.Rule = W | W + S;
            W.Rule = new WhitespaceTerminal();

            var grammar = new GrammarExpression(S, new[] { S, W }).ToGrammar();

            var lexerRule = new GrammarLexerRule(
                "whitespace",
                grammar);

            var parseEngine = new ParseEngine(lexerRule.Grammar);
            var lexeme = new ParseEngineLexeme(parseEngine, new TokenType("whitespace"));
            var input = "\t\r\n\v\f ";
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]), "Unable to recognize input[{0}]", i);
            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void ParseEngineLexemeShouldConsumeCharacterSequence()
        {
            ProductionExpression sequence = "sequence";
            sequence.Rule = (Expr)'a' + 'b' + 'c' + '1' + '2' + '3';

            var grammar = new GrammarExpression(sequence, new[] { sequence }).ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            var lexeme = new ParseEngineLexeme(parseEngine, new TokenType("sequence"));
            var input = "abc123";
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]));
            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void ParseEngineLexemeShouldMatchLongestAcceptableTokenWhenGivenAmbiguity()
        {
            var lexemeList = new List<ParseEngineLexeme>();

            ProductionExpression There = "there";
            There.Rule = (Expr)'t' + 'h' + 'e' + 'r' + 'e';
            var thereGrammar = new GrammarExpression(There, new[] { There })
                .ToGrammar();
            var thereParseEngine = new ParseEngine(thereGrammar);
            var thereLexeme = new ParseEngineLexeme(thereParseEngine, new TokenType(There.ProductionModel.LeftHandSide.NonTerminal.Value));
            lexemeList.Add(thereLexeme);

            ProductionExpression Therefore = "therefore";
            Therefore.Rule = (Expr)'t' + 'h' + 'e' + 'r' + 'e' + 'f' + 'o' + 'r' + 'e';
            var thereforeGrammar = new GrammarExpression(Therefore, new[] { Therefore })
                .ToGrammar();
            var parseEngine = new ParseEngine(thereforeGrammar);
            var thereforeLexeme = new ParseEngineLexeme(parseEngine, new TokenType(Therefore.ProductionModel.LeftHandSide.NonTerminal.Value));
            lexemeList.Add(thereforeLexeme);

            var input = "therefore";
            var i = 0;
            for (; i < input.Length; i++)
            {
                var passedLexemes = lexemeList
                    .Where(l => l.Scan(input[i]))
                    .ToList();

                // all existing lexemes have failed
                // fall back onto the lexemes that existed before
                // we read this character
                if (passedLexemes.Count() == 0)
                    break;

                lexemeList = passedLexemes;
            }

            Assert.AreEqual(i, input.Length);
            Assert.AreEqual(1, lexemeList.Count);
            var remainingLexeme = lexemeList[0];
            Assert.IsNotNull(remainingLexeme);
            Assert.IsTrue(remainingLexeme.IsAccepted());
        }
    }
}