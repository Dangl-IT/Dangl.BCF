using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using iabi.BCF.BCFv21;
using iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory;
using Xunit;

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
                CreatedContainer = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.ComponentSelection);
            }

            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BcFv21TestCaseData.COMPONENT_SELECTION_TEST_CASE_NAME, TestCaseResourceFactory.GetReadmeForV21(BcfTestCaseV21.ComponentSelection));
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
            foreach (var currentEntry in CreatedArchive.Entries)
            {
                if (currentEntry.FullName.Contains(".bcfp")
                    || currentEntry.FullName.Contains(".version")
                    || currentEntry.FullName.Contains(".bcf")
                    || currentEntry.FullName.Contains(".bcfv")
                    || currentEntry.FullName.Contains(".xsd"))
                {
                    using (StreamReader rdr = new StreamReader(currentEntry.Open()))
                    {
                        var text = rdr.ReadToEnd();
                        var xml = XElement.Parse(text);
                        // No exception no cry!
                    }
                }
            }
        }

        [Fact]
        public void VersionTagCorrect()
        {
            var expectedVersionId = "2.1";
            var expectedDetailedVersion = "2.1";
            var versionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
            var actualVersionId = versionXml.Attribute("VersionId").Value;
            var actualDetailedVersion = ((XText)((XElement)versionXml.FirstNode).FirstNode).Value;

            Assert.True(versionXml.Nodes().Count() == 1 && ((XElement)versionXml.FirstNode).Name.LocalName == "DetailedVersion");
            Assert.Equal(expectedVersionId, actualVersionId);
            Assert.Equal(expectedDetailedVersion, actualDetailedVersion);
        }

        [Fact]
        public void CorrectGuidCreated_InMarkup()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.COMPONENT_SELECTION_TOPIC_GUID + "/markup.bcf");
            var viewpointsElement = markupXml.Descendants().OfType<XElement>()
                .Where(element => element.Name.LocalName == "Viewpoints");
            Assert.Single(viewpointsElement);
            Assert.Equal(BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID, viewpointsElement.First().Attributes().First(attribute => attribute.Name.LocalName == "Guid").Value);
        }

        [Fact]
        public void CorrectGuidCreated_InVisinfo()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.COMPONENT_SELECTION_TOPIC_GUID + "/Viewpoint_" + BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID + ".bcfv");

            var guidAttribute = viewpointXml.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "Guid");
            Assert.NotNull(guidAttribute);
            Assert.Equal(BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID, guidAttribute.Value);
        }

        [Fact]
        public void ViewpointComponentDefaultVisiblity()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.COMPONENT_SELECTION_TOPIC_GUID + "/Viewpoint_" + BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID + ".bcfv");

            var components = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Components
            var defaultComponentsVisibilitySetting = components
                .Elements().FirstOrDefault(e => e.Name.LocalName == "Visibility")
                ?.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibility");
            Assert.NotNull(defaultComponentsVisibilitySetting);
            Assert.Equal(BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_COMPONENTS, bool.Parse(defaultComponentsVisibilitySetting.Value));
        }

        [Fact]
        public void ViewpointComponentDefaultVisiblitySpaceBoundaries()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.COMPONENT_SELECTION_TOPIC_GUID + "/Viewpoint_" + BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID + ".bcfv");

            var components = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Openings
            var defaultSpaceBoundariesVisibilitySetting = components
                .Elements().FirstOrDefault(e => e.Name.LocalName == "ViewSetupHints")
                ?.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "SpaceBoundariesVisible");
            Assert.NotNull(defaultSpaceBoundariesVisibilitySetting);
            Assert.Equal(BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_SPACE_BOUNDARIES, bool.Parse(defaultSpaceBoundariesVisibilitySetting.Value));
        }

        [Fact]
        public void ViewpointComponentDefaultVisiblitySpaces()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.COMPONENT_SELECTION_TOPIC_GUID + "/Viewpoint_" + BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID + ".bcfv");

            var components = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Spaces
            var defaultSpacesVisibilitySetting = components
                .Elements().FirstOrDefault(e => e.Name.LocalName == "ViewSetupHints")
                ?.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "SpacesVisible");
            Assert.NotNull(defaultSpacesVisibilitySetting);
            Assert.Equal(BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_SPACES, bool.Parse(defaultSpacesVisibilitySetting.Value));
        }

        [Fact]
        public void ViewpointComponentDefaultVisiblityOpenings()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv21TestCaseData.COMPONENT_SELECTION_TOPIC_GUID + "/Viewpoint_" + BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID + ".bcfv");

            var components = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Openings
            var defaultOpeningsVisibilitySetting = components
                .Elements().FirstOrDefault(e => e.Name.LocalName == "ViewSetupHints")
                ?.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "OpeningsVisible");
            Assert.NotNull(defaultOpeningsVisibilitySetting);
            Assert.Equal(BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_OPENINGS, bool.Parse(defaultOpeningsVisibilitySetting.Value));
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