using BenchmarkDotNet.Running;
using System;
using System.IO;

namespace Pliant.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<AnsiCBnfBenchmark>();
            // var summary = BenchmarkRunner.Run<LargeJsonFileDeterministicBenchmark>();
            var summary = BenchmarkRunner.Run<LargeJsonFileBenchmark>();
        }
    }
}
