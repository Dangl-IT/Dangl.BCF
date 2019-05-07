using iabi.BCF.Converter;
using Xunit;

namespace iabi.BCF.Tests.Converter
{
    public class ConversionExtensionsTests
    {
        [Fact]
        public void ReturnsNullRgbHexStringForNullByteArray()
        {
            var actual = ((byte[])null).ToRgbHexColorString();
            Assert.Null(actual);
        }

        [Fact]
        public void ReturnsNullRgbHexStringForEmptyByteArray()
        {
            var actual = new byte[0].ToRgbHexColorString();
            Assert.Null(actual);
        }

        [Fact]
        public void CanConvertByteArrayToRgbHexString()
        {
            var bytes = new byte[]
            {
                18,
                52,
                86
            };
            var actual = bytes.ToRgbHexColorString();
            Assert.Equal("123456", actual);
        }


        [Theory]
        [InlineData(null, true, null, null, null, null)]
        [InlineData("", true, null, null, null, null)]
        [InlineData("Hello World", true, null, null, null, null)]
        [InlineData("0", true, null, null, null, null)]
        [InlineData("00", true, null, null, null, null)]
        [InlineData("000", true, null, null, null, null)]
        [InlineData("0000", true, null, null, null, null)]
        [InlineData("00000", true, null, null, null, null)]
        [InlineData("0000000", true, null, null, null, null)]
        [InlineData("000000", false, 0, 0, 0, null)]
        [InlineData("00000000", false, 0, 0, 0, 0)]
        [InlineData("FFFFFF", false, 255, 255, 255, null)]
        [InlineData("FFFFFFFF", false, 255, 255, 255, 255)]
        [InlineData("12345678", false, 18, 52, 86, 120)]
        [InlineData("123456", false, 18, 52, 86, null)]
        public void ConvertsRgbHexStringToByteArray(string src, bool shouldReturnNull, int? expectedR, int? expectedG, int? expectedB, int? expectedAlpha)
        {
            var actual = src.ToByteArrayFromHexRgbColor();
            if (shouldReturnNull)
            {
                Assert.Null(actual);
            }
            else
            {
                if (expectedAlpha != null)
                {
                    Assert.Equal(expectedAlpha, actual[3]);
                    Assert.Equal(4, actual.Length);
                }
                else
                {
                    Assert.Equal(3, actual.Length);
                }

                Assert.Equal(expectedR, actual[0]);
                Assert.Equal(expectedG, actual[1]);
                Assert.Equal(expectedB, actual[2]);
            }
        }
    }
}
