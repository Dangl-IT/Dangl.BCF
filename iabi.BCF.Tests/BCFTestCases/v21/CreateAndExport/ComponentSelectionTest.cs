using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using iabi.BCF.BCFv21;
using iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory;
using Xunit;

// TODO TEST FOR ALL TEST CASES THAT GENERATED VIEWPOINTS HAVE A CONSISTENT GUID

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport
{
    /*
     * Note: Here is "\n" used as line break instead of "\r\n" due to the
     * XML specification specifying "\n" as the line break character.
     *
     */

    public class ComponentSelectionTest
    {
        public BCFv21Container CreatedContainer;

        public ZipArchive CreatedArchive;

        public ComponentSelectionTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.ComponentSelection);
            }

            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFTestCaseData.ComponentSelection_TestCaseName, BCFTestCaseData.ComponentSelection_Readme);
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

        [Fact]
        public void VersionTagCorrect()
        {
            var ExpectedVersionId = "2.1";
            var ExpectedDetailedVersion = "2.1";
            var VersionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
            var ActualVersionId = VersionXml.Attribute("VersionId").Value;
            var ActualDetailedVersion = ((XText)((XElement)VersionXml.FirstNode).FirstNode).Value;

            Assert.True(VersionXml.Nodes().Count() == 1 && ((XElement)VersionXml.FirstNode).Name.LocalName == "DetailedVersion");
            Assert.Equal(ExpectedVersionId, ActualVersionId);
            Assert.Equal(ExpectedDetailedVersion, ActualDetailedVersion);
        }

        [Fact]
        public void CorrectGuidCreated_InMarkup()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.ComponentSelection_TopicGuid + "/markup.bcf");
            var viewpointsElement = markupXml.Descendants().OfType<XElement>()
                .Where(element => element.Name.LocalName == "Viewpoints");
            Assert.Equal(1, viewpointsElement.Count());
            Assert.Equal(BCFTestCaseData.ComponentSelection_ViewpointGuid, viewpointsElement.First().Attributes().First(attribute => attribute.Name.LocalName == "Guid").Value);
        }

        [Fact]
        public void CorrectGuidCreated_InVisinfo()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.ComponentSelection_TopicGuid + "/Viewpoint_" + BCFTestCaseData.ComponentSelection_ViewpointGuid + ".bcfv");

            var guidAttribute = viewpointXml.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "Guid");
            Assert.NotNull(guidAttribute);
            Assert.Equal(BCFTestCaseData.ComponentSelection_ViewpointGuid, guidAttribute.Value);
        }

        [Fact]
        public void ViewpointComponentDefaultVisiblityCorrect()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.ComponentSelection_TopicGuid + "/Viewpoint_" + BCFTestCaseData.ComponentSelection_ViewpointGuid + ".bcfv");

            var components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Components
            var defaultComponentsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityComponents");
            if (bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilityComponents)) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultComponentsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultComponentsVisibilitySetting);
                Assert.Equal(BCFTestCaseData.ComponentSelection_DefaultVisibilityComponents, defaultComponentsVisibilitySetting.Value);
            }

            // Default Visibility for Openings
            var defaultOpeningsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityOpenings");
            if (bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilityOpenings)) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultOpeningsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultOpeningsVisibilitySetting);
                Assert.Equal(BCFTestCaseData.ComponentSelection_DefaultVisibilityOpenings, defaultOpeningsVisibilitySetting.Value);
            }

            // Default Visibility for Spaces
            var defaultSpacesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaces");
            if (bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilitySpaces)) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpacesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpacesVisibilitySetting);
                Assert.Equal(BCFTestCaseData.ComponentSelection_DefaultVisibilitySpaces, defaultSpacesVisibilitySetting.Value);
            }

            // Default Visibility for Openings
            var defaultSpaceBoundariesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaceBoundaries");
            if (bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilitySpaceBoundaries)) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpaceBoundariesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpaceBoundariesVisibilitySetting);
                Assert.Equal(BCFTestCaseData.ComponentSelection_DefaultVisibilitySpaceBoundaries, defaultSpaceBoundariesVisibilitySetting.Value);
            }
        }

        [Fact]
        public void WriteReadAgainAndCompare()
        {
            using (var MemStream = new MemoryStream())
            {
                CreatedContainer.WriteStream(MemStream);
                MemStream.Position = 0;

                var ReadContainer = BCFv21Container.ReadStream(MemStream);

                var ReadMemStream = new MemoryStream();
                ReadContainer.WriteStream(ReadMemStream);
                var WrittenZipArchive = new ZipArchive(ReadMemStream);

                CompareTool.CompareContainers(CreatedContainer, ReadContainer, CreatedArchive, WrittenZipArchive);
            }
        }
    }
}