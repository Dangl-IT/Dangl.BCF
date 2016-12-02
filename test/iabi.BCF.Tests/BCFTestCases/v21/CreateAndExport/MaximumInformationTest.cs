using System;
using System.Globalization;
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


    public class MaximumInformationTest
    {
        public static BCFv21Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        public MaximumInformationTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.MaximumInformation);
            }

            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFv21TestCaseData.MaximumInformation_TestCaseName, TestCaseResourceFactory.GetReadmeForV21(BcfTestCaseV21.MaximumInformation));
            }
        }

        public string[] ExpectedFiles
        {
            get
            {
                return new[]
                {
                    "project.bcfp",
                    "extensions.xsd",
                    "bcf.version",
                    "markup.xsd", // Schema for markup file, is referenced in a topic as document reference
                    "IfcPile_01.ifc", // File attachment in the root folder
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf",
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv",
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".bcfv",
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".bcfv",
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".png",
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".png",
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".png",
                    BCFv21TestCaseData.MaximumInformation_TopicGuid + "/JsonElement.json", // Data element of the BIM Snippet
                    BCFv21TestCaseData.MaximumInformation_ReferencedTopicGuid + "/markup.bcf" // Referenced topic
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
            foreach (var ExpectedFile in ExpectedFiles)
            {
                if (CreatedArchive.Entries.All(Curr => Curr.FullName != ExpectedFile))
                {
                    Assert.True(false, "Did not find expected file in archive: " + ExpectedFile);
                }
            }
        }

        [Fact]
        public void CheckIfNoAdditionalFilesPresent()
        {
            foreach (var CurrentEntry in CreatedArchive.Entries)
            {
                if (!ExpectedFiles.Contains(CurrentEntry.FullName))
                {
                    Assert.True(false, "Zip Archive should not contain entry " + CurrentEntry.FullName);
                }
            }
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
                    using (var Rdr = new StreamReader(CurrentEntry.Open()))
                    {
                        var Text = Rdr.ReadToEnd();
                        var Xml = XElement.Parse(Text);
                        // No exception no cry!
                    }
                }
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_JsonElement()
        {
            var DataExpected = TestCaseResourceFactory.GetFileAttachment(FileAttachments.JsonElement);
            using (var MemStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFv21TestCaseData.MaximumInformation_TopicGuid + "/JsonElement.json").Open().CopyTo(MemStream);
                var DataActual = MemStream.ToArray();
                Assert.True(DataExpected.SequenceEqual(DataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_IfcPile()
        {
            var DataExpected = TestCaseResourceFactory.GetIfcFile(IfcFiles.IfcPile);
            using (var MemStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == "IfcPile_01.ifc").Open().CopyTo(MemStream);
                var DataActual = MemStream.ToArray();
                Assert.True(DataExpected.SequenceEqual(DataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_MarkupSchema()
        {
            var DataExpected = TestCaseResourceFactory.GetFileAttachment(FileAttachments.MarkupSchemaV21);
            using (var MemStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == "markup.xsd").Open().CopyTo(MemStream);
                var DataActual = MemStream.ToArray();
                Assert.True(DataExpected.SequenceEqual(DataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_Snapshot01()
        {
            var DataExpected = TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_01);
            using (var MemStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".png").Open().CopyTo(MemStream);
                var DataActual = MemStream.ToArray();
                Assert.True(DataExpected.SequenceEqual(DataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_Snapshot02()
        {
            var DataExpected = TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_02);
            using (var MemStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".png").Open().CopyTo(MemStream);
                var DataActual = MemStream.ToArray();
                Assert.True(DataExpected.SequenceEqual(DataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_Snapshot03()
        {
            var DataExpected = TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_03);
            using (var MemStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".png").Open().CopyTo(MemStream);
                var DataActual = MemStream.ToArray();
                Assert.True(DataExpected.SequenceEqual(DataActual));
            }
        }

        [Fact]
        public void VersionTagCorrect()
        {
            var ExpectedVersionId = "2.1";
            var ExpectedDetailedVersion = "2.1";
            var VersionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
            var ActualVersionId = VersionXml.Attribute("VersionId").Value;
            var ActualDetailedVersion = ((XText) ((XElement) VersionXml.FirstNode).FirstNode).Value;

            Assert.True(VersionXml.Nodes().Count() == 1 && ((XElement) VersionXml.FirstNode).Name.LocalName == "DetailedVersion");
            Assert.Equal(ExpectedVersionId, ActualVersionId);
            Assert.Equal(ExpectedDetailedVersion, ActualDetailedVersion);
        }

        [Fact]
        public void Viewpoint_01_InMarkupSet()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");

            var ViewpointElement = MarkupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoints" && Curr.Attribute("Guid").Value == BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01);

            Assert.NotNull(ViewpointElement);

            Assert.Equal(2, ViewpointElement.DescendantNodes().OfType<XElement>().Count());
            Assert.Equal(2, ViewpointElement.DescendantNodes().OfType<XText>().Count());

            var ViewpointReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoint");
            Assert.Equal("Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv", ViewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

            var SnapshotReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Snapshot");
            Assert.Equal("Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".png", SnapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        }

        [Fact]
        public void Viewpoint_02_InMarkupSet()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");

            var ViewpointElement = MarkupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoints" && Curr.Attribute("Guid").Value == BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02);

            Assert.NotNull(ViewpointElement);

            Assert.Equal(2, ViewpointElement.DescendantNodes().OfType<XElement>().Count());
            Assert.Equal(2, ViewpointElement.DescendantNodes().OfType<XText>().Count());

            var ViewpointReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoint");
            Assert.Equal("Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".bcfv", ViewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

            var SnapshotReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Snapshot");
            Assert.Equal("Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".png", SnapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        }

        [Fact]
        public void Viewpoint_03_InMarkupSet()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");

            var ViewpointElement = MarkupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoints" && Curr.Attribute("Guid").Value == BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03);

            Assert.NotNull(ViewpointElement);

            Assert.Equal(2, ViewpointElement.DescendantNodes().OfType<XElement>().Count());
            Assert.Equal(2, ViewpointElement.DescendantNodes().OfType<XText>().Count());

            var ViewpointReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoint");
            Assert.Equal("Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".bcfv", ViewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

            var SnapshotReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Snapshot");
            Assert.Equal("Snapshot_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".png", SnapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        }

        [Fact]
        public void Viewpoint_01_NoOrthogonalCameraSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv");
            Assert.False(ViewpointXml.DescendantNodes().OfType<XElement>().Any(Curr => Curr.Name.LocalName == "OrthogonalCamera"));
        }

        [Fact]
        public void Viewpoint_02_NoOrthogonalCameraSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".bcfv");
            Assert.False(ViewpointXml.DescendantNodes().OfType<XElement>().Any(Curr => Curr.Name.LocalName == "OrthogonalCamera"));
        }

        [Fact]
        public void Viewpoint_03_NoOrthogonalCameraSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".bcfv");
            Assert.False(ViewpointXml.DescendantNodes().OfType<XElement>().Any(Curr => Curr.Name.LocalName == "OrthogonalCamera"));
        }

        [Fact]
        public void Viewpoint_01_PerspectiveCameraSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv");
            var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
            Assert.NotNull(CameraXml);
        }

        [Fact]
        public void Viewpoint_02_PerspectiveCameraSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".bcfv");
            var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
            Assert.NotNull(CameraXml);
        }

        [Fact]
        public void Viewpoint_03_PerspectiveCameraSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".bcfv");
            var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
            Assert.NotNull(CameraXml);
        }

        [Fact]
        public void Viewpoint_01_ClippingPlanesSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv");
            var Planes = ViewpointXml.Descendants("ClippingPlanes").FirstOrDefault().Descendants("ClippingPlane").Select(Curr => new
            {
                Location = new
                {
                    x = Convert.ToDouble(Curr.Descendants("Location").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(Curr.Descendants("Location").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(Curr.Descendants("Location").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                Direction = new
                {
                    x = Convert.ToDouble(Curr.Descendants("Direction").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(Curr.Descendants("Direction").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(Curr.Descendants("Direction").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                }
            });

            var ExpectedPlanes = MaximumInformationTestCase.GetPlanes().ToList();
            var ActualPlanes = Planes.ToList();

            Assert.Equal(ExpectedPlanes.Count, ActualPlanes.Count);

            for (var i = 0; i < ExpectedPlanes.Count; i++)
            {
                // location
                Assert.Equal(ExpectedPlanes[i].Location.X, ActualPlanes[i].Location.x);
                Assert.Equal(ExpectedPlanes[i].Location.Y, ActualPlanes[i].Location.y);
                Assert.Equal(ExpectedPlanes[i].Location.Z, ActualPlanes[i].Location.z);

                //direction
                Assert.Equal(ExpectedPlanes[i].Direction.X, ActualPlanes[i].Direction.x);
                Assert.Equal(ExpectedPlanes[i].Direction.Y, ActualPlanes[i].Direction.y);
                Assert.Equal(ExpectedPlanes[i].Direction.Z, ActualPlanes[i].Direction.z);
            }
        }

        [Fact]
        public void Viewpoint_01_LinesSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv");

            var Lines = ViewpointXml.Descendants("Lines").FirstOrDefault().Descendants("Line").Select(Curr => new
            {
                StartPoint = new
                {
                    x = Convert.ToDouble(Curr.Descendants("StartPoint").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(Curr.Descendants("StartPoint").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(Curr.Descendants("StartPoint").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                EndPoint = new
                {
                    x = Convert.ToDouble(Curr.Descendants("EndPoint").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(Curr.Descendants("EndPoint").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(Curr.Descendants("EndPoint").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                }
            });

            var ExpectedLines = MaximumInformationTestCase.GetLines().ToList();
            var ActualLines = Lines.ToList();

            Assert.Equal(ExpectedLines.Count, ActualLines.Count);

            for (var i = 0; i < ExpectedLines.Count; i++)
            {
                // start
                Assert.Equal(ExpectedLines[i].StartPoint.X, ActualLines[i].StartPoint.x);
                Assert.Equal(ExpectedLines[i].StartPoint.Y, ActualLines[i].StartPoint.y);
                Assert.Equal(ExpectedLines[i].StartPoint.Z, ActualLines[i].StartPoint.z);

                //end
                Assert.Equal(ExpectedLines[i].EndPoint.X, ActualLines[i].EndPoint.x);
                Assert.Equal(ExpectedLines[i].EndPoint.Y, ActualLines[i].EndPoint.y);
                Assert.Equal(ExpectedLines[i].EndPoint.Z, ActualLines[i].EndPoint.z);
            }
        }

        [Fact]
        public void Viewpoint_01_ComponentDefaultValues()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv");

            var components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Components
            var defaultComponentsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityComponents");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilityComponents) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultComponentsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultComponentsVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilityComponents, bool.Parse(defaultComponentsVisibilitySetting.Value));
            }

            // Default Visibility for Openings
            var defaultOpeningsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityOpenings");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilityOpenings) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultOpeningsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultOpeningsVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilityOpenings, bool.Parse(defaultOpeningsVisibilitySetting.Value));
            }

            // Default Visibility for Spaces
            var defaultSpacesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaces");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilitySpaces) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpacesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpacesVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilitySpaces, bool.Parse(defaultSpacesVisibilitySetting.Value));
            }

            // Default Visibility for Openings
            var defaultSpaceBoundariesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaceBoundaries");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilitySpaceBoundaries) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpaceBoundariesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpaceBoundariesVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilitySpaceBoundaries, bool.Parse(defaultSpaceBoundariesVisibilitySetting.Value));
            }
        }

        [Fact]
        public void Viewpoint_01_ComponentsSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv");

            var Components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Components")
                .Nodes().OfType<XElement>().Where(Curr => Curr.Name.LocalName == "Component")
                .Select(Curr => new
                {
                    IfcGuid = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "IfcGuid").Value,
                    Visible = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible").Value),
                    Selected = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected").Value)
                }).ToList();

            // Count of components
            Assert.Equal(7, Components.Count);

            // #1
            Assert.Equal("0Gl71cVurFn8bxAOox6M4X", Components[0].IfcGuid);
            Assert.Equal(true, Components[0].Selected);

            // #2
            Assert.Equal("23Zwlpd71EyvHlH6OZ77nK", Components[1].IfcGuid);
            Assert.Equal(true, Components[1].Selected);

            // #3
            Assert.Equal("3DvyPxGIn8qR0KDwbL_9r1", Components[2].IfcGuid);
            Assert.Equal(true, Components[2].Selected);

            // #4
            Assert.Equal("0fdpeZZEX3FwJ7x0ox5kzF", Components[3].IfcGuid);
            Assert.Equal(true, Components[3].Selected);

            // #5
            Assert.Equal("1OpjQ1Nlv4sQuTxfUC_8zS", Components[4].IfcGuid);
            Assert.Equal(true, Components[3].Selected);

            // #6
            Assert.Equal("0cSRUx$EX1NRjqiKcYQ$a0", Components[5].IfcGuid);
            Assert.Equal(true, Components[3].Selected);

            // #7
            Assert.Equal("1jQQiGIAnFzxOUzrdmJYDS", Components[6].IfcGuid);
            Assert.Equal(true, Components[3].Selected);
        }

        [Fact]
        public void Viewpoint_02_ComponentDefaultValues()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".bcfv");

            var components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Components
            var defaultComponentsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityComponents");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilityComponents) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultComponentsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultComponentsVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilityComponents, bool.Parse(defaultComponentsVisibilitySetting.Value));
            }

            // Default Visibility for Openings
            var defaultOpeningsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityOpenings");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilityOpenings) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultOpeningsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultOpeningsVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilityOpenings, bool.Parse(defaultOpeningsVisibilitySetting.Value));
            }

            // Default Visibility for Spaces
            var defaultSpacesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaces");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilitySpaces) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpacesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpacesVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilitySpaces, bool.Parse(defaultSpacesVisibilitySetting.Value));
            }

            // Default Visibility for Openings
            var defaultSpaceBoundariesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaceBoundaries");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilitySpaceBoundaries) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpaceBoundariesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpaceBoundariesVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilitySpaceBoundaries, bool.Parse(defaultSpaceBoundariesVisibilitySetting.Value));
            }
        }

        [Fact]
        public void Viewpoint_02_ComponentsSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".bcfv");

            var Components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Components")
                .Nodes().OfType<XElement>().Where(Curr => Curr.Name.LocalName == "Component")
                .Select(Curr => new
                {
                    IfcGuid = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "IfcGuid").Value,
                    Visible = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible").Value),
                    Selected = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected").Value)
                }).ToList();

            // Count of components
            Assert.Equal(4, Components.Count);

            // #1
            Assert.Equal("0fdpeZZEX3FwJ7x0ox5kzF", Components[0].IfcGuid);
            Assert.Equal(true, Components[0].Selected);

            // #2
            Assert.Equal("23Zwlpd71EyvHlH6OZ77nK", Components[1].IfcGuid);
            Assert.Equal(true, Components[1].Selected);

            // #3
            Assert.Equal("1OpjQ1Nlv4sQuTxfUC_8zS", Components[2].IfcGuid);
            Assert.Equal(true, Components[2].Selected);

            // #4
            Assert.Equal("0cSRUx$EX1NRjqiKcYQ$a0", Components[3].IfcGuid);
            Assert.Equal(true, Components[3].Selected);
        }

        [Fact]
        public void Viewpoint_03_ComponentDefaultValues()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".bcfv");

            var components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(element => element.Name.LocalName == "Components");

            // Default Visibility for Components
            var defaultComponentsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityComponents");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilityComponents) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultComponentsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultComponentsVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilityComponents, bool.Parse(defaultComponentsVisibilitySetting.Value));
            }

            // Default Visibility for Openings
            var defaultOpeningsVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilityOpenings");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilityOpenings) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultOpeningsVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultOpeningsVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilityOpenings, bool.Parse(defaultOpeningsVisibilitySetting.Value));
            }

            // Default Visibility for Spaces
            var defaultSpacesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaces");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilitySpaces) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpacesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpacesVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilitySpaces, bool.Parse(defaultSpacesVisibilitySetting.Value));
            }

            // Default Visibility for Openings
            var defaultSpaceBoundariesVisibilitySetting = components.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "DefaultVisibilitySpaceBoundaries");
            if (BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilitySpaceBoundaries) // true is the default value and therefore not serialized
            {
                Assert.Null(defaultSpaceBoundariesVisibilitySetting);
            }
            else
            {
                Assert.NotNull(defaultSpaceBoundariesVisibilitySetting);
                Assert.Equal(BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilitySpaceBoundaries, bool.Parse(defaultSpaceBoundariesVisibilitySetting.Value));
            }
        }

        [Fact]
        public void Viewpoint_03_ComponentsSet()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".bcfv");

            var Components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Components")
                .Nodes().OfType<XElement>().Where(Curr => Curr.Name.LocalName == "Component")
                .Select(Curr => new
                {
                    IfcGuid = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "IfcGuid").Value,
                    Visible = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible").Value),
                    Selected = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected").Value)
                }).ToList();

            // Count of components
            Assert.Equal(4, Components.Count);

            // #1
            Assert.Equal("0fdpeZZEX3FwJ7x0ox5kzF", Components[0].IfcGuid);
            Assert.Equal(false, Components[0].Visible);

            // #2
            Assert.Equal("23Zwlpd71EyvHlH6OZ77nK", Components[1].IfcGuid);
            Assert.Equal(false, Components[1].Visible);

            // #3
            Assert.Equal("1OpjQ1Nlv4sQuTxfUC_8zS", Components[2].IfcGuid);
            Assert.Equal(false, Components[2].Visible);

            // #4
            Assert.Equal("0cSRUx$EX1NRjqiKcYQ$a0", Components[3].IfcGuid);
            Assert.Equal(false, Components[3].Visible);
        }

        [Fact]
        public void Viewpoint_01_PerspectiveCameraCorrect()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01 + ".bcfv");
            var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
            var CameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(CameraXml);
            var CameraExpected = MaximumInformationTestCase.GetCamera_01();
            TopicsCompareTool.ComparePerspectiveCameras(CameraExpected, CameraActual);
        }

        [Fact]
        public void Viewpoint_02_PerspectiveCameraCorrect()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02 + ".bcfv");
            var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
            var CameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(CameraXml);
            var CameraExpected = MaximumInformationTestCase.GetCamera_02();
            TopicsCompareTool.ComparePerspectiveCameras(CameraExpected, CameraActual);
        }

        [Fact]
        public void Viewpoint_03_PerspectiveCameraCorrect()
        {
            var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/Viewpoint_" + BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03 + ".bcfv");
            var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
            var CameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(CameraXml);
            var CameraExpected = MaximumInformationTestCase.GetCamera_03();
            TopicsCompareTool.ComparePerspectiveCameras(CameraExpected, CameraActual);
        }

        [Fact]
        public void VerifyCountOfComments()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            Assert.Equal(4, MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").Count());
        }

        [Fact]
        public void VerifyCommentCorrect_01()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            var CommentExpected = MaximumInformationTestCase.CreateComments().ToList()[0];
            var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[0]);
            TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        }

        [Fact]
        public void VerifyCommentCorrect_02()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            var CommentExpected = MaximumInformationTestCase.CreateComments().ToList()[1];
            var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[1]);
            TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        }

        [Fact]
        public void VerifyCommentCorrect_03()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            var CommentExpected = MaximumInformationTestCase.CreateComments().ToList()[2];
            var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[2]);
            TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        }

        [Fact]
        public void VerifyCommentCorrect_04()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            var CommentExpected = MaximumInformationTestCase.CreateComments().ToList()[3];
            var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[3]);
            TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        }

        [Fact]
        public void VerifySnippetPresent()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault();
            Assert.NotNull(SnippetXml);
        }

        [Fact]
        public void VerifySnippetDataPresent()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault();

            Assert.NotNull(SnippetXml.Descendants("Reference"));
            Assert.NotNull(SnippetXml.Descendants("ReferenceSchema"));
            Assert.NotNull(SnippetXml.Attribute("SnippetType"));

            // isExternal is false by default, so if the actual element points to an external snippet
            // the XML attribute may be omitted
            if (MaximumInformationTestCase.CreateTopic().Markup.Topic.BimSnippet.isExternal)
            {
                Assert.NotNull(SnippetXml.Attribute("isExternal"));
                Assert.Equal("true", SnippetXml.Attribute("isExternal").Value);
            }
            else
            {
                // But if it is false AND the attribute is present, it must be set to false
                if (SnippetXml.Attribute("isExternal") != null)
                {
                    Assert.Equal("false", SnippetXml.Attribute("isExternal").Value);
                }
            }
        }

        [Fact]
        public void VerifySnippetDataCorrect()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");
            var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault();

            var SnippetExpected = MaximumInformationTestCase.CreateTopic().Markup.Topic.BimSnippet;

            var SnippetActual = TestUtilities.GetBimSnippetFromXml(SnippetXml);
            TopicsCompareTool.CompareBimSnippet(SnippetExpected, SnippetActual);
        }

        [Fact]
        public void CheckIfFileInHeaderSetCorrectly()
        {
            var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFv21TestCaseData.MaximumInformation_TopicGuid + "/markup.bcf");

            // Only one header element
            Assert.True(MarkupXml.Descendants("Header").Count() == 1);

            var FileXml = MarkupXml.Descendants("File").FirstOrDefault();

            // Only one file element and no other elements
            Assert.True(MarkupXml.Descendants("File").Count() == 1);
            Assert.True(MarkupXml.Descendants("Header").FirstOrDefault().Nodes().Count() == 1);

            // File has exactly three descendants
            Assert.True(MarkupXml.Descendants("File").Descendants().Count() == 3);

            var IfcProjectExpected = "0M6o7Znnv7hxsbWgeu7oQq";
            var IfcSpatialStructElementExpected = "23B$bNeGHFQuMYJzvUX0FD";
            var IsExternalExpected = false;
            var FilenameExpected = "IfcPile_01.ifc";
            var DateExpected = "2014-10-27T16:27:27Z";
            var ReferenceExpected = "../IfcPile_01.ifc";

            var HeaderXml = MarkupXml.FirstNode as XElement;

            var IfcProjectActual = HeaderXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("IfcProject").Value;
            var IfcSpatialStructElementActual = HeaderXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("IfcSpatialStructureElement").Value;
            var IsExternalActual = bool.Parse(HeaderXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("isExternal").Value);
            var FilenameActual = HeaderXml.Descendants("Filename").OfType<XElement>().FirstOrDefault().Value;
            ;
            var DateActual = HeaderXml.Descendants("Date").OfType<XElement>().FirstOrDefault().Value;
            ;
            var ReferenceActual = HeaderXml.Descendants("Reference").OfType<XElement>().FirstOrDefault().Value;
            ;

            Assert.Equal(IfcProjectExpected, IfcProjectActual);
            Assert.Equal(IfcSpatialStructElementExpected, IfcSpatialStructElementActual);
            Assert.Equal(IsExternalExpected, IsExternalActual);
            Assert.Equal(FilenameExpected, FilenameActual);
            Assert.Equal(DateExpected, DateActual);
            Assert.Equal(ReferenceExpected, ReferenceActual);
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