using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Collections;
using System.Collections;

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
        [Ignore]
        public void BitMatrixShouldComputeTransitiveClosure()
        {
            var bitMatrix = new BitMatrix(2);
            bitMatrix[0][0] = true;
            bitMatrix.TransitiveClosure();
            Assert.IsTrue(bitMatrix[0][0]);
            Assert.IsFalse(bitMatrix[0][1]);
            Assert.IsFalse(bitMatrix[1][0]);
            Assert.IsFalse(bitMatrix[1][1]);
        }

        [TestMethod]
        [Ignore]
        public void BitMatrixShouldComputeTransitiveClosureOverIdentity()
        {
            var bitMatrix = new BitMatrix(2);
            bitMatrix[0][0] = true;
            bitMatrix[1][1] = true;
            bitMatrix.TransitiveClosure();

            Assert.IsTrue(bitMatrix[0][0]);
            Assert.IsFalse(bitMatrix[0][1]);
            Assert.IsFalse(bitMatrix[1][0]);
            Assert.IsTrue(bitMatrix[1][1]);
        }

        [TestMethod]
        [Ignore]
        public void BitMatrixShouldComputeTransitiveClosureOfAycockHorspool()
        {
            var bitMatrix = new BitMatrix(12);
            //for (int i = 0; i < bitMatrix.Length; i++)
            //    bitMatrix[i][i] = true;
            bitMatrix[0][1] = true;
            bitMatrix[0][2] = true;
            bitMatrix[2][3] = true;
            bitMatrix[2][7] = true;
            bitMatrix[2][9] = true;
            bitMatrix[3][4] = true;
            bitMatrix[3][7] = true;
            bitMatrix[3][9] = true;
            bitMatrix[4][5] = true;
            bitMatrix[4][7] = true;
            bitMatrix[4][9] = true;
            bitMatrix[5][6] = true;
            bitMatrix[5][7] = true;
            bitMatrix[5][9] = true;
            bitMatrix[7][8] = true;
            bitMatrix[7][11] = true;
            bitMatrix[9][10] = true;
            bitMatrix[11][11] = true;
            bitMatrix.TransitiveClosure();
            Console.Write(bitMatrix);
            Assert.IsTrue(bitMatrix[11][0]);            
        }
    }
}
