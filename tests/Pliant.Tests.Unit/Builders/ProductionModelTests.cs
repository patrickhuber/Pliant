using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using System.Linq;

namespace Pliant.Tests.Unit.Builders
{
    [TestClass]
    public class ProductionModelTests
    {
        [TestMethod]
        public void ProductionModelToProductionsShouldReturnNullableProductionWhenRuleIsEmpty()
        {
            var E = new ProductionModel("E");
            Assert.AreEqual(1, E.ToProductions().Count());
        }

        [TestMethod]
        public void ProductionModelToProductionsShouldContainTwoProductionsWhenGivenNullTerminator()
        {
            var A = new ProductionModel("A");
            var B = new ProductionModel("B");
            A.AddWithAnd(B);
            A.AddWithOr(null);
        }
    }
}
