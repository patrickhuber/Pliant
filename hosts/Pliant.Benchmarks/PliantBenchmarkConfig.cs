using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Benchmarks
{
    public class PliantBenchmarkConfig : ManualConfig
    {
        public PliantBenchmarkConfig()
        {
            Add(new MemoryDiagnoser());
        }
    }
}
