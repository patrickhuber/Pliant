using System;
using BenchmarkDotNet.Running;

namespace Pliant.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<AnsiCBnfBenchmark>();
            BenchmarkRunner.Run<JsonBenchmark>();
        }
    }
}
