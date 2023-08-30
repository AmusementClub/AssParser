using AssParser.Lib;

namespace AssParser.Test
{
    public class UUEncodeTest
    {
        [Fact]
        public void UUDecode_ShouldBe_Same()
        {
            var ttf = File.ReadAllBytes(Path.Combine("Resource", "FreeSans.ttf"));
            var assfile = Lib.AssParser.ParseAssFile(Path.Combine("Resource", "1.ass")).Result;
            var fontsdata = assfile.UnknownSections["[Fonts]"];
            fontsdata = fontsdata.Remove(0, fontsdata.IndexOf("\n", StringComparison.Ordinal) + 1).Trim();
            var data1 = UUEncode.Decode(fontsdata);
            var encoded = UUEncode.Encode(ttf, true, false);
            Assert.Equal(ttf, data1);
            Assert.Equal(fontsdata, encoded);
        }

        [Fact]
        public void UUEncode_ShouldBe_Same()
        {
            var assfile = Lib.AssParser.ParseAssFile(Path.Combine("Resource", "1.ass")).Result;
            var fontsdata = assfile.UnknownSections["[Fonts]"];
            fontsdata = fontsdata.Remove(0, fontsdata.IndexOf("\n", StringComparison.Ordinal) + 1).Trim();
            var data1 = UUEncode.Decode(fontsdata);
            var encoded = UUEncode.Encode(data1, true, false);
            Assert.Equal(fontsdata, encoded);
        }
    }
}
