using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using iabi.BCF.BCFv21;
using iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport
{
    public class PdfFileTest
    {
        public static BCFv21Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        public PdfFileTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.PdfFile);
            }
            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BcFv21TestCaseData.PDF_FILE_TEST_CASE_NAME, TestCaseResourceFactory.GetReadmeForV21(BcfTestCaseV21.PDFFile));
            }
        }

        public string[] ExpectedFiles
        {
            get
            {
                return new[]
                {
                    "bcf.version",
                    "Requirements.pdf", // Attached PDF file
                    BcFv21TestCaseData.PDF_FILE_TOPIC_GUID + "/markup.bcf"
                };
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
            foreach (var expectedFile in ExpectedFiles)
            {
                if (CreatedArchive.Entries.All(curr => curr.FullName != expectedFile))
                {
                    Assert.True(false, "Did not find expected file in archive: " + expectedFile);
                }
            }
        }

        [Fact]
        public void CheckIfNoAdditionalFilesPresent()
        {
            foreach (var currentEntry in CreatedArchive.Entries)
            {
                if (!ExpectedFiles.Contains(currentEntry.FullName))
                {
                    Assert.True(false, "Zip Archive should not contain entry " + currentEntry.FullName);
                }
            }
        }

        [Fact]
        public void CheckIfFilesAreAllValidXml()
        {
            foreach (var currentEntry in CreatedArchive.Entries)
            {
                if (currentEntry.FullName.Contains(".bcfp")
                    || currentEntry.FullName.Contains(".version")
                    || currentEntry.FullName.Contains(".bcf")
                    || currentEntry.FullName.Contains(".bcfv")
                    || currentEntry.FullName.Contains(".xsd"))
                {
                    using (var rdr = new StreamReader(currentEntry.Open()))
                    {
                        var text = rdr.ReadToEnd();
                        var xml = XElement.Parse(text);
                        // No exception no cry!
                    }
                }
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_PDFAttachment()
        {
            var dataExpected = TestCaseResourceFactory.GetFileAttachment(FileAttachments.RequirementsPdf);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == "Requirements.pdf").Open().CopyTo(memStream);
                var dataActual = memStream.ToArray();
                Assert.True(dataExpected.SequenceEqual(dataActual));
            }
        }

        [Fact]
        public void VersionTagCorrect()
        {
            var expectedVersionId = "2.1";
            var expectedDetailedVersion = "2.1";
            var versionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
            var actualVersionId = versionXml.Attribute("VersionId").Value;
            var actualDetailedVersion = ((XText) ((XElement) versionXml.FirstNode).FirstNode).Value;

            Assert.True(versionXml.Nodes().Count() == 1 && ((XElement) versionXml.FirstNode).Name.LocalName == "DetailedVersion");
            Assert.Equal(expectedVersionId, actualVersionId);
            Assert.Equal(expectedDetailedVersion, actualDetailedVersion);
        }

        [Fact]
        public void VerifySnippetNotPresent()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.PDF_FILE_TOPIC_GUID + "/markup.bcf");
            var snippetXml = markupXml.Descendants("BimSnippet").FirstOrDefault();
            Assert.Null(snippetXml);
        }

        [Fact]
        public void VeryfyDocumentReferenceSet()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.PDF_FILE_TOPIC_GUID + "/markup.bcf");

            var documentRefsXml = markupXml.Descendants("DocumentReference").First();

            Assert.NotNull(documentRefsXml);
            Assert.Equal("../Requirements.pdf", documentRefsXml.Descendants("ReferencedDocument").First().Value);
            Assert.Equal("Project requirements (pdf)", documentRefsXml.Descendants("Description").First().Value);
        }

        [Fact]
        public void WriteReadAgainAndCompare()
        {
            using (var memStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(memStream);
                memStream.Position = 0;

                var readContainer = BCFv21Container.ReadStream(memStream);

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