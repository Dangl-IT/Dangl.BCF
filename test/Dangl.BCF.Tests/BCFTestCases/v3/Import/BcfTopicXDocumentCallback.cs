using Xunit;
using Dangl.BCF.BCFv3;
using System.Linq;
using System.IO;

namespace Dangl.BCF.Tests.BCFTestCases.v3.Import
{
    public class BcfTopicXDocumentCallback
    {
        [Fact]
        public void CanGetBcfTopicViaXDocumentCallback()
        {
            using var bcfStream = new MemoryStream(TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.SingleInvisibleWall));
            var hasFound = false;
            BCFv3Container.ReadStream(bcfStream, (bcfTopic, xDocument) =>
            {
                Assert.NotNull(xDocument);
                Assert.NotNull(bcfTopic);
                var topicId = xDocument
                    .Root
                    .Elements()
                    .Single(t => t.Name.LocalName == "Topic")
                    .Attributes()
                    .Single(a => a.Name.LocalName == "Guid")
                    .Value;
                Assert.Equal(topicId, bcfTopic.Markup.Topic.Guid);
                hasFound = true;
            });
            Assert.True(hasFound);
        }
    }
}
