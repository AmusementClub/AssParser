using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssParser.Lib;

namespace AssParser
{
    [SimpleJob(RuntimeMoniker.Net70, baseline: true)]
    [SimpleJob(RuntimeMoniker.NativeAot70)]
    [RPlotExporter]
    public class ParserBenchmark
    {
        AssParser.Lib.AssParser AssParser;
        [GlobalSetup]
        public void Setup()
        {
            AssParser = new ();
        }
        [Benchmark]
        public void ParserBenchmark2()
        {
            AssParser.ParseAssFile2(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass").Wait();
        }
    }
}
