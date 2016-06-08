using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Builders.Models;

namespace Pliant.Tests.Unit.Builders.Expressions
{
    [TestClass]
    public class NamespaceExpressionTests
    {
        [TestMethod]
        public void NamespaceExpressionShouldCreateFullyQualifiedNameFromNamespaceExpressionAndNameString()
        {
            NamespaceExpression ns1 = "namespace1";
            ProductionExpression
                S = ns1 + "S",
                A = ns1 + "A";
            S.Rule = A;
            A.Rule = 'a';

            var symbolS = S.ProductionModel.LeftHandSide;
            Assert.IsNotNull(symbolS);
            Assert.AreEqual(ns1.Namespace, symbolS.Value.Namespace);
            Assert.AreEqual(S.ProductionModel.LeftHandSide.Value.Name, symbolS.Value.Name);

            var symbolA = A.ProductionModel.LeftHandSide;
            Assert.IsNotNull(symbolA);
            Assert.AreEqual(ns1.Namespace, symbolA.Value.Namespace);
            Assert.AreEqual(A.ProductionModel.LeftHandSide.Value.Name, symbolA.Value.Name);
        }
    }
}
