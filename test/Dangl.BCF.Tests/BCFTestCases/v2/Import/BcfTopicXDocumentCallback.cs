using System.IO;
using System.Linq;
using Dangl.BCF.BCFv2;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v2.Import
{
    public class BcfTopicXDocumentCallback
    {
        [Fact]
        public void CanGetBcfTopicViaXDocumentCallback()
        {
            using var bcfStream = new MemoryStream(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleTopics));
            var hasFound = false;
            BCFv2Container.ReadStream(bcfStream, (bcfTopic, xDocument) =>
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