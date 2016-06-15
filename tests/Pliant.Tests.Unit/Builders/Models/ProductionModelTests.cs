using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Models;
using System.Linq;

namespace Pliant.Tests.Unit.Builders.Models
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
    }
}
