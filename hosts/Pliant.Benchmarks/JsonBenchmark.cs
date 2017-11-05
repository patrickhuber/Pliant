using BenchmarkDotNet.Attributes;
using Pliant.Bnf;
using Pliant.Grammars;
using Pliant.Json;
using Pliant.Runtime;
using System;
using System.IO;

namespace Pliant.Benchmarks
{
    [Config(typeof(PliantBenchmarkConfig))]
    public class JsonBenchmark
    {
        string _json;
        IGrammar _grammar;

        [GlobalSetup]
        public void Setup()
        {
            _json = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "10000.json"));
            _grammar = new JsonGrammar();
        }

        [Benchmark]
        public bool ParseEngineParse()
        {
            return RunParse(new ParseEngine(_grammar), _json);
        }
        
        //[Benchmark]
        public bool DeterministicParseEngineParse()
        {
            return RunParse(new DeterministicParseEngine(_grammar), _json);
        }
        
        //[Benchmark]
        public bool MarpaParseEngineParse()
        {
            return RunParse(new MarpaParseEngine(_grammar), _json);
        }
        
        private bool RunParse(IParseEngine parseEngine, string input)
        {
            var parseRunner = new ParseRunner(parseEngine, input);

            return parseRunner.RunToEnd();
        }
    }
}