using iabi.BCF.BCFv2;
using iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport
{
    [TestClass]
    public class InternalBIMSnippetTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            CreatedContainer = BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.InternalBIMSnippet);
            CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFTestCaseData.InternalBIMSnippet_TestCaseName, BCFTestCaseData.InternalBIMSnippet_Readme);
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

        public string[] ExpectedFiles
        {
            get
            {
                return new string[]
                {
                    "bcf.version",
                    BCFTestCaseData.InternalBIMSnippet_TopicGuid + "/markup.bcf",
                    BCFTestCaseData.InternalBIMSnippet_TopicGuid + "/JsonElement.json", // Data element of the BIM Snippet
                };
            }
        }

        [TestMethod]
        public void CheckIfFilesPresent()
        {
            foreach (var ExpectedFile in ExpectedFiles)
            {
                if (CreatedArchive.Entries.All(Curr => Curr.FullName != ExpectedFile))
                {
                    Assert.Fail("Did not find expected file in archive: " + ExpectedFile);
                }
            }
        }

        [TestMethod]
        public void CheckIfNoAdditionalFilesPresent()
        {
            foreach (var CurrentEntry in CreatedArchive.Entries)
            {
                if (!ExpectedFiles.Contains(CurrentEntry.FullName))
                {
                    Assert.Fail("Zip Archive should not contain entry " + CurrentEntry.FullName);
                }
            }
        }

        [TestMethod]
        public void CheckIfFilesAreAllValidXml()
        {
            foreach (var CurrentEntry in CreatedArchive.Entries)
            {
                if (CurrentEntry.FullName.Contains(".bcfp")
                    || CurrentEntry.FullName.Contains(".version")
                    || CurrentEntry.FullName.Contains(".bcf")
                    || CurrentEntry.FullName.Contains(".bcfv")
                    || CurrentEntry.FullName.Contains(".xsd"))
                {
                    using (StreamReader Rdr = new StreamReader(CurrentEntry.Open()))
                    {
                        var Text = Rdr.ReadToEnd();
                        var Xml = XElement.Parse(Text);
                        // No exception no cry!
                    }
                }
            }
        }

        [TestMethod]
        public void CheckIfFileDataIsEqual_JsonElement()
        {
            var DataExpected = BCFTestCaseData.JsonElement;
            using (MemoryStream MemStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFTestCaseData.InternalBIMSnippet_TopicGuid + "/JsonElement.json").Open().CopyTo(MemStream);
                var DataActual = MemStream.ToArray();
                Assert.IsTrue(DataExpected.SequenceEqual(DataActual));
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
        public void VerifySnippetPresent()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.InternalBIMSnippet_TopicGuid + "/markup.bcf");
            var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault() as XElement;
            Assert.IsNotNull(SnippetXml);
        }

        [TestMethod]
        public void VerifySnippetIsNotExternal()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.InternalBIMSnippet_TopicGuid + "/markup.bcf");
            var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault() as XElement;
            // Null means false by default
            if (SnippetXml.Attribute("isExternal") != null)
            {
                var Expected = "false";
                var Actual = SnippetXml.Attribute("isExternal").Value;
                Assert.AreEqual(Expected, Actual);
            }
        }

        [TestMethod]
        public void WriteReadAgainAndCompare()
        {
            using (var MemStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(MemStream);
                MemStream.Position = 0;

                var ReadContainer = BCFv2Container.ReadStream(MemStream);

                var ReadMemStream = new MemoryStream();
                ReadContainer.WriteStream(ReadMemStream);
                var WrittenZipArchive = new ZipArchive(ReadMemStream);

                CompareTool.CompareContainers(CreatedContainer, ReadContainer, CreatedArchive, WrittenZipArchive);

                //TestUtilities.CompareBCFv2Container(CreatedContainer, ReadContainer);
            }
        }
    }
}