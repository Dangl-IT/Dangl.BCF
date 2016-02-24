using iabi.BCF.BCFv2;
using iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport
{
    [TestClass]
    public class MinimumInformationTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        //public const string TopicGuid = "9898DE65-C0CE-414B-857E-1DF97FFAED8D";

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            CreatedContainer = BCFTestCases.CreateAndExport.Factory.BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.MinimumInformation);
            CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFTestCaseData.MinimumInformation_TestCaseName, BCFTestCaseData.MinimumInformation_Readme);
        }

        [TestMethod]
        public void ContainerPresent()
        {
            Assert.IsNotNull(CreatedContainer);
        }

        [TestMethod]
        public void ZipPresent()
        {
            Assert.IsNotNull(CreatedArchive);
        }

        [TestMethod]
        public void CheckIfFilesPresent()
        {
            var ExpectedFilesList = new string[] {
                BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf",
                "bcf.version"
            };

            foreach (var CurrentEntry in CreatedArchive.Entries)
            {
                if (!ExpectedFilesList.Contains(CurrentEntry.FullName))
                {
                    Assert.Fail("Zip Archive should not contain entry " + CurrentEntry.FullName);
                }
            }

            foreach (var ExpectedFile in ExpectedFilesList)
            {
                if (CreatedArchive.Entries.All(Curr => Curr.FullName != ExpectedFile))
                {
                    Assert.Fail("Did not find expected file in archive: " + ExpectedFile);
                }
            }
        }

        [TestMethod]
        public void VersionTagCorrect()
        {
            var ExpectedVersionId = "2.0";
            var ExpectedDetailedVersion = "2.0";
            var VersionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
            var ActualVersionId = VersionXml.Attribute("VersionId").Value;
            var ActualDetailedVersion = ((XText)((XElement)VersionXml.FirstNode).FirstNode).Value;

            Assert.IsTrue(VersionXml.Nodes().Count() == 1 && ((XElement)VersionXml.FirstNode).Name.LocalName == "DetailedVersion");
            Assert.AreEqual(ExpectedVersionId, ActualVersionId);
            Assert.AreEqual(ExpectedDetailedVersion, ActualDetailedVersion);
        }

        [TestMethod]
        public void CheckMarkupOnlyHasTopicElement()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf");
            Assert.IsTrue(MarkupXml.Nodes().Count() == 1 && MarkupXml.Nodes().OfType<XElement>().FirstOrDefault().Name.LocalName == "Topic");
        }

        [TestMethod]
        public void CheckTopicHasThreeSubElementsOfTypeXText()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;

            var ChildNodesCount = TopicXml.Nodes().Count();
            var ChildrenWithOnlyText = TopicXml.Nodes().OfType<XElement>().Where(Curr => Curr.Nodes().Count() == 1 && Curr.Nodes().OfType<XText>().Any()).Count();

            Assert.AreEqual(ChildNodesCount, ChildrenWithOnlyText);
        }

        [TestMethod]
        public void TitleSet()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;
            var TitleXml = TopicXml.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Title");

            var Expected = "Minimum information BCFZip topic.";
            var Actual = (TitleXml.FirstNode as XText).Value;

            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CreationDateSet()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;
            var CreationDateXml = TopicXml.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "CreationDate");

            var Expected = "2015-07-15T13:12:42Z";
            var Actual = (CreationDateXml.FirstNode as XText).Value;

            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CreationAuthorSet()
        {
            var TopicXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.MinimumInformation_TopicGuid + "/markup.bcf").FirstNode as XElement;
            var AuthorXml = TopicXml.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "CreationAuthor");

            var Expected = "Developer@example.com";
            var Actual = (AuthorXml.FirstNode as XText).Value;

            Assert.AreEqual(Expected, Actual);
        }
    }
}