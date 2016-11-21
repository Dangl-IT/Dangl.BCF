using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport
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
                CreatedContainer = BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.MinimumInformation);
            }
            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFTestCaseData.MinimumInformation_TestCaseName, BCFTestCaseData.MinimumInformation_Readme);
            }
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
            var ExpectedFilesList = new[]
            {
                BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf",
                "bcf.version"
            };

            foreach (var CurrentEntry in CreatedArchive.Entries)
            {
                if (!ExpectedFilesList.Contains(CurrentEntry.FullName))
                {
                    Assert.True(false, "Zip Archive should not contain entry " + CurrentEntry.FullName);
                }
            }

            foreach (var ExpectedFile in ExpectedFilesList)
            {
                if (CreatedArchive.Entries.All(Curr => Curr.FullName != ExpectedFile))
                {
                    Assert.True(false, "Did not find expected file in archive: " + ExpectedFile);
                }
            }
        }

        [Fact]
        public void VersionTagCorrect()
        {
            var ExpectedVersionId = "2.0";
            var ExpectedDetailedVersion = "2.0";
            var VersionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
            var ActualVersionId = VersionXml.Attribute("VersionId").Value;
            var ActualDetailedVersion = ((XText) ((XElement) VersionXml.FirstNode).FirstNode).Value;

            Assert.True(VersionXml.Nodes().Count() == 1 && ((XElement) VersionXml.FirstNode).Name.LocalName == "DetailedVersion");
            Assert.Equal(ExpectedVersionId, ActualVersionId);
            Assert.Equal(ExpectedDetailedVersion, ActualDetailedVersion);
        }

        [Fact]
        public void CheckMarkupOnlyHasTopicElement()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf");
            Assert.True(MarkupXml.Nodes().Count() == 1 && MarkupXml.Nodes().OfType<XElement>().FirstOrDefault().Name.LocalName == "Topic");
        }

        [Fact]
        public void CheckTopicHasThreeSubElementsOfTypeXText()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;

            var ChildNodesCount = TopicXml.Nodes().Count();
            var ChildrenWithOnlyText = TopicXml.Nodes().OfType<XElement>().Where(Curr => Curr.Nodes().Count() == 1 && Curr.Nodes().OfType<XText>().Any()).Count();

            Assert.Equal(ChildNodesCount, ChildrenWithOnlyText);
        }

        [Fact]
        public void TitleSet()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;
            var TitleXml = TopicXml.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Title");

            var Expected = "Minimum information BCFZip topic.";
            var Actual = (TitleXml.FirstNode as XText).Value;

            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CreationDateSet()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;
            var CreationDateXml = TopicXml.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "CreationDate");

            var Expected = "2015-07-15T13:12:42Z";
            var Actual = (CreationDateXml.FirstNode as XText).Value;

            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CreationAuthorSet()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;
            var AuthorXml = TopicXml.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "CreationAuthor");

            var Expected = "Developer@example.com";
            var Actual = (AuthorXml.FirstNode as XText).Value;

            Assert.Equal(Expected, Actual);
        }
    }
}