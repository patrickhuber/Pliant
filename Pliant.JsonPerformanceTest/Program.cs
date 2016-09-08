using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Pliant.Grammars;
using Pliant.Json;
using Pliant.Runtime;

namespace Pliant.JsonPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            RunSingle();
            //RunTests();
        }

        private static void RunSingle()
        {
            var jsonStr = LoadJsonStr();
            var jsonGrammar = new JsonGrammar();
            var parseTester = new ParseTester(new DeterministicParseEngine(new PreComputedGrammar(jsonGrammar)));
            parseTester.RunParse(new StringReader(jsonStr));
        }

        private static void RunTests()
        {
            var jsonStr = LoadJsonStr();

            var sw = new Stopwatch();
            
            //Warmup a couple of times
            Warmup(1, sw, jsonStr);

            var perfStats = RunJsonTest(10, sw, jsonStr);

            var statStr = perfStats.ToString();

            Console.WriteLine("\n{0}\n\n", statStr);
            Console.Read();
        }

        private static string LoadJsonStr()
        {
            var jsonStr = "";
            using (var stream = File.OpenRead(".\\10000.json"))
            using (var reader = new StreamReader(stream))
            {
                jsonStr = reader.ReadToEnd();
            }
            return jsonStr;
        }

        private static void Warmup(int count, Stopwatch sw, string jsonStr)
        {
            for (var i = 0; i < count; ++i)
            {
                RunJsonTest(sw, new StringReader(jsonStr));
            }
        }

        private static PerfRunStatistics RunJsonTest(int numTimes, Stopwatch sw, string jsonStr)
        {
            var measuredTimes = new List<long>();

            for (var i = 0; i < numTimes; ++i)
            {
                var measuredTime = RunJsonTest(sw, new StringReader(jsonStr));
                measuredTimes.Add(measuredTime);
            }

            return new PerfRunStatistics(measuredTimes);
        }

        private static long RunJsonTest(Stopwatch sw, StringReader jsonStrReader)
        {
            var jsonGrammar = new JsonGrammar();
            var parseTester = new ParseTester(new DeterministicParseEngine(new PreComputedGrammar(jsonGrammar)));

            sw.Stop();
            sw.Reset();

            sw.Start();
            parseTester.RunParse(jsonStrReader);
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }
    }

    class PerfRunStatistics
    {
        public PerfRunStatistics(List<long> measuredTimes)
        {
            NumRuns = measuredTimes.Count;
            MeasuredTime = measuredTimes.ToArray();

            var sumMeasuredTime = measuredTimes.Sum(m => m);
            Average = sumMeasuredTime/NumRuns;

            Min = measuredTimes.Min();
            Max = measuredTimes.Max();
        }

        public int NumRuns { get; set; }

        public long[] MeasuredTime { get; set; }

        public long Average { get; set; }

        public long Min { get; set; }

        public long Max { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("Num runs: {0}\n", NumRuns);
            sb.AppendFormat("Average/Min/Max: {0}/{1}/{2}\n", Average, Min, Max);
            sb.AppendFormat("Measurements: \n");
            foreach (var m in MeasuredTime)
            {
                sb.AppendLine(m.ToString());
            }
            return sb.ToString();
        }
    }
}
