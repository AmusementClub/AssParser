using BenchmarkDotNet.Running;
using System.Text;

namespace AssParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AssParser2 assParser = new();
            var ass = assParser.ParseAssFile2(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass").Result;
            Console.WriteLine();
            var summary = BenchmarkRunner.Run<ParserBenchmark>();
            Console.ReadLine();
        }
    }
}
