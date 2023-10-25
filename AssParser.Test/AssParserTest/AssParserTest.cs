using AssParser.Lib;

namespace AssParser.Test.AssParserTest
{
    public class AssParserTest
    {
        [Theory]
        [InlineData("1.ass")]
        [InlineData("2.ass")]
        public void AssParser_ShouldNot_Throw(string file)
        {
            //Arrange
            var path = Path.Combine("AssParserTest", file);

            //Act
            var exception = Record.ExceptionAsync(() => Lib.AssParser.ParseAssFile(path));

            //Assert
            Assert.Null(exception.Result);
        }
        [Theory]
        [InlineData("1.ass")]
        [InlineData("2.ass")]
        public void ToString_ShouldBe_Same(string file)
        {
            //Arrange
            var path = Path.Combine("AssParserTest", file);
            var source = File.ReadAllText(path);
            var assfile = Lib.AssParser.ParseAssFile(path).Result;

            //Act
            var res = assfile.ToString();

            //Assert
            Assert.Equal(source.Replace("\r\n", "\n").Replace("\n", "\r\n"), res.Replace("\r\n", "\n").Replace("\n", "\r\n"));
        }
        [Fact]
        public async void AssParser_ShouldThrow_InvalidStyle()
        {
            //Arrange
            var path = Path.Combine("AssParserTest", "format_14.ass");
            using var sr = new StreamReader(File.OpenRead(path));

            //Act
            var act = () => Lib.AssParser.ParseAssFile(sr);

            //Assert
            var exception = await Assert.ThrowsAsync<AssParserException>(act);
            Assert.Equal(14, exception?.LineCount);
            Assert.Equal(AssParserErrorType.InvalidStyleLine, exception?.ErrorType);
        }
    }
}
