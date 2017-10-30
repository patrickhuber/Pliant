using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;

namespace Pliant.Tests.Integration
{
    /// <summary>
    /// Summary description for NewtonsoftComparisonTests
    /// </summary>
    [TestClass]
    public class NewtonsoftComparisonTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void NewtonsoftCanParseLargeJsonFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                while (jsonTextReader.Read())
                {
                }
            }
        }
    }
}
