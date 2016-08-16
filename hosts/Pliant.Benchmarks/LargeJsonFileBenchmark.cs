using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Benchmarks
{
    public class LargeJsonFileBenchmark
    {
        [Setup]
        public void Setup() { }

        [Benchmark]
        public void Parse() { }
    }
}
