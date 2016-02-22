using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ProductionTests
    {
        [TestMethod]
        public void ProductionToStringShouldGenerateCorrectlyFormattedString()
        {
            var production = new Production(new NonTerminal("A"), new NonTerminal("B"));
            Assert.AreEqual("A -> B", production.ToString());
        }


        [TestMethod]
        public void ProductionGetHashCodeShouldProduceSameValueForEmptyProductionWithSameLeftHandSide()
        {
            var production1 = new Production(new NonTerminal("A"));
            var production2 = new Production(new NonTerminal("A"));
            Assert.AreEqual(production1.GetHashCode(), production2.GetHashCode());
        }

        [TestMethod]
        public void ProductionGetHashCodeShouldProduceSameValueForSameRightHandSides()
        {
            var production1 = new Production(new NonTerminal("A"), new CharacterTerminal('a'), new NonTerminal("B"));
            var production2 = new Production(new NonTerminal("A"), new CharacterTerminal('a'), new NonTerminal("B"));

            Assert.AreEqual(production1.GetHashCode(), production2.GetHashCode());
        }

        [TestMethod]
        public void ProductionGetHashCodeShouldNotProduceSameValueForDifferentLeftHandSides()
        {
            var production1 = new Production(new NonTerminal("A"));
            var production2 = new Production(new NonTerminal("B"));
            Assert.AreNotEqual(production1.GetHashCode(), production2.GetHashCode());
        }

        [TestMethod]
        public void ProductionGetHashCodeShouldProduceSameValueForSameObject()
        {
            var production = new Production(new NonTerminal("Z"), 
                new CharacterTerminal('a'), 
                new NonTerminal("B"),
                new SetTerminal('a','z'));
            var hashCode = production.GetHashCode();
            Assert.AreEqual(hashCode, production.GetHashCode());
        }
    }
}