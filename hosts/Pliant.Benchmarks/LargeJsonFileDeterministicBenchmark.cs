using BenchmarkDotNet.Attributes;
using Pliant.Json;
using Pliant.Runtime;
using System;
using System.IO;
using Pliant.Grammars;

namespace Pliant.Benchmarks
{

    public class LargeJsonFileDeterministicBenchmark
    {
        string json;

        [Setup]
        public void Setup()
        {
            json = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "10000.json"));
        }

        [Benchmark]
        public bool Parse()
        {
            var grammar = new JsonGrammar();
            var parseEngine = new DeterministicParseEngine(new PreComputedGrammar(grammar));
            var parseRunner = new ParseRunner(parseEngine, json);

            while (!parseRunner.EndOfStream() && parseRunner.Read()) { }
            return parseRunner.ParseEngine.IsAccepted();
        }
    }
}