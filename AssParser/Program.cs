using AssParser.Lib;
using BenchmarkDotNet.Running;
using System.Text;

namespace AssParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var assfile = Lib.AssParser.ParseAssFile(@"D:\Users\sunjialin\Source\Repos\AssParser\AssParser.Test\AssParserTest\1.ass").Result;
            var fonts = assfile.UsedFonts();
            foreach (var font in fonts)
            {
                Console.WriteLine(font.FontName+"\t"+font.UsedChar);
            }
            var txt = assfile.ToString();
            //var summary = BenchmarkRunner.Run<ParserBenchmark>();
            Console.ReadLine();
        }
    }
}
