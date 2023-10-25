using AssParser.Lib;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AssParser
{
    [SimpleJob(RuntimeMoniker.Net70, baseline: true)]
    [SimpleJob(RuntimeMoniker.NativeAot70)]
    [RPlotExporter]
    public class ParserBenchmark
    {
        public AssSubtitleModel assfile;
        [GlobalSetup]
        public void setup()
        {
            assfile= Lib.AssParser.ParseAssFile(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass").Result;
        }
        [Benchmark]
        public void ParserBenchmarkTest()
        {
            Lib.AssParser.ParseAssFile(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass").Wait();
        }
        [Benchmark]
        public void ParserBenchmarkTest2()
        {
            var fonts = assfile.UsedFonts();
        }
    }
}
