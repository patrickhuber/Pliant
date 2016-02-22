using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ebnf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tests.Unit.Ebnf
{
    [TestClass]
    public class EbnfParserTests
    {
        [TestMethod]
        public void EbnfParserShouldParseEmptyRule()
        {
            var ebnf = Parse(@"");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenCharacterShouldCreateOneProductionn()
        {
            var ebnf =  Parse(@"Rule = 'a';");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenConcatenationShouldCreateOneProduction()
        {
            var ebnf =  Parse(@"Rule = 'a' 'b';");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenAlterationShouldCreateTwoProductions()
        {
            var ebnf =  Parse(@"Rule = 'a' | 'b';");
            Assert.IsNotNull(ebnf);

        }

        [TestMethod]
        public void EbnfParserGivenAlterationAndConcatenationShouldCreateTwoProductions()
        {
            var ebnf =  Parse(@"Rule = 'a' 'b' | 'c';");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenRegexShouldCreateLexerRule()
        {
            var ebnf =  Parse(@"Rule = /[a-z]/;");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenBracesShouldCreateRepetition()
        {
            var ebnf =  Parse(@"Rule = { 'a' };");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenBracketsShouldCreateOptional()
        {
            var ebnf =  Parse(@"Rule = [ 'a' ];");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenParanthesisShouldCreateGrouping()
        {
            var ebnf =  Parse(@"Rule = ('a');");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserGivenNamespaceShouldCreateQualifiedIdentifier()
        {
            var ebnf = Parse(@"This.Is.A.Namespace.Rule = 'a'; ");
            Assert.IsNotNull(ebnf);
        }

        private static EbnfDefinition Parse(string input)
        {
            var ebnfParser = new EbnfParser();
            return ebnfParser.Parse(input);
        }
    }
}
