using Pliant.Bnf;
using Pliant.Runtime;
using System;
using System.Diagnostics;
using System.IO;

namespace Pliant.PerfViewApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sampleBnf = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "AnsiC.bnf"));

            var grammar = new BnfGrammar();

            var stopwatch = new Stopwatch();
            for (long i = 0; i < 1000; i++)
            {
                stopwatch.Restart();
                var parseEngine = new ParseEngine(grammar);
                var parseRunner = new ParseRunner(parseEngine, sampleBnf);

                while (!parseRunner.EndOfStream() && parseRunner.Read()) { }

                var result = parseRunner.ParseEngine.IsAccepted();
                stopwatch.Stop();
                Console.WriteLine($"Pass {i} elapsed ms: {stopwatch.ElapsedMilliseconds}");
            }
        }
    }
}