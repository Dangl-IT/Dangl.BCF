using iabi.BCF.BCFv2;
using iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport
{
    /*
     * Note: Here is "\n" used as line break instead of "\r\n" due to the
     * XML specification specifying "\n" as the line break character.
     *
     */

    [TestClass]
    public class DocumentReferenceExternalTest
    {
        public static BCFv2Container CreatedContainer;

        public static ZipArchive CreatedArchive;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            CreatedContainer = BCFTestCases.CreateAndExport.Factory.BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.DocumentReferenceExternal);

            CreatedArchive = ZipArchiveFactory.ReturnAndWriteIfRequired(CreatedContainer, BCFTestCaseData.DocumentReferenceExternal_TestCaseName, BCFTestCaseData.DocumentReferenceExternal_Readme);
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

        // TODO IMPLEMENT TESTS

        //public string[] ExpectedFiles
        //{
        //    get
        //    {
        //        return new string[]
        //        {
        //            "project.bcfp",
        //            "extensions.xsd",
        //            "bcf.version",
        //            "markup.xsd",       // Schema for markup file, is referenced in a topic as document reference
        //            "IfcPile_01.ifc",   // File attachment in the root folder
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf",
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv",
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_"+ BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02+ ".bcfv",
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_"+ BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03+ ".bcfv",
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Snapshot_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01+ ".png",
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Snapshot_"+ BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02+ ".png",
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Snapshot_"+ BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03+ ".png",
        //           BCFTestCaseData.PerspectiveCamera_TopicGuid + "/JsonElement.json", // Data element of the BIM Snippet
        //           BCFTestCaseData.PerspectiveCamera_ReferencedTopicGuid + "/markup.bcf", // Referenced topic
        //        };
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFilesPresent()
        //{
        //    foreach (var ExpectedFile in ExpectedFiles)
        //    {
        //        if (CreatedArchive.Entries.All(Curr => Curr.FullName != ExpectedFile))
        //        {
        //            Assert.Fail("Did not find expected file in archive: " + ExpectedFile);
        //        }
        //    }
        //}

        //[TestMethod]
        //public void CheckIfNoAdditionalFilesPresent()
        //{
        //    foreach (var CurrentEntry in CreatedArchive.Entries)
        //    {
        //        if (!ExpectedFiles.Contains(CurrentEntry.FullName))
        //        {
        //            Assert.Fail("Zip Archive should not contain entry " + CurrentEntry.FullName);
        //        }
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFilesAreAllValidXml()
        //{
        //    foreach (var CurrentEntry in CreatedArchive.Entries)
        //    {
        //        if (CurrentEntry.FullName.Contains(".bcfp")
        //            || CurrentEntry.FullName.Contains(".version")
        //            || CurrentEntry.FullName.Contains(".bcf")
        //            || CurrentEntry.FullName.Contains(".bcfv")
        //            || CurrentEntry.FullName.Contains(".xsd"))
        //        {
        //            using (StreamReader Rdr = new StreamReader(CurrentEntry.Open()))
        //            {
        //                var Text = Rdr.ReadToEnd();
        //                var Xml = XElement.Parse(Text);
        //                // No exception no cry!
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFileDataIsEqual_JsonElement()
        //{
        //    var DataExpected = BCFTestCaseData.JsonElement;
        //    using (MemoryStream MemStream = new MemoryStream())
        //    {
        //        CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFTestCaseData.PerspectiveCamera_TopicGuid + "/JsonElement.json").Open().CopyTo(MemStream);
        //        var DataActual = MemStream.ToArray();
        //        Assert.IsTrue(DataExpected.SequenceEqual(DataActual));
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFileDataIsEqual_IfcPile()
        //{
        //    var DataExpected = BCFTestCaseData.IfcPile;
        //    using (MemoryStream MemStream = new MemoryStream())
        //    {
        //        CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == "IfcPile_01.ifc").Open().CopyTo(MemStream);
        //        var DataActual = MemStream.ToArray();
        //        Assert.IsTrue(DataExpected.SequenceEqual(DataActual));
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFileDataIsEqual_MarkupSchema()
        //{
        //    var DataExpected = BCFTestCaseData.MarkupSchema;
        //    using (MemoryStream MemStream = new MemoryStream())
        //    {
        //        CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == "markup.xsd").Open().CopyTo(MemStream);
        //        var DataActual = MemStream.ToArray();
        //        Assert.IsTrue(DataExpected.SequenceEqual(DataActual));
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFileDataIsEqual_Snapshot01()
        //{
        //    var ImageConverter = new ImageConverter();
        //    var DataExpected = (byte[])ImageConverter.ConvertTo(BCFTestCaseData.PerspectiveCamera_Snapshot_01, typeof(byte[]));
        //    using (MemoryStream MemStream = new MemoryStream())
        //    {
        //        CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Snapshot_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".png").Open().CopyTo(MemStream);
        //        var DataActual = MemStream.ToArray();
        //        Assert.IsTrue(DataExpected.SequenceEqual(DataActual));
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFileDataIsEqual_Snapshot02()
        //{
        //    var ImageConverter = new ImageConverter();
        //    var DataExpected = (byte[])ImageConverter.ConvertTo(BCFTestCaseData.PerspectiveCamera_Snapshot_02, typeof(byte[]));
        //    using (MemoryStream MemStream = new MemoryStream())
        //    {
        //        CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Snapshot_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02 + ".png").Open().CopyTo(MemStream);
        //        var DataActual = MemStream.ToArray();
        //        Assert.IsTrue(DataExpected.SequenceEqual(DataActual));
        //    }
        //}

        //[TestMethod]
        //public void CheckIfFileDataIsEqual_Snapshot03()
        //{
        //    var ImageConverter = new ImageConverter();
        //    var DataExpected = (byte[])ImageConverter.ConvertTo(BCFTestCaseData.PerspectiveCamera_Snapshot_03, typeof(byte[]));
        //    using (MemoryStream MemStream = new MemoryStream())
        //    {
        //        CreatedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Snapshot_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03 + ".png").Open().CopyTo(MemStream);
        //        var DataActual = MemStream.ToArray();
        //        Assert.IsTrue(DataExpected.SequenceEqual(DataActual));
        //    }
        //}

        //[TestMethod]
        //public void VersionTagCorrect()
        //{
        //    var ExpectedVersionId = "2.0";
        //    var ExpectedDetailedVersion = "2.0";
        //    var VersionXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, "bcf.version");
        //    var ActualVersionId = VersionXml.Attribute("VersionId").Value;
        //    var ActualDetailedVersion = ((XText)((XElement)VersionXml.FirstNode).FirstNode).Value;

        //    Assert.IsTrue(VersionXml.Nodes().Count() == 1 && ((XElement)VersionXml.FirstNode).Name.LocalName == "DetailedVersion");
        //    Assert.AreEqual(ExpectedVersionId, ActualVersionId);
        //    Assert.AreEqual(ExpectedDetailedVersion, ActualDetailedVersion);
        //}

        //[TestMethod]
        //public void Viewpoint_01_InMarkupSet()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");

        //    var ViewpointElement = MarkupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoints" && Curr.Attribute("Guid").Value == BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01);

        //    Assert.IsNotNull(ViewpointElement);

        //    Assert.AreEqual(2, ViewpointElement.DescendantNodes().OfType<XElement>().Count());
        //    Assert.AreEqual(2, ViewpointElement.DescendantNodes().OfType<XText>().Count());

        //    var ViewpointReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoint");
        //    Assert.AreEqual("Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv", ViewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

        //    var SnapshotReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Snapshot");
        //    Assert.AreEqual("Snapshot_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".png", SnapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        //}

        //[TestMethod]
        //public void Viewpoint_02_InMarkupSet()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");

        //    var ViewpointElement = MarkupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoints" && Curr.Attribute("Guid").Value == BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02);

        //    Assert.IsNotNull(ViewpointElement);

        //    Assert.AreEqual(2, ViewpointElement.DescendantNodes().OfType<XElement>().Count());
        //    Assert.AreEqual(2, ViewpointElement.DescendantNodes().OfType<XText>().Count());

        //    var ViewpointReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoint");
        //    Assert.AreEqual("Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02 + ".bcfv", ViewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

        //    var SnapshotReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Snapshot");
        //    Assert.AreEqual("Snapshot_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02 + ".png", SnapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        //}

        //[TestMethod]
        //public void Viewpoint_03_InMarkupSet()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");

        //    var ViewpointElement = MarkupXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoints" && Curr.Attribute("Guid").Value == BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03);

        //    Assert.IsNotNull(ViewpointElement);

        //    Assert.AreEqual(2, ViewpointElement.DescendantNodes().OfType<XElement>().Count());
        //    Assert.AreEqual(2, ViewpointElement.DescendantNodes().OfType<XText>().Count());

        //    var ViewpointReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Viewpoint");
        //    Assert.AreEqual("Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03 + ".bcfv", ViewpointReferenceElement.DescendantNodes().OfType<XText>().First().Value);

        //    var SnapshotReferenceElement = ViewpointElement.Nodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Snapshot");
        //    Assert.AreEqual("Snapshot_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03 + ".png", SnapshotReferenceElement.DescendantNodes().OfType<XText>().First().Value);
        //}

        //[TestMethod]
        //public void Viewpoint_01_NoOrthogonalCameraSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv");
        //    Assert.IsFalse(ViewpointXml.DescendantNodes().OfType<XElement>().Any(Curr => Curr.Name.LocalName == "OrthogonalCamera"));
        //}

        //[TestMethod]
        //public void Viewpoint_02_NoOrthogonalCameraSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02 + ".bcfv");
        //    Assert.IsFalse(ViewpointXml.DescendantNodes().OfType<XElement>().Any(Curr => Curr.Name.LocalName == "OrthogonalCamera"));
        //}

        //[TestMethod]
        //public void Viewpoint_03_NoOrthogonalCameraSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03 + ".bcfv");
        //    Assert.IsFalse(ViewpointXml.DescendantNodes().OfType<XElement>().Any(Curr => Curr.Name.LocalName == "OrthogonalCamera"));
        //}

        //[TestMethod]
        //public void Viewpoint_01_PerspectiveCameraSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv");
        //    var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
        //    Assert.IsNotNull(CameraXml);
        //}

        //[TestMethod]
        //public void Viewpoint_02_PerspectiveCameraSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02 + ".bcfv");
        //    var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
        //    Assert.IsNotNull(CameraXml);
        //}

        //[TestMethod]
        //public void Viewpoint_03_PerspectiveCameraSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03 + ".bcfv");
        //    var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
        //    Assert.IsNotNull(CameraXml);
        //}

        //[TestMethod]
        //public void Viewpoint_01_ClippingPlanesSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv");
        //    var Planes = ViewpointXml.Descendants("ClippingPlanes").FirstOrDefault().Descendants("ClippingPlane").Select(Curr => new
        //    {
        //        Location = new
        //        {
        //            x = Convert.ToDouble(Curr.Descendants("Location").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
        //            y = Convert.ToDouble(Curr.Descendants("Location").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
        //            z = Convert.ToDouble(Curr.Descendants("Location").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
        //        },
        //        Direction = new
        //        {
        //            x = Convert.ToDouble(Curr.Descendants("Direction").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
        //            y = Convert.ToDouble(Curr.Descendants("Direction").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
        //            z = Convert.ToDouble(Curr.Descendants("Direction").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
        //        }
        //    });

        //    var ExpectedPlanes = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.GetPlanes().ToList();
        //    var ActualPlanes = Planes.ToList();

        //    Assert.AreEqual(ExpectedPlanes.Count, ActualPlanes.Count);

        //    for (int i = 0; i < ExpectedPlanes.Count; i++)
        //    {
        //        // location
        //        Assert.AreEqual(ExpectedPlanes[i].Location.X, ActualPlanes[i].Location.x);
        //        Assert.AreEqual(ExpectedPlanes[i].Location.Y, ActualPlanes[i].Location.y);
        //        Assert.AreEqual(ExpectedPlanes[i].Location.Z, ActualPlanes[i].Location.z);

        //        //direction
        //        Assert.AreEqual(ExpectedPlanes[i].Direction.X, ActualPlanes[i].Direction.x);
        //        Assert.AreEqual(ExpectedPlanes[i].Direction.Y, ActualPlanes[i].Direction.y);
        //        Assert.AreEqual(ExpectedPlanes[i].Direction.Z, ActualPlanes[i].Direction.z);
        //    }
        //}

        //[TestMethod]
        //public void Viewpoint_01_LinesSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv");

        //    var Lines = ViewpointXml.Descendants("Lines").FirstOrDefault().Descendants("Line").Select(Curr => new
        //    {
        //        StartPoint = new
        //        {
        //            x = Convert.ToDouble(Curr.Descendants("StartPoint").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
        //            y = Convert.ToDouble(Curr.Descendants("StartPoint").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
        //            z = Convert.ToDouble(Curr.Descendants("StartPoint").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
        //        },
        //        EndPoint = new
        //        {
        //            x = Convert.ToDouble(Curr.Descendants("EndPoint").First().Descendants("X").First().Value, CultureInfo.InvariantCulture),
        //            y = Convert.ToDouble(Curr.Descendants("EndPoint").First().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
        //            z = Convert.ToDouble(Curr.Descendants("EndPoint").First().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
        //        }
        //    });

        //    var ExpectedLines = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.GetLines().ToList();
        //    var ActualLines = Lines.ToList();

        //    Assert.AreEqual(ExpectedLines.Count, ActualLines.Count);

        //    for (int i = 0; i < ExpectedLines.Count; i++)
        //    {
        //        // start
        //        Assert.AreEqual(ExpectedLines[i].StartPoint.X, ActualLines[i].StartPoint.x);
        //        Assert.AreEqual(ExpectedLines[i].StartPoint.Y, ActualLines[i].StartPoint.y);
        //        Assert.AreEqual(ExpectedLines[i].StartPoint.Z, ActualLines[i].StartPoint.z);

        //        //end
        //        Assert.AreEqual(ExpectedLines[i].EndPoint.X, ActualLines[i].EndPoint.x);
        //        Assert.AreEqual(ExpectedLines[i].EndPoint.Y, ActualLines[i].EndPoint.y);
        //        Assert.AreEqual(ExpectedLines[i].EndPoint.Z, ActualLines[i].EndPoint.z);
        //    }
        //}

        //[TestMethod]
        //public void Viewpoint_01_ComponentsSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv");

        //    var Components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Components")
        //        .Nodes().OfType<XElement>().Where(Curr => Curr.Name.LocalName == "Component")
        //        .Select(Curr => new
        //        {
        //            IfcGuid = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "IfcGuid").Value,
        //            Visible = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible").Value),
        //            Selected = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected").Value)
        //        }).ToList();

        //    // Count of components
        //    Assert.AreEqual(7, Components.Count);

        //    // #1
        //    Assert.AreEqual("0Gl71cVurFn8bxAOox6M4X", Components[0].IfcGuid);
        //    Assert.AreEqual(true, Components[0].Selected);

        //    // #2
        //    Assert.AreEqual("23Zwlpd71EyvHlH6OZ77nK", Components[1].IfcGuid);
        //    Assert.AreEqual(true, Components[1].Selected);

        //    // #3
        //    Assert.AreEqual("3DvyPxGIn8qR0KDwbL_9r1", Components[2].IfcGuid);
        //    Assert.AreEqual(true, Components[2].Selected);

        //    // #4
        //    Assert.AreEqual("0fdpeZZEX3FwJ7x0ox5kzF", Components[3].IfcGuid);
        //    Assert.AreEqual(true, Components[3].Selected);

        //    // #5
        //    Assert.AreEqual("1OpjQ1Nlv4sQuTxfUC_8zS", Components[4].IfcGuid);
        //    Assert.AreEqual(true, Components[3].Selected);

        //    // #6
        //    Assert.AreEqual("0cSRUx$EX1NRjqiKcYQ$a0", Components[5].IfcGuid);
        //    Assert.AreEqual(true, Components[3].Selected);

        //    // #7
        //    Assert.AreEqual("1jQQiGIAnFzxOUzrdmJYDS", Components[6].IfcGuid);
        //    Assert.AreEqual(true, Components[3].Selected);
        //}

        //[TestMethod]
        //public void Viewpoint_02_ComponentsSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02 + ".bcfv");

        //    var Components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Components")
        //        .Nodes().OfType<XElement>().Where(Curr => Curr.Name.LocalName == "Component")
        //        .Select(Curr => new
        //        {
        //            IfcGuid = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "IfcGuid").Value,
        //            Visible = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible").Value),
        //            Selected = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected").Value)
        //        }).ToList();

        //    // Count of components
        //    Assert.AreEqual(4, Components.Count);

        //    // #1
        //    Assert.AreEqual("0fdpeZZEX3FwJ7x0ox5kzF", Components[0].IfcGuid);
        //    Assert.AreEqual(true, Components[0].Selected);

        //    // #2
        //    Assert.AreEqual("23Zwlpd71EyvHlH6OZ77nK", Components[1].IfcGuid);
        //    Assert.AreEqual(true, Components[1].Selected);

        //    // #3
        //    Assert.AreEqual("1OpjQ1Nlv4sQuTxfUC_8zS", Components[2].IfcGuid);
        //    Assert.AreEqual(true, Components[2].Selected);

        //    // #4
        //    Assert.AreEqual("0cSRUx$EX1NRjqiKcYQ$a0", Components[3].IfcGuid);
        //    Assert.AreEqual(true, Components[3].Selected);
        //}

        //[TestMethod]
        //public void Viewpoint_03_ComponentsSet()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03 + ".bcfv");

        //    var Components = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "Components")
        //        .Nodes().OfType<XElement>().Where(Curr => Curr.Name.LocalName == "Component")
        //        .Select(Curr => new
        //        {
        //            IfcGuid = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "IfcGuid").Value,
        //            Visible = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Visible").Value),
        //            Selected = Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected") == null ? false : bool.Parse(Curr.Attributes().FirstOrDefault(Attr => Attr.Name.LocalName == "Selected").Value)
        //        }).ToList();

        //    // Count of components
        //    Assert.AreEqual(4, Components.Count);

        //    // #1
        //    Assert.AreEqual("0fdpeZZEX3FwJ7x0ox5kzF", Components[0].IfcGuid);
        //    Assert.AreEqual(false, Components[0].Visible);

        //    // #2
        //    Assert.AreEqual("23Zwlpd71EyvHlH6OZ77nK", Components[1].IfcGuid);
        //    Assert.AreEqual(false, Components[1].Visible);

        //    // #3
        //    Assert.AreEqual("1OpjQ1Nlv4sQuTxfUC_8zS", Components[2].IfcGuid);
        //    Assert.AreEqual(false, Components[2].Visible);

        //    // #4
        //    Assert.AreEqual("0cSRUx$EX1NRjqiKcYQ$a0", Components[3].IfcGuid);
        //    Assert.AreEqual(false, Components[3].Visible);
        //}

        //[TestMethod]
        //public void Viewpoint_01_PerspectiveCameraCorrect()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_01 + ".bcfv");
        //    var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
        //    var CameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(CameraXml);
        //    var CameraExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.GetCamera_01();
        //    TopicsCompareTool.ComparePerspectiveCameras(CameraExpected, CameraActual);
        //}

        //[TestMethod]
        //public void Viewpoint_02_PerspectiveCameraCorrect()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_02 + ".bcfv");
        //    var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
        //    var CameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(CameraXml);
        //    var CameraExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.GetCamera_02();
        //    TopicsCompareTool.ComparePerspectiveCameras(CameraExpected, CameraActual);
        //}

        //[TestMethod]
        //public void Viewpoint_03_PerspectiveCameraCorrect()
        //{
        //    var ViewpointXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/Viewpoint_" + BCFTestCaseData.PerspectiveCamera_ViewpointGuid_03 + ".bcfv");
        //    var CameraXml = ViewpointXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "PerspectiveCamera");
        //    var CameraActual = TestUtilities.GetPerspectiveCameraObjectFromXml(CameraXml);
        //    var CameraExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.GetCamera_03();
        //    TopicsCompareTool.ComparePerspectiveCameras(CameraExpected, CameraActual);
        //}

        //[TestMethod]
        //public void VerifyCountOfComments()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    Assert.AreEqual(4, MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").Count());
        //}

        //[TestMethod]
        //public void VerifyCommentCorrect_01()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    var CommentExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.CreateComments().ToList()[0];
        //    var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[0]);
        //    TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        //}

        //[TestMethod]
        //public void VerifyCommentCorrect_02()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    var CommentExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.CreateComments().ToList()[1];
        //    var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[1]);
        //    TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        //}

        //[TestMethod]
        //public void VerifyCommentCorrect_03()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    var CommentExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.CreateComments().ToList()[2];
        //    var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[2]);
        //    TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        //}

        //[TestMethod]
        //public void VerifyCommentCorrect_04()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    var CommentExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.CreateComments().ToList()[3];
        //    var CommentActual = TestUtilities.GetCommentFromXml(MarkupXml.Descendants("Comment").Where(Curr => Curr.Parent.Name.LocalName != "Comment").ToList()[3]);
        //    TopicsCompareTool.CompareSingleComment(CommentExpected, CommentActual);
        //}

        //[TestMethod]
        //public void VerifySnippetPresent()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault() as XElement;
        //    Assert.IsNotNull(SnippetXml);
        //}

        //[TestMethod]
        //public void VerifySnippetDataPresent()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault() as XElement;

        //    Assert.IsNotNull(SnippetXml.Descendants("Reference"), "Missing Reference");
        //    Assert.IsNotNull(SnippetXml.Descendants("ReferenceSchema"), "Missing Schema");
        //    Assert.IsNotNull(SnippetXml.Attribute("SnippetType"), "Missing Type");

        //    // isExternal is false by default, so if the actual element points to an external snippet
        //    // the XML attribute may be omitted
        //    if (BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.CreateTopic().Markup.Topic.BimSnippet.isExternal)
        //    {
        //        Assert.IsNotNull(SnippetXml.Attribute("isExternal"), "Missing external indicator");
        //        Assert.AreEqual("true", SnippetXml.Attribute("isExternal").Value);
        //    }
        //    else
        //    {
        //        // But if it is false AND the attribute is present, it must be set to false
        //        if (SnippetXml.Attribute("isExternal") != null)
        //        {
        //            Assert.AreEqual("false", SnippetXml.Attribute("isExternal").Value);
        //        }
        //    }
        //}

        //[TestMethod]
        //public void VerifySnippetDataCorrect()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");
        //    var SnippetXml = MarkupXml.Descendants("BimSnippet").FirstOrDefault() as XElement;

        //    var SnippetExpected = BCFTestCases.CreateAndExport.Factory.MaximumInformationTestCase.CreateTopic().Markup.Topic.BimSnippet;

        //    var SnippetActual = TestUtilities.GetBimSnippetFromXml(SnippetXml);
        //    TopicsCompareTool.CompareBimSnippet(SnippetExpected, SnippetActual);
        //}

        //[TestMethod]
        //public void CheckIfFileInHeaderSetCorrectly()
        //{
        //    var MarkupXml = XmlUtilities.GetElementFromZipFile(CreatedArchive, BCFTestCaseData.PerspectiveCamera_TopicGuid + "/markup.bcf");

        //    // Only one header element
        //    Assert.IsTrue(MarkupXml.Descendants("Header").Count() == 1);

        //    var FileXml = MarkupXml.Descendants("File").FirstOrDefault();

        //    // Only one file element and no other elements
        //    Assert.IsTrue(MarkupXml.Descendants("File").Count() == 1);
        //    Assert.IsTrue(MarkupXml.Descendants("Header").FirstOrDefault().Nodes().Count() == 1);

        //    // File has exactly three descendants
        //    Assert.IsTrue(MarkupXml.Descendants("File").Descendants().Count() == 3);

        //    var IfcProjectExpected = "0M6o7Znnv7hxsbWgeu7oQq";
        //    var IfcSpatialStructElementExpected = "23B$bNeGHFQuMYJzvUX0FD";
        //    var IsExternalExpected = false;
        //    var FilenameExpected = "IfcPile_01.ifc";
        //    var DateExpected = "2014-10-27T16:27:27Z";
        //    var ReferenceExpected = "../IfcPile_01.ifc";

        //    var HeaderXml = MarkupXml.FirstNode as XElement;

        //    var IfcProjectActual = HeaderXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("IfcProject").Value;
        //    var IfcSpatialStructElementActual = HeaderXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("IfcSpatialStructureElement").Value;
        //    var IsExternalActual = bool.Parse(HeaderXml.Descendants("File").OfType<XElement>().FirstOrDefault().Attribute("isExternal").Value);
        //    var FilenameActual = HeaderXml.Descendants("Filename").OfType<XElement>().FirstOrDefault().Value; ;
        //    var DateActual = HeaderXml.Descendants("Date").OfType<XElement>().FirstOrDefault().Value; ;
        //    var ReferenceActual = HeaderXml.Descendants("Reference").OfType<XElement>().FirstOrDefault().Value; ;

        //    Assert.AreEqual(IfcProjectExpected, IfcProjectActual);
        //    Assert.AreEqual(IfcSpatialStructElementExpected, IfcSpatialStructElementActual);
        //    Assert.AreEqual(IsExternalExpected, IsExternalActual);
        //    Assert.AreEqual(FilenameExpected, FilenameActual);
        //    Assert.AreEqual(DateExpected, DateActual);
        //    Assert.AreEqual(ReferenceExpected, ReferenceActual);
        //}

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
            }
        }
    }
}