using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Fluent;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Builders.Fluent
{
    [TestClass]
    public class FluentGrammarBuilderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fluentGrammarBuilder = new FluentGrammarBuilder();
            fluentGrammarBuilder.Grammar(p => 
            {
                var S = new NonTerminal("S");
                var A = new NonTerminal("A");
                var B = new NonTerminal("B");
                var C = new NonTerminal("C");
                var D = new NonTerminal("D");
                var E = new NonTerminal("E");

                var b = new StringLiteralLexerRule("b");
                p.Production(S, rules => rules
                    .Rule(A, B, C)
                    .Or(D, E))
                  .Production(A, rules => rules
                    .Rule(D, C)
                    .Or())
                  .Production(B, rules=> rules
                    .Rule(b));
            });
        }
    }
}
