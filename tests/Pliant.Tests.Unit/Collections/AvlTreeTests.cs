using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Collections;
using System;

namespace Pliant.Tests.Unit.Collections
{
    [TestClass]
    public class AvlTreeTests
    {
        [TestMethod]
        public void AvlTreeShouldRebalanceLeftWhenInsertingIncrementalNumbers()
        {
            var avlTree = new AvlTree<int>();
            for (var i = 1; i <= 5; i++)
                avlTree.Insert(i);

            var index = 0;
            foreach (var value in avlTree)
            {
                index++;
                Assert.AreEqual(value, index);
            }

            Assert.AreEqual(5, index);
        }

        [TestMethod]
        public void AvlTreeShouldRebalanceRightWhenInsertingDecrementalNumbers()
        {
            var avlTree = new AvlTree<int>();
            for (var i = 5; i >= 1; i--)
                avlTree.Insert(i);

            var index = 0;
            foreach (var value in avlTree)
            {
                index++;
                Assert.AreEqual(value, index);
            }

            Assert.AreEqual(5, index);
        }

        [TestMethod]
        public void AvlTreeShouldRebalanceRightLeftWhenInsertingMinMaxMiddle()
        {
            var avlTree = new AvlTree<int>();
            avlTree.Insert(1);
            avlTree.Insert(3);
            avlTree.Insert(2);

            var index = 0;
            foreach (var value in avlTree)
            {
                index++;
                Assert.AreEqual(value, index);
            }
            Assert.AreEqual(3, index);
        }

        [TestMethod]
        public void AvlTreeShouldReblanceLeftRightWhenInsertingMaxMinMiddle()
        {
            var avlTree = new AvlTree<int>();
            avlTree.Insert(3);
            avlTree.Insert(1);
            avlTree.Insert(2);

            var index = 0;
            foreach (var value in avlTree)
            {
                index++;
                Assert.AreEqual(value, index);
            }
            Assert.AreEqual(3, index);
        }

        [TestMethod]
        public void AvlTreeShouldAcceptCharacters()
        {
            var avlTree = new AvlTree<char>();
            avlTree.Insert('a');
            avlTree.Insert('z');
            avlTree.Insert('0');
            avlTree.Insert('A');

            var min = char.MinValue;
            foreach (var value in avlTree)
            {
                Assert.IsTrue(min < value);
                min = value;
            }
        }
    }
}
