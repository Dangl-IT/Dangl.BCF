using Xunit;
using Dangl.BCF.BCFv21;
using System.Linq;
using System.IO;

namespace Dangl.BCF.Tests.BCFTestCases.v21.Import
{
    public class BcfTopicXDocumentCallback
    {
        [Fact]
        public void CanGetBcfTopicViaXDocumentCallback()
        {
            using var bcfStream = new MemoryStream(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleInvisibleWall));
            var hasFound = false;
            BCFv21Container.ReadStream(bcfStream, (bcfTopic, xDocument) =>
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

        [Fact]
        public void CanWriteCustomInformationIntoBcfTopicDocument()
        {
            using var bcfStream = new MemoryStream(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleInvisibleWall));
            var bcfContainer = BCFv21Container.ReadStream(bcfStream);
            using var memoryStream = new MemoryStream();
            var hasCalled = false;
            bcfContainer.WriteStream(memoryStream, (bcfTopix, xDocument) =>
            {
                hasCalled = true;
                xDocument.Root.Elements().First()
                .AddBeforeSelf(new object[]
                {
                    new System.Xml.Linq.XElement("CustomData",
                        new System.Xml.Linq.XAttribute("Name", "Test"),
                        new System.Xml.Linq.XAttribute("Value", "TestValue"))
                });
            });

            Assert.True(hasCalled);

            using var zipArchive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Read);
            var topicEntry = zipArchive.Entries.First(e => e.Name.EndsWith("markup.bcf"));
            using var topicEntryStream = topicEntry.Open();
            var topicDocument = System.Xml.Linq.XDocument.Load(topicEntryStream);
            var customData = topicDocument
                .Root
                .Elements()
                .Single(t => t.Name.LocalName == "CustomData");
            Assert.NotNull(customData);
            Assert.Equal("Test", customData.Attribute("Name").Value);
            Assert.Equal("TestValue", customData.Attribute("Value").Value);
        }
    }
}
