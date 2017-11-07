using Pliant.Json;
using Pliant.Runtime;
using System;
using System.IO;

namespace Pliant.PerfViewApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sampleBnf = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "10000.json"));

            var grammar = new JsonGrammar();

            var loopCount = 100;
            for (long i = 0; i < loopCount; i++)
            {
                Console.WriteLine($"Iteration {i} of {loopCount}");
                var parseEngine = new ParseEngine(grammar);
                var parseRunner = new ParseRunner(parseEngine, sampleBnf);

                while (!parseRunner.EndOfStream() && parseRunner.Read()) { }

                var result = parseRunner.ParseEngine.IsAccepted();
            }
        }
    }
}
