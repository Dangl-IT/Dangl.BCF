using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v21.Import
{
    public class SingleInvisibleWall
    {
        [Fact]
        public void CanRead()
        {
            var bcfContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.SingleInvisibleWall);
            Assert.NotNull(bcfContainer);
        }
    }
}