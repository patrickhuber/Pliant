using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Collections;

namespace Pliant.Tests.Unit.Collections
{
    /// <summary>
    /// Summary description for ProcessOnceQueue
    /// </summary>
    [TestClass]
    public class ProcessOnceQueueTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ProcessOnceQueueShouldOnlyProcessItemOnce()
        {
            var processOnceQueue = new ProcessOnceQueue<int>();
            processOnceQueue.Enqueue(1);
            processOnceQueue.Enqueue(1);
            processOnceQueue.Enqueue(2);
            Assert.AreEqual(2, processOnceQueue.Count);
        }       
    }
}
