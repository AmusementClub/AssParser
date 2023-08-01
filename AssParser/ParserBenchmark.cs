using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssParser
{
    [SimpleJob(RuntimeMoniker.Net70, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21)]
    [SimpleJob(RuntimeMoniker.NativeAot70)]
    [RPlotExporter]
    public class ParserBenchmark
    {
        AssParser AssParser;
        AssParser2 AssParser2;
        [GlobalSetup]
        public void Setup()
        {
            AssParser = new AssParser();
            AssParser2 = new AssParser2();
        }

        public void AssParserB()
        {
            AssParser.ParseAssFile(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass");
        }

        [Benchmark]
        public void ParserBenchmark2()
        {
            AssParser2.ParseAssFile2(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass").Wait();
        }
    }
}
