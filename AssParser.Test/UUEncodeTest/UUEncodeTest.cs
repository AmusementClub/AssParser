using AssParser.Lib;

namespace AssParser.Test.UUEncodeTest
{
    public class UUEncodeTest
    {
        readonly string fontsdata;
        public UUEncodeTest()
        {
            var assfile = Lib.AssParser.ParseAssFile(Path.Combine("UUEncodeTest", "1.ass")).Result;
            fontsdata = assfile.UnknownSections["[Fonts]"];
            fontsdata = fontsdata.Remove(0, fontsdata.IndexOf("\n", StringComparison.Ordinal) + 1).Trim();
        }

        [Fact]
        public void UUDecode_ShouldBe_Same()
        {
            var ttf = File.ReadAllBytes(Path.Combine("UUEncodeTest", "FreeSans.ttf"));
            var data1 = UUEncode.Decode(fontsdata);
            Assert.Equal(ttf, data1);
        }

        [Fact]
        public void UUEncode_ShouldBe_Same()
        {
            var data1 = UUEncode.Decode(fontsdata);
            var encoded = UUEncode.Encode(data1, true, true);
            Assert.Equal(fontsdata, encoded);
        }
    }
}
