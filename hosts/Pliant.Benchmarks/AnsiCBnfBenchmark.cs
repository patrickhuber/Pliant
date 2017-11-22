using BenchmarkDotNet.Attributes;
using Pliant.Bnf;
using Pliant.Grammars;
using Pliant.Runtime;
using System;
using System.IO;

namespace Pliant.Benchmarks
{
    [Config(typeof(PliantBenchmarkConfig))]
    public class AnsiCBnfBenchmark
    {
        string _sampleBnf;
        IGrammar _grammar;

        [GlobalSetup]
        public void Setup()
        {
            _sampleBnf = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "AnsiC.bnf"));
            _grammar = new BnfGrammar();
        }

        [Benchmark]
        public bool Parse()
        {
            var parseEngine = new ParseEngine(_grammar);
            var parseRunner = new ParseRunner(parseEngine, _sampleBnf);

            return parseRunner.RunToEnd();
        }
    }
}