using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.ProtocolBuffers.Tests.Unit
{
    [TestClass]
    public class ProtocolBuffersV3GrammarTests
    {
        [TestMethod]
        public void ProtocolBuffersV3GrammarShouldInitialize()
        {
            var grammar = new ProtocolBuffersV3Grammar();
        }
    }
}
