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
            ProductionBuilder 
                s = "S",
                a = "A";
            s.Definition = a;

            AssertProductionCount(s, 2, s, a);
        }

        [TestMethod]
        public void ProductionBuilderShouldImplicitCastString()
        {
            ProductionBuilder s = "S";
            s.Rule("string");

            AssertProductionCount(s, 1, s);
        }

        [TestMethod]
        public void ProductionBuilderShouldImplicitCastCharacter()
        {

            ProductionBuilder s = "S";
            s.Rule('c');
            
            AssertProductionCount(s, 1, s);
        }

        [TestMethod]
        public void ProductionShouldResolveAlteration()
        {
            ProductionBuilder
                s = "S",
                a = "A",
                b = "B";

            s.Rule(a)
             .Or(b);
            
            AssertProductionCount(s, 4, s, a, b);
        }

        [TestMethod]
        public void ProductionBuilderGivenTwoProductionBuildersShouldResolveConcatenation()
        {
            ProductionBuilder
                s = "S",
                a = "A",
                b = "B";

            s.Rule(a, b);
            
            AssertProductionCount(s, 3, s, a, b);
        }

        [TestMethod]
        public void ProductionBuilderGivenCharAndProductionBuilderShouldResolveAlteration()
        {
            ProductionBuilder
                s = "S",
                a = "A";

            s.Rule('c')
             .Or(a);

            AssertProductionCount(s, 3, s, a);
        }

        [TestMethod]
        public void ProductionBuilderGivenProductionBuilderAndCharShouldResolveAlteration()
        {
            ProductionBuilder
                s = "S",
                a = "A";

            s.Rule(a)
             .Or('c');
            
            AssertProductionCount(s, 3, s, a);
        }

        [TestMethod]
        public void ProductionBuilderGivenStringAndProductionBuilderShouldResolveAlteration()
        {
            ProductionBuilder
                s = "S",
                a = "A";

            s.Rule("string")
                .Or(a);
            
            AssertProductionCount(s, 3, s, a);
        }

        [TestMethod]
        public void ProductionBuilderGivenProductionBuilderAndStringShouldResolveAlteration()
        {
            ProductionBuilder
                s = "S",
                a = "A";

            s.Rule(a)
                .Or("string");
            AssertProductionCount(s, 3, s, a);
        }


        [TestMethod]
        public void ProductionBuilderGivenTwoStringsShouldResolveAlteration()
        {
            ProductionBuilder
                s = "S";

            s.Rule("one")
                .Or("two");

            AssertProductionCount(s, 2, s);
        }


        private static void AssertProductionCount(ProductionBuilder start, int count, params ProductionBuilder[] additional)
        {
            var grammarBuilder = new GrammarBuilder(start, additional);
            var grammar = grammarBuilder.ToGrammar();
            Assert.AreEqual(count, grammar.Productions.Count);
        }
    }
}