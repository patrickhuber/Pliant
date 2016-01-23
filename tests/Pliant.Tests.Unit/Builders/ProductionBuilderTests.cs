using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;

namespace Pliant.Tests.Unit.Builders
{
    [TestClass]
    public class ProductionBuilderTests
    {
        [TestMethod]
        public void ProductionBuilderShouldImplicitCastRule()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");
            s.Definition = a;
        }

        [TestMethod]
        public void ProductionBuilderShouldImplicitCastString()
        {
            var s = new ProductionBuilder("S");
            s.Rule("string");
        }

        [TestMethod]
        public void ProductionBuilderShouldImplicitCastCharacter()
        {
            var s = new ProductionBuilder("S");
            s.Rule('c');
        }

        [TestMethod]
        public void ProductionShouldResolveAlteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");
            var b = new ProductionBuilder("B");

            s.Rule(a)
             .Or(b);
        }

        [TestMethod]
        public void ProductionBuilderGivenTwoProductionBuildersShouldResolveConcatenation()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");
            var b = new ProductionBuilder("B");

            s.Rule(a, b);
        }

        [TestMethod]
        public void ProductionBuilderGivenCharAndProductionBuilderShouldResolveAlteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule('c')
             .Or(a);
        }

        [TestMethod]
        public void ProductionBuilderGivenProductionBuilderAndCharShouldResolveAlteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule(a)
             .Or('c');
        }

        [TestMethod]
        public void ProductionBuilderGivenStringAndProductionBuilderShouldResolveAlteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule("string")
                .Or(a);
        }

        [TestMethod]
        public void ProductionBuilderGivenProductionBuilderAndStringShouldResolveAlteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule(a)
                .Or("string");
        }

        [TestMethod]
        public void ProductionBuilderGivenTwoStringsShouldResolveAlteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule("one")
                .Or("two");
            var grammarBuilder = new GrammarBuilder(s, new[] { s, a });
        }
    }
}