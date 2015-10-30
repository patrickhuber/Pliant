using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ebnf;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Ebnf
{

    [TestClass]
    public class EbnfCompilerTests
    {
        [TestMethod]
        public void Test_EbnfCompiler_Creates_Character_Grammar_With_One_Production()
        {
            var input = @"Rule = 'a';";
            var ebnfCompiler = new EbnfCompiler();
            var actual = ebnfCompiler.Compile(input);
            
            var rule = new NonTerminal("Rule");
            var expected = new Grammar(
                rule,
                new[] { new Production(rule, new TerminalLexerRule('a')) },
                null);
            
            Assert.AreEqual(expected, actual);
        }
    }
}
