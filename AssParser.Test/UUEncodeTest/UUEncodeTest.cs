using System.Text.RegularExpressions;
using AssParser.Lib;
using Xunit.Abstractions;

namespace AssParser.Test.UUEncodeTest
{
    public class UUEncodeTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string fontsDataCrlf;
        private readonly string fontsDataLf;
        
        public UUEncodeTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var assfile = Lib.AssParser.ParseAssFile(Path.Combine("UUEncodeTest", "1.ass")).Result;
            var fontsData = assfile.UnknownSections["[Fonts]"];
            fontsData = fontsData.Remove(0, fontsData.IndexOf("\n", StringComparison.Ordinal) + 1).Trim();

            fontsDataCrlf = Regex.Replace(fontsData, "\r?\n", "\r\n");
            fontsDataLf = Regex.Replace(fontsData, "\r?\n", "\n");
        }

        [Fact]
        public void UUDecode_ShouldBe_Same()
        {
            var ttf = File.ReadAllBytes(Path.Combine("UUEncodeTest", "FreeSans.ttf"));
            var data1 = UUEncode.Decode(fontsDataCrlf, out _);
            Assert.Equal(ttf, data1);
        }

        [Fact]
        public void UUEncode_ShouldBe_Same_Crlf()
        {
            var data1 = UUEncode.Decode(fontsDataCrlf, out var crlf);
            var encoded = UUEncode.Encode(data1, true, crlf);
            Assert.Equal(fontsDataCrlf, encoded);
        }
        
        [Fact]
        public void UUEncode_ShouldBe_Same_Lf()
        {
            var data1 = UUEncode.Decode(fontsDataLf, out var crlf);
            var encoded = UUEncode.Encode(data1, true, crlf);
            Assert.Equal(fontsDataLf, encoded);
        }

        [Fact]
        public void UUEncode_Encode_Coverage_Test()
        {
            Assert.Equal("-1", UUEncode.Encode("1"u8.ToArray()));
            Assert.Equal("-4%", UUEncode.Encode("11"u8.ToArray()));
            Assert.Equal("-4%R", UUEncode.Encode("111"u8.ToArray()));
        }
    }
}
