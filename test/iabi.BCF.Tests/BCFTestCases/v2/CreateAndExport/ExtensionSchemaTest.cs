using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport
{
    public class ExtensionSchemaTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        public ExtensionSchemaTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.ExtensionSchema);
            }
            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFv2TestCaseData.ExtensionSchema_TestCaseName, TestCaseResourceFactory.GetReadmeForV2(BcfTestCaseV2.ExtensionSchema));
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
                BCFv2TestCaseData.ExtensionSchema_TopicGuid + "/markup.bcf",
                "project.bcfp",
                "extensions.xsd",
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
        public void CheckThatNoProjectWasWritten()
        {
            var ProjectEntry = CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == "project.bcfp");
            Assert.False(XmlUtilities.ElementNameInXml(ProjectEntry.Open(), "Project"));
        }

        [Fact]
        public void CheckThatExtensionsIsReferenced()
        {
            var ProjectXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "project.bcfp");
            Assert.True(ProjectXml.DescendantNodes().OfType<XElement>().Any(Curr =>
                Curr.Name.LocalName == "ExtensionSchema" &&
                Curr.DescendantNodes().Count() == 1
                && Curr.DescendantNodes().First().NodeType == XmlNodeType.Text
                && ((XText) Curr.DescendantNodes().First()).Value == "extensions.xsd"));
        }

        [Fact]
        public void CheckTopicTypes()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicType"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new[]
            {
                "Information",
                "Warning",
                "Error",
                "Request"
            };

            var AllTypesPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.True(AllTypesPresent);
            Assert.True(NothingSuperfluousPresent);
        }

        [Fact]
        public void CheckTopicStati()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicStatus"));
            var TopicStati = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var Values = new[]
            {
                "Open",
                "Closed",
                "Reopened"
            };

            var AllPresent = Values.All(Curr => TopicStati.Contains(Curr));
            var NothingSuperfluousPresent = TopicStati.All(Curr => Values.Contains(Curr));
            Assert.True(AllPresent);
            Assert.True(NothingSuperfluousPresent);
        }

        [Fact]
        public void CheckTopicLabels()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicLabel"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new[]
            {
                "Development",
                "Architecture",
                "MEP"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.True(AllPresent);
            Assert.True(NothingSuperfluousPresent);
        }

        [Fact]
        public void CheckSnippetTypes()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "SnippetType"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new[]
            {
                "IFC2X3",
                "IFC4",
                "JSON"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.True(AllPresent);
            Assert.True(NothingSuperfluousPresent);
        }

        [Fact]
        public void CheckPriority()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "Priority"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new[]
            {
                "Low",
                "Medium",
                "High"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.True(AllPresent);
            Assert.True(NothingSuperfluousPresent);
        }

        [Fact]
        public void CheckUserIdType()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "UserIdType"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new[]
            {
                "Architect@example.com",
                "MEPEngineer@example.com",
                "Developer@example.com"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.True(AllPresent);
            Assert.True(NothingSuperfluousPresent);
        }

        [Fact]
        public void ReadExtensionSchema()
        {
            using (var MemStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(MemStream);
                MemStream.Position = 0;

                var ReadContainer = BCFv2Container.ReadStream(MemStream);

                Assert.NotNull(ReadContainer.ProjectExtensions);
            }
        }

        [Fact]
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
            }
        }

        [Fact]
        public void CheckXmlBrandingCommentsArePresent()
        {
            using (var MemStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(MemStream);
                CompareTool.CheckBrandingCommentPresenceInEveryFile(MemStream.ToArray());
            }
        }
    }
}