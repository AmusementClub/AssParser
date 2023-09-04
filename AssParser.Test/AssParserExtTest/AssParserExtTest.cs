using AssParser.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssParser.Test.AssParserExtTest
{
    public class AssParserExtTest
    {
        [Fact]
        public void UsedFonts_ShouldBe_Equivalent()
        {
            var truth = File.ReadAllLines(Path.Combine("AssParserExtTest", "FontsTest.txt"));
            var sortedTruth = new List<string>();
            foreach (var line in truth)
            {
                var parts = line.Split('\t');
                parts[3] = SortString(parts[3]);
                sortedTruth.Add(string.Join("\t", parts));
            }
            var assfile = Lib.AssParser.ParseAssFile(Path.Combine("AssParserExtTest", "FontsTest.ass")).Result;
            var fonts = assfile.UsedFonts();
            var res = new List<string>();
            foreach (var font in fonts)
            {
                res.Add(font.FontName + "\t" + font.Bold + "\t" + font.IsItalic + "\t" + SortString(font.UsedChar));
            }
            Assert.Equivalent(sortedTruth.ToHashSet(), res.ToHashSet());
        }
        private static string SortString(string s)
        {
            var sa = s.ToArray();
            Array.Sort(sa);
            return new(sa);
        }
    }
}
