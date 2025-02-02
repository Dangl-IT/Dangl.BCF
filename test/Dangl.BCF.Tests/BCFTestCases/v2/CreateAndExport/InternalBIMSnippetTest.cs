﻿using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Dangl.BCF.BCFv2;
using Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport
{
    public class InternalBimSnippetTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        public InternalBimSnippetTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.InternalBimSnippet);
            }
            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BcFv2TestCaseData.INTERNAL_BIM_SNIPPET_TEST_CASE_NAME, TestCaseResourceFactory.GetReadmeForV2(BcfTestCaseV2.InternalBIMSnippet));
            }
        }

        public string[] ExpectedFiles
        {
            get
            {
                return new[]
                {
                    "bcf.version",
                    BcFv2TestCaseData.INTERNAL_BIM_SNIPPET_TOPIC_GUID + "/markup.bcf",
                    BcFv2TestCaseData.INTERNAL_BIM_SNIPPET_TOPIC_GUID + "/JsonElement.json" // Data element of the BIM Snippet
                };
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
        public void CheckIfFileDataIsEqual_JsonElement()
        {
            var dataExpected = TestCaseResourceFactory.GetFileAttachment(FileAttachments.JsonElement);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == BcFv2TestCaseData.INTERNAL_BIM_SNIPPET_TOPIC_GUID + "/JsonElement.json").Open().CopyTo(memStream);
                var dataActual = memStream.ToArray();
                Assert.True(dataExpected.SequenceEqual(dataActual));
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
        public void VerifySnippetPresent()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.INTERNAL_BIM_SNIPPET_TOPIC_GUID + "/markup.bcf");
            var snippetXml = markupXml.Descendants("BimSnippet").FirstOrDefault();
            Assert.NotNull(snippetXml);
        }

        [Fact]
        public void VerifySnippetIsNotExternal()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.INTERNAL_BIM_SNIPPET_TOPIC_GUID + "/markup.bcf");
            var snippetXml = markupXml.Descendants("BimSnippet").FirstOrDefault();
            // Null means false by default
            if (snippetXml.Attribute("isExternal") != null)
            {
                var expected = "false";
                var actual = snippetXml.Attribute("isExternal").Value;
                Assert.Equal(expected, actual);
            }
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