using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Dangl.BCF.BCFv2;
using Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport
{
    public class MinimumInformationTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        //public const string TopicGuid = "9898DE65-C0CE-414B-857E-1DF97FFAED8D";

        public MinimumInformationTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.MinimumInformation);
            }
            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BcFv2TestCaseData.MINIMUM_INFORMATION_TEST_CASE_NAME, TestCaseResourceFactory.GetReadmeForV2(BcfTestCaseV2.MinimumInformation));
            }
        }

        [Fact]
        public void CanConverterToBcfV21Container()
        {
            var converter = new Dangl.BCF.Converter.V2ToV21(CreatedContainer);
            var upgradedContainer = converter.Convert();
            Assert.NotNull(upgradedContainer);
        }

        [Fact]
        public void ContainerPresent()
        {
            Assert.NotNull(CreatedContainer);
        }

        [Fact]
        public void ZipPresent()
        {
            Assert.NotNull(CreatedArchive);
        }

        [Fact]
        public void CheckIfFilesPresent()
        {
            var expectedFilesList = new[]
            {
                BcFv2TestCaseData.MINIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf",
                "bcf.version"
            };

            foreach (var currentEntry in CreatedArchive.Entries)
            {
                if (!expectedFilesList.Contains(currentEntry.FullName))
                {
                    Assert.True(false, "Zip Archive should not contain entry " + currentEntry.FullName);
                }
            }

            foreach (var expectedFile in expectedFilesList)
            {
                if (CreatedArchive.Entries.All(curr => curr.FullName != expectedFile))
                {
                    Assert.True(false, "Did not find expected file in archive: " + expectedFile);
                }
            }
        }

        [Fact]
        public void VersionTagCorrect()
        {
            var expectedVersionId = "2.0";
            var expectedDetailedVersion = "2.0";
            var versionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
            var actualVersionId = versionXml.Attribute("VersionId").Value;
            var actualDetailedVersion = ((XText) ((XElement) versionXml.FirstNode).FirstNode).Value;

            Assert.True(versionXml.Nodes().Count() == 1 && ((XElement) versionXml.FirstNode).Name.LocalName == "DetailedVersion");
            Assert.Equal(expectedVersionId, actualVersionId);
            Assert.Equal(expectedDetailedVersion, actualDetailedVersion);
        }

        [Fact]
        public void CheckMarkupOnlyHasTopicElement()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MINIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            Assert.True(markupXml.Nodes().Count() == 1 && markupXml.Nodes().OfType<XElement>().FirstOrDefault().Name.LocalName == "Topic");
        }

        [Fact]
        public void CheckTopicHasThreeSubElementsOfTypeXText()
        {
            var topicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MINIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf").FirstNode as XElement;

            var childNodesCount = topicXml.Nodes().Count();
            var childrenWithOnlyText = topicXml.Nodes().OfType<XElement>().Where(curr => curr.Nodes().Count() == 1 && curr.Nodes().OfType<XText>().Any()).Count();

            Assert.Equal(childNodesCount, childrenWithOnlyText);
        }

        [Fact]
        public void TitleSet()
        {
            var topicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MINIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf").FirstNode as XElement;
            var titleXml = topicXml.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Title");

            var expected = "Minimum information BCFZip topic.";
            var actual = (titleXml.FirstNode as XText).Value;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreationDateSet()
        {
            var topicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MINIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf").FirstNode as XElement;
            var creationDateXml = topicXml.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "CreationDate");

            var expected = "2015-07-15T13:12:42Z";
            var actual = (creationDateXml.FirstNode as XText).Value;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreationAuthorSet()
        {
            var topicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MINIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf").FirstNode as XElement;
            var authorXml = topicXml.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "CreationAuthor");

            var expected = "Developer@example.com";
            var actual = (authorXml.FirstNode as XText).Value;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WriteReadAgainAndCompare()
        {
            using (var memStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(memStream);
                memStream.Position = 0;

                var readContainer = BCFv2Container.ReadStream(memStream);

                var readMemStream = new MemoryStream();
                readContainer.WriteStream(readMemStream);
                var writtenZipArchive = new ZipArchive(readMemStream);

                CompareTool.CompareContainers(CreatedContainer, readContainer, CreatedArchive, writtenZipArchive);
            }
        }

        [Fact]
        public void CheckXmlBrandingCommentsArePresent()
        {
            using (var memStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(memStream);
                CompareTool.CheckBrandingCommentPresenceInEveryFile(memStream.ToArray());
            }
        }
    }
}