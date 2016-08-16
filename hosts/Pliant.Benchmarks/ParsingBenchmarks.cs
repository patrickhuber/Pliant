using BenchmarkDotNet.Attributes;
using Pliant.Bnf;
using Pliant.Runtime;
using System;
using System.IO;

namespace Pliant.Benchmarks
{
    public class ParsingBenchmarks
    {
        string sampleBnf;

        [Setup]
        public void Setup()
        {
            sampleBnf = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "AnsiC.bnf"));
        }

        [Benchmark]
        public bool Parse()
        {
            var grammar = new BnfGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseRunner = new ParseRunner(parseEngine, sampleBnf);
            
            while (!parseRunner.EndOfStream() && parseRunner.Read()) { }
            return parseRunner.ParseEngine.IsAccepted();
        }
    }
}