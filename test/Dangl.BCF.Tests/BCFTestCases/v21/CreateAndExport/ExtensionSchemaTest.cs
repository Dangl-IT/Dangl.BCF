using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Dangl.BCF.BCFv21;
using Dangl.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v21.CreateAndExport
{
    public class ExtensionSchemaTest
    {
        public static BCFv21Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        public ExtensionSchemaTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.ExtensionSchema);
            }
            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BcFv21TestCaseData.EXTENSION_SCHEMA_TEST_CASE_NAME, TestCaseResourceFactory.GetReadmeForV21(BcfTestCaseV21.ExtensionSchema));
            }
        }

        [Fact]
        public void CanConverterToBcfV2Container()
        {
            var converter = new Dangl.BCF.Converter.V21ToV2(CreatedContainer);
            var downgradedContainer = converter.Convert();
            Assert.NotNull(downgradedContainer);
        }

        [Fact]
        public void CanConverterToBcfV3Container()
        {
            var converter = new Dangl.BCF.Converter.V21ToV3(CreatedContainer);
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
                BcFv21TestCaseData.EXTENSION_SCHEMA_TOPIC_GUID + "/markup.bcf",
                "project.bcfp",
                "extensions.xsd",
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
        public void CheckThatNoProjectWasWritten()
        {
            var projectEntry = CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == "project.bcfp");
            Assert.False(XmlUtilities.ElementNameInXml(projectEntry.Open(), "Project"));
        }

        [Fact]
        public void CheckThatExtensionsIsReferenced()
        {
            var projectXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "project.bcfp");
            Assert.Contains(projectXml.DescendantNodes().OfType<XElement>(), curr =>
                curr.Name.LocalName == "ExtensionSchema" &&
                curr.DescendantNodes().Count() == 1
                && curr.DescendantNodes().First().NodeType == XmlNodeType.Text
                && ((XText) curr.DescendantNodes().First()).Value == "extensions.xsd");
        }

        [Fact]
        public void CheckTopicTypes()
        {
            var extensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var restrictionBaseElement = extensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Attributes().Any(attr => attr.Name.LocalName == "name" && attr.Value == "TopicType"));
            var values = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(curr => curr.Attribute("value").Value);
            var expectedValues = new[]
            {
                "Information",
                "Warning",
                "Error",
                "Request"
            };

            var allTypesPresent = expectedValues.All(curr => values.Contains(curr));
            var nothingSuperfluousPresent = values.All(curr => expectedValues.Contains(curr));
            Assert.True(allTypesPresent);
            Assert.True(nothingSuperfluousPresent);
        }

        [Fact]
        public void CheckTopicStati()
        {
            var extensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var restrictionBaseElement = extensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Attributes().Any(attr => attr.Name.LocalName == "name" && attr.Value == "TopicStatus"));
            var topicStati = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(curr => curr.Attribute("value").Value);
            var values = new[]
            {
                "Open",
                "Closed",
                "Reopened"
            };

            var allPresent = values.All(curr => topicStati.Contains(curr));
            var nothingSuperfluousPresent = topicStati.All(curr => values.Contains(curr));
            Assert.True(allPresent);
            Assert.True(nothingSuperfluousPresent);
        }

        [Fact]
        public void CheckTopicLabels()
        {
            var extensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var restrictionBaseElement = extensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Attributes().Any(attr => attr.Name.LocalName == "name" && attr.Value == "TopicLabel"));
            var values = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(curr => curr.Attribute("value").Value);
            var expectedValues = new[]
            {
                "Development",
                "Architecture",
                "MEP"
            };

            var allPresent = expectedValues.All(curr => values.Contains(curr));
            var nothingSuperfluousPresent = values.All(curr => expectedValues.Contains(curr));
            Assert.True(allPresent);
            Assert.True(nothingSuperfluousPresent);
        }

        [Fact]
        public void CheckSnippetTypes()
        {
            var extensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var restrictionBaseElement = extensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Attributes().Any(attr => attr.Name.LocalName == "name" && attr.Value == "SnippetType"));
            var values = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(curr => curr.Attribute("value").Value);
            var expectedValues = new[]
            {
                "IFC2X3",
                "IFC4",
                "JSON"
            };

            var allPresent = expectedValues.All(curr => values.Contains(curr));
            var nothingSuperfluousPresent = values.All(curr => expectedValues.Contains(curr));
            Assert.True(allPresent);
            Assert.True(nothingSuperfluousPresent);
        }

        [Fact]
        public void CheckPriority()
        {
            var extensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var restrictionBaseElement = extensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Attributes().Any(attr => attr.Name.LocalName == "name" && attr.Value == "Priority"));
            var values = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(curr => curr.Attribute("value").Value);
            var expectedValues = new[]
            {
                "Low",
                "Medium",
                "High"
            };

            var allPresent = expectedValues.All(curr => values.Contains(curr));
            var nothingSuperfluousPresent = values.All(curr => expectedValues.Contains(curr));
            Assert.True(allPresent);
            Assert.True(nothingSuperfluousPresent);
        }

        [Fact]
        public void CheckUserIdType()
        {
            var extensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var restrictionBaseElement = extensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Attributes().Any(attr => attr.Name.LocalName == "name" && attr.Value == "UserIdType"));
            var values = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(curr => curr.Attribute("value").Value);
            var expectedValues = new[]
            {
                "Architect@example.com",
                "MEPEngineer@example.com",
                "Developer@example.com"
            };

            var allPresent = expectedValues.All(curr => values.Contains(curr));
            var nothingSuperfluousPresent = values.All(curr => expectedValues.Contains(curr));
            Assert.True(allPresent);
            Assert.True(nothingSuperfluousPresent);
        }

        [Fact]
        public void ReadExtensionSchema()
        {
            using (var memStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(memStream);
                memStream.Position = 0;

                var readContainer = BCFv21Container.ReadStream(memStream);

                Assert.NotNull(readContainer.ProjectExtensions);
            }
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