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
