using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Collections;

namespace Pliant.Tests.Unit.Collections
{
    /// <summary>
    /// Summary description for BitMatrixTests
    /// </summary>
    [TestClass]
    public class BitMatrixTests
    {
        public TestContext TestContext { get; set; }
             
        [TestMethod]
        public void BitMatrixShouldComputeTransitiveClosure()
        {
            var bitMatrix = new BitMatrix(3);
            bitMatrix[0][1] = true;
            bitMatrix[0][2] = true;
            bitMatrix[1][0] = true;

            var transitiveClosure = bitMatrix.TransitiveClosure();

            Assert.IsTrue(transitiveClosure[0][0]);
            Assert.IsTrue(transitiveClosure[0][1]);
            Assert.IsTrue(transitiveClosure[0][2]);
            Assert.IsTrue(transitiveClosure[1][0]);
            Assert.IsTrue(transitiveClosure[1][1]);
            Assert.IsTrue(transitiveClosure[1][2]);
        }

        [TestMethod]
        public void BitMatrixShouldComputeTransitiveClosureOverCycleGraph()
        {
            // Grammar
            // A -> a B | a
            // B -> b C | b
            // C -> c A | c

            // Verticies
            // ------------
            // (0 ) A -> . a B 
            // (1 ) A -> . a
            // (2 ) A -> a . B
            // (3 ) A -> a .
            // (4 ) A -> a B .            
            // (5 ) B -> . b C
            // (6 ) B -> . b
            // (7 ) B -> b . C
            // (8 ) B -> b .
            // (9 ) B -> b C .
            // (10) C -> . c A
            // (11) C -> . c
            // (12) C -> c . A
            // (13) C -> c .
            // (14) C -> c A .

            var bitMatrix = new BitMatrix(15);
            bitMatrix[0][2] = true;
            bitMatrix[1][3] = true;
            bitMatrix[3][4] = true;
            bitMatrix[3][5] = true;
            bitMatrix[3][6] = true;
            bitMatrix[5][7] = true;
            bitMatrix[6][8] = true;
            bitMatrix[8][9] = true;
            bitMatrix[8][10] = true;
            bitMatrix[8][11] = true;
            bitMatrix[10][12] = true;
            bitMatrix[11][13] = true;
            bitMatrix[13][0] = true;
            bitMatrix[13][1] = true;
            bitMatrix[13][14] = true;

            var transitiveClosure = bitMatrix.TransitiveClosure();
            for (var j = 0; j <= 14; j++)
            {
                Assert.IsTrue(transitiveClosure[1][j]);
                Assert.IsTrue(transitiveClosure[3][j]);
                Assert.IsTrue(transitiveClosure[6][j]);
                Assert.IsTrue(transitiveClosure[8][j]);
                Assert.IsTrue(transitiveClosure[11][j]);
                Assert.IsTrue(transitiveClosure[13][j]);
            }
            Assert.IsTrue(transitiveClosure[0][2]);
            Assert.IsTrue(transitiveClosure[5][7]);
            Assert.IsTrue(transitiveClosure[10][12]);
            Assert.IsFalse(transitiveClosure[10][13]);           
        }

        [TestMethod]
        [Ignore]
        public void BitMatrixShouldComputeTransitiveClosureOfAycockHorspool()
        {            
        }
    }
}
