using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    [Ignore("Lasts too long. Execute locally on demand.")]
    public class DumpExtensionsBenchmark
    {
        [Benchmark]
        public string DumpNull() => DumpExtensions.Dump(null);

        [Benchmark]
        public string DumpInt32Type() => typeof(int).Dump();

        [Benchmark]
        public string DumpComplexType() => typeof(IDictionary<string, double>).Dump();

        [Test]
        public void Run() {
            BenchmarkRunner.Run<DumpExtensionsBenchmark>();
        }
    }
}