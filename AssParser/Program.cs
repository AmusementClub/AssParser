using AssParser.Lib;
using BenchmarkDotNet.Running;
using System.Text;

namespace AssParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var assfile = Lib.AssParser.ParseAssFile(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass").Result;
            var fonts = assfile.UsedFonts();
            foreach (var font in fonts)
            {
                Console.WriteLine(font.FontName+"\t"+font.UsedChar);
            }
            var summary = BenchmarkRunner.Run<ParserBenchmark>();
            Console.ReadLine();
        }
    }
}
