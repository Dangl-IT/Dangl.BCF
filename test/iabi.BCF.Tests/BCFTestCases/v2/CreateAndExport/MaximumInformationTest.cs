using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport
{
    /*
     * Note: Here is "\n" used as line break instead of "\r\n" due to the
     * XML specification specifying "\n" as the line break character.
     *
     */


    public class MaximumInformationTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        public MaximumInformationTest()
        {
            if (CreatedContainer == null)
            {
                CreatedContainer = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.MaximumInformation);
            }

            if (CreatedArchive == null)
            {
                CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BcFv2TestCaseData.MAXIMUM_INFORMATION_TEST_CASE_NAME, TestCaseResourceFactory.GetReadmeForV2(BcfTestCaseV2.MaximumInformation));
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
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf",
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv",
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".bcfv",
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".bcfv",
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".png",
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".png",
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".png",
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/JsonElement.json", // Data element of the BIM Snippet
                    BcFv2TestCaseData.MAXIMUM_INFORMATION_REFERENCED_TOPIC_GUID + "/markup.bcf" // Referenced topic
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
        public void CheckIfFileDataIsEqual_JsonElement()
        {
            var dataExpected = TestCaseResourceFactory.GetFileAttachment(FileAttachments.JsonElement);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/JsonElement.json").Open().CopyTo(memStream);
                var dataActual = memStream.ToArray();
                Assert.True(dataExpected.SequenceEqual(dataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_IfcPile()
        {
            var dataExpected = TestCaseResourceFactory.GetIfcFile(IfcFiles.IfcPile);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == "IfcPile_01.ifc").Open().CopyTo(memStream);
                var dataActual = memStream.ToArray();
                Assert.True(dataExpected.SequenceEqual(dataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_MarkupSchema()
        {
            var dataExpected = TestCaseResourceFactory.GetFileAttachment(FileAttachments.MarkupSchemaV2);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == "markup.xsd").Open().CopyTo(memStream);
                var dataActual = memStream.ToArray();
                Assert.True(dataExpected.SequenceEqual(dataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_Snapshot01()
        {
            var dataExpected = TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_01);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".png").Open().CopyTo(memStream);
                var dataActual = memStream.ToArray();
                Assert.True(dataExpected.SequenceEqual(dataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_Snapshot02()
        {
            var dataExpected = TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_02);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".png").Open().CopyTo(memStream);
                var dataActual = memStream.ToArray();
                Assert.True(dataExpected.SequenceEqual(dataActual));
            }
        }

        [Fact]
        public void CheckIfFileDataIsEqual_Snapshot03()
        {
            var dataExpected = TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_03);
            using (var memStream = new MemoryStream())
            {
                CreatedArchive.Entries.FirstOrDefault(curr => curr.FullName == BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".png").Open().CopyTo(memStream);
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
        public void Viewpoint_01_InMarkupSet()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");

            var viewpointElement = markupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Viewpoints" && curr.Attribute("Guid").Value == BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01);

            Assert.NotNull(viewpointElement);

            Assert.Equal(2, viewpointElement.DescendantNodes().OfType<XElement>().Count());
            Assert.Equal(2, viewpointElement.DescendantNodes().OfType<XText>().Count());

            var viewpointReferenceElement = viewpointElement.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Viewpoint");
            Assert.Equal("Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv", viewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

            var snapshotReferenceElement = viewpointElement.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Snapshot");
            Assert.Equal("Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".png", snapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        }

        [Fact]
        public void Viewpoint_02_InMarkupSet()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");

            var viewpointElement = markupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Viewpoints" && curr.Attribute("Guid").Value == BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02);

            Assert.NotNull(viewpointElement);

            Assert.Equal(2, viewpointElement.DescendantNodes().OfType<XElement>().Count());
            Assert.Equal(2, viewpointElement.DescendantNodes().OfType<XText>().Count());

            var viewpointReferenceElement = viewpointElement.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Viewpoint");
            Assert.Equal("Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".bcfv", viewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

            var snapshotReferenceElement = viewpointElement.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Snapshot");
            Assert.Equal("Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".png", snapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        }

        [Fact]
        public void Viewpoint_03_InMarkupSet()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");

            var viewpointElement = markupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Viewpoints" && curr.Attribute("Guid").Value == BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03);

            Assert.NotNull(viewpointElement);

            Assert.Equal(2, viewpointElement.DescendantNodes().OfType<XElement>().Count());
            Assert.Equal(2, viewpointElement.DescendantNodes().OfType<XText>().Count());

            var viewpointReferenceElement = viewpointElement.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Viewpoint");
            Assert.Equal("Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".bcfv", viewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

            var snapshotReferenceElement = viewpointElement.Nodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Snapshot");
            Assert.Equal("Snapshot_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".png", snapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        }

        [Fact]
        public void Viewpoint_01_NoOrthogonalCameraSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv");
            Assert.False(viewpointXml.DescendantNodes().OfType<XElement>().Any(curr => curr.Name.LocalName == "OrthogonalCamera"));
        }

        [Fact]
        public void Viewpoint_02_NoOrthogonalCameraSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".bcfv");
            Assert.False(viewpointXml.DescendantNodes().OfType<XElement>().Any(curr => curr.Name.LocalName == "OrthogonalCamera"));
        }

        [Fact]
        public void Viewpoint_03_NoOrthogonalCameraSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".bcfv");
            Assert.False(viewpointXml.DescendantNodes().OfType<XElement>().Any(curr => curr.Name.LocalName == "OrthogonalCamera"));
        }

        [Fact]
        public void Viewpoint_01_PerspectiveCameraSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv");
            var cameraXml = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "PerspectiveCamera");
            Assert.NotNull(cameraXml);
        }

        [Fact]
        public void Viewpoint_02_PerspectiveCameraSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".bcfv");
            var cameraXml = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "PerspectiveCamera");
            Assert.NotNull(cameraXml);
        }

        [Fact]
        public void Viewpoint_03_PerspectiveCameraSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".bcfv");
            var cameraXml = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "PerspectiveCamera");
            Assert.NotNull(cameraXml);
        }

        [Fact]
        public void Viewpoint_01_ClippingPlanesSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv");
            var planes = viewpointXml.Descendants("ClippingPlanes").FirstOrDefault().Descendants("ClippingPlane").Select(curr => new
            {
                Location = new
                {
                    x = Convert.ToDouble(curr.Descendants("Location").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(curr.Descendants("Location").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(curr.Descendants("Location").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                Direction = new
                {
                    x = Convert.ToDouble(curr.Descendants("Direction").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(curr.Descendants("Direction").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(curr.Descendants("Direction").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                }
            });

            var expectedPlanes = MaximumInformationTestCase.GetPlanes().ToList();
            var actualPlanes = planes.ToList();

            Assert.Equal(expectedPlanes.Count, actualPlanes.Count);

            for (var i = 0; i < expectedPlanes.Count; i++)
            {
                // location
                Assert.Equal(expectedPlanes[i].Location.X, actualPlanes[i].Location.x);
                Assert.Equal(expectedPlanes[i].Location.Y, actualPlanes[i].Location.y);
                Assert.Equal(expectedPlanes[i].Location.Z, actualPlanes[i].Location.z);

                //direction
                Assert.Equal(expectedPlanes[i].Direction.X, actualPlanes[i].Direction.x);
                Assert.Equal(expectedPlanes[i].Direction.Y, actualPlanes[i].Direction.y);
                Assert.Equal(expectedPlanes[i].Direction.Z, actualPlanes[i].Direction.z);
            }
        }

        [Fact]
        public void Viewpoint_01_LinesSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv");

            var lines = viewpointXml.Descendants("Lines").FirstOrDefault().Descendants("Line").Select(curr => new
            {
                StartPoint = new
                {
                    x = Convert.ToDouble(curr.Descendants("StartPoint").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(curr.Descendants("StartPoint").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(curr.Descendants("StartPoint").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                EndPoint = new
                {
                    x = Convert.ToDouble(curr.Descendants("EndPoint").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    y = Convert.ToDouble(curr.Descendants("EndPoint").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    z = Convert.ToDouble(curr.Descendants("EndPoint").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                }
            });

            var expectedLines = MaximumInformationTestCase.GetLines().ToList();
            var actualLines = lines.ToList();

            Assert.Equal(expectedLines.Count, actualLines.Count);

            for (var i = 0; i < expectedLines.Count; i++)
            {
                // start
                Assert.Equal(expectedLines[i].StartPoint.X, actualLines[i].StartPoint.x);
                Assert.Equal(expectedLines[i].StartPoint.Y, actualLines[i].StartPoint.y);
                Assert.Equal(expectedLines[i].StartPoint.Z, actualLines[i].StartPoint.z);

                //end
                Assert.Equal(expectedLines[i].EndPoint.X, actualLines[i].EndPoint.x);
                Assert.Equal(expectedLines[i].EndPoint.Y, actualLines[i].EndPoint.y);
                Assert.Equal(expectedLines[i].EndPoint.Z, actualLines[i].EndPoint.z);
            }
        }

        [Fact]
        public void Viewpoint_01_ComponentsSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv");

            var components = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Components")
                .Nodes().OfType<XElement>().Where(curr => curr.Name.LocalName == "Component")
                .Select(curr => new
                {
                    IfcGuid = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "IfcGuid").Value,
                    Visible = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Visible") == null ? false : bool.Parse(curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Visible").Value),
                    Selected = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Selected") == null ? false : bool.Parse(curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Selected").Value)
                }).ToList();

            // Count of components
            Assert.Equal(7, components.Count);

            // #1
            Assert.Equal("0Gl71cVurFn8bxAOox6M4X", components[0].IfcGuid);
            Assert.Equal(true, components[0].Selected);

            // #2
            Assert.Equal("23Zwlpd71EyvHlH6OZ77nK", components[1].IfcGuid);
            Assert.Equal(true, components[1].Selected);

            // #3
            Assert.Equal("3DvyPxGIn8qR0KDwbL_9r1", components[2].IfcGuid);
            Assert.Equal(true, components[2].Selected);

            // #4
            Assert.Equal("0fdpeZZEX3FwJ7x0ox5kzF", components[3].IfcGuid);
            Assert.Equal(true, components[3].Selected);

            // #5
            Assert.Equal("1OpjQ1Nlv4sQuTxfUC_8zS", components[4].IfcGuid);
            Assert.Equal(true, components[3].Selected);

            // #6
            Assert.Equal("0cSRUx$EX1NRjqiKcYQ$a0", components[5].IfcGuid);
            Assert.Equal(true, components[3].Selected);

            // #7
            Assert.Equal("1jQQiGIAnFzxOUzrdmJYDS", components[6].IfcGuid);
            Assert.Equal(true, components[3].Selected);
        }

        [Fact]
        public void Viewpoint_02_ComponentsSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".bcfv");

            var components = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Components")
                .Nodes().OfType<XElement>().Where(curr => curr.Name.LocalName == "Component")
                .Select(curr => new
                {
                    IfcGuid = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "IfcGuid").Value,
                    Visible = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Visible") == null ? false : bool.Parse(curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Visible").Value),
                    Selected = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Selected") == null ? false : bool.Parse(curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Selected").Value)
                }).ToList();

            // Count of components
            Assert.Equal(4, components.Count);

            // #1
            Assert.Equal("0fdpeZZEX3FwJ7x0ox5kzF", components[0].IfcGuid);
            Assert.Equal(true, components[0].Selected);

            // #2
            Assert.Equal("23Zwlpd71EyvHlH6OZ77nK", components[1].IfcGuid);
            Assert.Equal(true, components[1].Selected);

            // #3
            Assert.Equal("1OpjQ1Nlv4sQuTxfUC_8zS", components[2].IfcGuid);
            Assert.Equal(true, components[2].Selected);

            // #4
            Assert.Equal("0cSRUx$EX1NRjqiKcYQ$a0", components[3].IfcGuid);
            Assert.Equal(true, components[3].Selected);
        }

        [Fact]
        public void Viewpoint_03_ComponentsSet()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".bcfv");

            var components = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "Components")
                .Nodes().OfType<XElement>().Where(curr => curr.Name.LocalName == "Component")
                .Select(curr => new
                {
                    IfcGuid = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "IfcGuid").Value,
                    Visible = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Visible") == null ? false : bool.Parse(curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Visible").Value),
                    Selected = curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Selected") == null ? false : bool.Parse(curr.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Selected").Value)
                }).ToList();

            // Count of components
            Assert.Equal(4, components.Count);

            // #1
            Assert.Equal("0fdpeZZEX3FwJ7x0ox5kzF", components[0].IfcGuid);
            Assert.Equal(false, components[0].Visible);

            // #2
            Assert.Equal("23Zwlpd71EyvHlH6OZ77nK", components[1].IfcGuid);
            Assert.Equal(false, components[1].Visible);

            // #3
            Assert.Equal("1OpjQ1Nlv4sQuTxfUC_8zS", components[2].IfcGuid);
            Assert.Equal(false, components[2].Visible);

            // #4
            Assert.Equal("0cSRUx$EX1NRjqiKcYQ$a0", components[3].IfcGuid);
            Assert.Equal(false, components[3].Visible);
        }

        [Fact]
        public void Viewpoint_01_PerspectiveCameraCorrect()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01 + ".bcfv");
            var cameraXml = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "PerspectiveCamera");
            var cameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(cameraXml);
            var cameraExpected = MaximumInformationTestCase.GetCamera_01();
            TopicsCompareTool.ComparePerspectiveCameras(cameraExpected, cameraActual);
        }

        [Fact]
        public void Viewpoint_02_PerspectiveCameraCorrect()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02 + ".bcfv");
            var cameraXml = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "PerspectiveCamera");
            var cameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(cameraXml);
            var cameraExpected = MaximumInformationTestCase.GetCamera_02();
            TopicsCompareTool.ComparePerspectiveCameras(cameraExpected, cameraActual);
        }

        [Fact]
        public void Viewpoint_03_PerspectiveCameraCorrect()
        {
            var viewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/Viewpoint_" + BcFv2TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03 + ".bcfv");
            var cameraXml = viewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "PerspectiveCamera");
            var cameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(cameraXml);
            var cameraExpected = MaximumInformationTestCase.GetCamera_03();
            TopicsCompareTool.ComparePerspectiveCameras(cameraExpected, cameraActual);
        }

        [Fact]
        public void VerifyCountOfComments()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            Assert.Equal(4, markupXml.Descendants("Comment").Where(curr => curr.Parent.Name.LocalName != "Comment").Count());
        }

        [Fact]
        public void VerifyCommentCorrect_01()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            var commentExpected = MaximumInformationTestCase.CreateComments().ToList()[0];
            var commentActual = TestUtilities.GetCommentFromXml(markupXml.Descendants("Comment").Where(curr => curr.Parent.Name.LocalName != "Comment").ToList()[0]);
            TopicsCompareTool.CompareSingleComment(commentExpected, commentActual);
        }

        [Fact]
        public void VerifyCommentCorrect_02()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            var commentExpected = MaximumInformationTestCase.CreateComments().ToList()[1];
            var commentActual = TestUtilities.GetCommentFromXml(markupXml.Descendants("Comment").Where(curr => curr.Parent.Name.LocalName != "Comment").ToList()[1]);
            TopicsCompareTool.CompareSingleComment(commentExpected, commentActual);
        }

        [Fact]
        public void VerifyCommentCorrect_03()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            var commentExpected = MaximumInformationTestCase.CreateComments().ToList()[2];
            var commentActual = TestUtilities.GetCommentFromXml(markupXml.Descendants("Comment").Where(curr => curr.Parent.Name.LocalName != "Comment").ToList()[2]);
            TopicsCompareTool.CompareSingleComment(commentExpected, commentActual);
        }

        [Fact]
        public void VerifyCommentCorrect_04()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            var commentExpected = MaximumInformationTestCase.CreateComments().ToList()[3];
            var commentActual = TestUtilities.GetCommentFromXml(markupXml.Descendants("Comment").Where(curr => curr.Parent.Name.LocalName != "Comment").ToList()[3]);
            TopicsCompareTool.CompareSingleComment(commentExpected, commentActual);
        }

        [Fact]
        public void VerifySnippetPresent()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            var snippetXml = markupXml.Descendants("BimSnippet").FirstOrDefault();
            Assert.NotNull(snippetXml);
        }

        [Fact]
        public void VerifySnippetDataPresent()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            var snippetXml = markupXml.Descendants("BimSnippet").FirstOrDefault();

            Assert.NotNull(snippetXml.Descendants("Reference"));
            Assert.NotNull(snippetXml.Descendants("ReferenceSchema"));
            Assert.NotNull(snippetXml.Attribute("SnippetType"));

            // isExternal is false by default, so if the actual element points to an external snippet
            // the XML attribute may be omitted
            if (MaximumInformationTestCase.CreateTopic().Markup.Topic.BimSnippet.isExternal)
            {
                Assert.NotNull(snippetXml.Attribute("isExternal"));
                Assert.Equal("true", snippetXml.Attribute("isExternal").Value);
            }
            else
            {
                // But if it is false AND the attribute is present, it must be set to false
                if (snippetXml.Attribute("isExternal") != null)
                {
                    Assert.Equal("false", snippetXml.Attribute("isExternal").Value);
                }
            }
        }

        [Fact]
        public void VerifySnippetDataCorrect()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");
            var snippetXml = markupXml.Descendants("BimSnippet").FirstOrDefault();

            var snippetExpected = MaximumInformationTestCase.CreateTopic().Markup.Topic.BimSnippet;

            var snippetActual = TestUtilities.GetBimSnippetFromXml(snippetXml);
            TopicsCompareTool.CompareBimSnippet(snippetExpected, snippetActual);
        }

        [Fact]
        public void CheckIfFileInHeaderSetCorrectly()
        {
            var markupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BcFv2TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID + "/markup.bcf");

            // Only one header element
            Assert.True(markupXml.Descendants("Header").Count() == 1);

            var fileXml = markupXml.Descendants("File").FirstOrDefault();

            // Only one file element and no other elements
            Assert.True(markupXml.Descendants("File").Count() == 1);
            Assert.True(markupXml.Descendants("Header").FirstOrDefault().Nodes().Count() == 1);

            // File has exactly three descendants
            Assert.True(markupXml.Descendants("File").Descendants().Count() == 3);

            var ifcProjectExpected = "0M6o7Znnv7hxsbWgeu7oQq";
            var ifcSpatialStructElementExpected = "23B$bNeGHFQuMYJzvUX0FD";
            var isExternalExpected = false;
            var filenameExpected = "IfcPile_01.ifc";
            var dateExpected = "2014-10-27T16:27:27Z";
            var referenceExpected = "../IfcPile_01.ifc";

            var headerXml = markupXml.FirstNode as XElement;

            var ifcProjectActual = headerXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("IfcProject").Value;
            var ifcSpatialStructElementActual = headerXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("IfcSpatialStructureElement").Value;
            var isExternalActual = bool.Parse(headerXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("isExternal").Value);
            var filenameActual = headerXml.Descendants("Filename").OfType<XElement>().FirstOrDefault().Value;
            ;
            var dateActual = headerXml.Descendants("Date").OfType<XElement>().FirstOrDefault().Value;
            ;
            var referenceActual = headerXml.Descendants("Reference").OfType<XElement>().FirstOrDefault().Value;
            ;

            Assert.Equal(ifcProjectExpected, ifcProjectActual);
            Assert.Equal(ifcSpatialStructElementExpected, ifcSpatialStructElementActual);
            Assert.Equal(isExternalExpected, isExternalActual);
            Assert.Equal(filenameExpected, filenameActual);
            Assert.Equal(dateExpected, dateActual);
            Assert.Equal(referenceExpected, referenceActual);
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