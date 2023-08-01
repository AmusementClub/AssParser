using BenchmarkDotNet.Running;
using System.Text;

namespace AssParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ParserBenchmark>();
            Console.ReadLine();
        }
    }
}
