using AssParser.Lib;
using BenchmarkDotNet.Running;
using System.Text;

namespace AssParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var f = File.ReadAllLines(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass");
                //var body = f[40..];
                //var fs = new FileStream(@"C:\Users\sunjialin\Desktop\test.ass", FileMode.CreateNew);
                //var writer = new StreamWriter(fs);
                //foreach (var line in f)
                //{
                //    writer.WriteLine(line);
                //}
                //for (int i = 0; i < 10; i++)
                //{
                //    foreach (var item in body)
                //    {
                //        writer.WriteLine(item);
                //    }
                //}
                //fs.Close();
                var assfile = Lib.AssParser.ParseAssFile(@"[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass").Result;
                //var fonts = assfile.UsedFonts();
                //foreach (var font in fonts)
                //{
                //    Console.WriteLine(font.FontName + "\t" + font.UsedChar);
                //}
                //var txt = assfile.ToString();
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    // Handle the custom exception.
                    if (ex is AssParserException)
                    {
                        var assExcption = ex as AssParserException;
                        Console.WriteLine(assExcption?.ToString());
                    }
                    // Rethrow any other exception.
                    else
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
//#if !DEBUG
//            var summary = BenchmarkRunner.Run<ParserBenchmark>();
//#endif
            //Console.ReadLine();
        }
    }
}
