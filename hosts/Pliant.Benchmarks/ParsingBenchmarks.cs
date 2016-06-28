using BenchmarkDotNet.Attributes;
using Pliant.Bnf;
using System;
using System.IO;

namespace Pliant.Benchmarks
{
    public class ParsingBenchmarks
    {
        IParseRunner parseRunner;
        string sampleBnf;

        [Setup]
        public void Setup()
        {
            var grammar = new BnfGrammar();
            var parseEngine = new ParseEngine(grammar);
            sampleBnf = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "AnsiC.bnf"));

            parseRunner = new ParseRunner(parseEngine, sampleBnf);
        }

        [Benchmark]
        public void Parse()
        {
            while (!parseRunner.EndOfStream() && parseRunner.Read()) { }
        }
    }
}