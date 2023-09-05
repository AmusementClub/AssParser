using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
