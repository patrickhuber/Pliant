using BenchmarkDotNet.Running;
using System;
using System.IO;

namespace Pliant.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<ParsingBenchmarks>();
            var summary = BenchmarkRunner.Run<LargeJsonFileBenchmarkDeterministic>();

            Console.Read();
        }
    }
}
