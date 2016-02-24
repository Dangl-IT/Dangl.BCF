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
    public class ExtensionSchemaTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            CreatedContainer = BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.ExtensionSchema);
            CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFTestCaseData.ExtensionSchema_TestCaseName, BCFTestCaseData.ExtensionSchema_Readme);
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
                BCFTestCaseData.ExtensionSchema_TopicGuid+"/markup.bcf",
                "project.bcfp",
                "extensions.xsd",
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
        public void CheckThatNoProjectWasWritten()
        {
            var ProjectEntry = CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == "project.bcfp");
            Assert.IsFalse(XmlUtilities.ElementNameInXml(ProjectEntry.Open(), "Project"));
        }

        [TestMethod]
        public void CheckThatExtensionsIsReferenced()
        {
            var ProjectXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "project.bcfp");
            Assert.IsTrue(ProjectXml.DescendantNodes().OfType<XElement>().Any(Curr =>
                Curr.Name.LocalName == "ExtensionSchema" &&
                Curr.DescendantNodes().Count() == 1
                && Curr.DescendantNodes().First().NodeType == System.Xml.XmlNodeType.Text
                && ((XText)Curr.DescendantNodes().First()).Value == "extensions.xsd"));
        }

        [TestMethod]
        public void CheckTopicTypes()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicType"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new string[]
            {
                "Information",
                "Warning",
                "Error",
                "Request"
            };

            var AllTypesPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.IsTrue(AllTypesPresent);
            Assert.IsTrue(NothingSuperfluousPresent);
        }

        [TestMethod]
        public void CheckTopicStati()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicStatus"));
            var TopicStati = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var Values = new string[]
            {
                "Open",
                "Closed",
                "Reopened"
            };

            var AllPresent = Values.All(Curr => TopicStati.Contains(Curr));
            var NothingSuperfluousPresent = TopicStati.All(Curr => Values.Contains(Curr));
            Assert.IsTrue(AllPresent);
            Assert.IsTrue(NothingSuperfluousPresent);
        }

        [TestMethod]
        public void CheckTopicLabels()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicLabel"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new string[]
            {
                "Development",
                "Architecture",
                "MEP"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.IsTrue(AllPresent);
            Assert.IsTrue(NothingSuperfluousPresent);
        }

        [TestMethod]
        public void CheckSnippetTypes()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "SnippetType"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new string[]
            {
                "IFC2X3",
                "IFC4",
                "JSON"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.IsTrue(AllPresent);
            Assert.IsTrue(NothingSuperfluousPresent);
        }

        [TestMethod]
        public void CheckPriority()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "Priority"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new string[]
            {
                "Low",
                "Medium",
                "High"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.IsTrue(AllPresent);
            Assert.IsTrue(NothingSuperfluousPresent);
        }

        [TestMethod]
        public void CheckUserIdType()
        {
            var ExtensionsXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "extensions.xsd");

            var RestrictionBaseElement = ExtensionsXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "UserIdType"));
            var Values = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value);
            var ExpectedValues = new string[]
            {
                "Architect@example.com",
                "MEPEngineer@example.com",
                "Developer@example.com"
            };

            var AllPresent = ExpectedValues.All(Curr => Values.Contains(Curr));
            var NothingSuperfluousPresent = Values.All(Curr => ExpectedValues.Contains(Curr));
            Assert.IsTrue(AllPresent);
            Assert.IsTrue(NothingSuperfluousPresent);
        }

        [TestMethod]
        public void ReadExtensionSchema()
        {
            using (var MemStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(MemStream);
                MemStream.Position = 0;

                var ReadContainer = BCFv2Container.ReadStream(MemStream);

                Assert.IsNotNull(ReadContainer.ProjectExtensions);
            }
        }

        [TestMethod]
        public void ReadWriteAndCompare()
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