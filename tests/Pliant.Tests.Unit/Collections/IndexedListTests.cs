using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tests.Unit.Collections
{
    [TestClass]
    public class IndexedListTests
    {
        [TestMethod]
        public void IndexedListAddShouldAddToList()
        {
            var indexedList = new IndexedList<int>();
            indexedList.Add(0);
            Assert.AreEqual(1, indexedList.Count);
        }

        [TestMethod]
        public void IndexListInsertAtStartShouldShiftItemsToRight()
        {
            var indexedList = CreateListForRange(0, 1);
            indexedList.Insert(0, 2);
            Assert.AreEqual(0, indexedList.IndexOf(2));
            Assert.AreEqual(1, indexedList.IndexOf(0));
            Assert.AreEqual(2, indexedList.IndexOf(1));
        }

        [TestMethod]
        public void IndexedListInsertAtMiddleShouldShiftItemsRight()
        {
            var indexedList = CreateListForRange(0, 2);
            indexedList.Insert(1, 3);
            Assert.AreEqual(0, indexedList.IndexOf(0));
            Assert.AreEqual(1, indexedList.IndexOf(3));
            Assert.AreEqual(2, indexedList.IndexOf(1));
            Assert.AreEqual(3, indexedList.IndexOf(2));
        }

        [TestMethod]
        public void IndexedListInsertAtEndShouldNotShiftItems()
        {
            var indexedList = CreateListForRange(0, 2);
            indexedList.Insert(3, 3);
            for (var i = 0; i < indexedList.Count; i++)
                Assert.AreEqual(i, indexedList.IndexOf(i));
        }

        [TestMethod]
        public void IndexedListInsertDuplicateAtCurrentPositionShouldDoNothing()
        {
            var indexedList = CreateListForRange(0, 3);
            indexedList.Insert(0, 0);
            for (var i = 0; i <= 3; i++)
                Assert.AreEqual(i, indexedList.IndexOf(i));
        }

        [TestMethod]
        public void IndexedListInsertDuplicateAtBeginningPositionShouldShiftRight()
        {
            var indexedList = CreateListForRange(0, 3);
            indexedList.Insert(0, 3);
            Assert.AreEqual(0, indexedList.IndexOf(3));
            Assert.AreEqual(1, indexedList.IndexOf(0));
            Assert.AreEqual(2, indexedList.IndexOf(1));
            Assert.AreEqual(3, indexedList.IndexOf(2));
        }

        [TestMethod]
        public void IndexedListInsertDuplicateAtMiddlePositionShouldShiftRight()
        {
            var indexedList = CreateListForRange(0, 3);
            indexedList.Insert(1, 3);
            Assert.AreEqual(0, indexedList.IndexOf(0));
            Assert.AreEqual(1, indexedList.IndexOf(3));
            Assert.AreEqual(2, indexedList.IndexOf(1));
            Assert.AreEqual(3, indexedList.IndexOf(2));
        }

        [TestMethod]
        public void IndexedListInsertDuplicateAtEndShouldShiftLeft()
        {
            var indexedList = CreateListForRange(0, 3);
            indexedList.Insert(3, 0);
            Assert.AreEqual(0, indexedList.IndexOf(1));
            Assert.AreEqual(1, indexedList.IndexOf(2));
            Assert.AreEqual(2, indexedList.IndexOf(0));            
            Assert.AreEqual(3, indexedList.IndexOf(3));
        }

        private static IndexedList<int> CreateListForRange(int min, int max)
        {
            var indexedList = new IndexedList<int>();
            for (var i = min; i <= max; i++)
                indexedList.Add(i);
            return indexedList;
        }
    }
}
