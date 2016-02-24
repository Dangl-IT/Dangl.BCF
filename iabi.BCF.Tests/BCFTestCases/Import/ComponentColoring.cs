using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class ComponentColoring
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.ComponentColoring);
        }

        [TestMethod]
        public void ReadSuccessfullyNotNull()
        {
            Assert.IsNotNull(ReadContainer);
        }

        [TestMethod]
        public void CheckTopicGuid()
        {
            var Expected = "06cf5831-cde5-4b19-b2f9-a319a9590bc2";
            var Actual = ReadContainer.Topics.First().Markup.Topic.Guid;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckTopicCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckCommentGuid_01()
        {
            var Expected = "fc72d354-8534-44b4-9686-f7b9c4a19adf";
            Assert.IsTrue(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [TestMethod]
        public void CheckCommentCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckCommentViewpointReference_01()
        {
            var CommentGuid = "fc72d354-8534-44b4-9686-f7b9c4a19adf";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.IsFalse(Comment.ShouldSerializeViewpoint());
        }

        [TestMethod]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "812da616-a239-404d-adeb-66dc69fa7cf1";
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.First().Guid;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckViewpointCount_InMarkup()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckViewpointCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Viewpoints.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void Viewpoint_CompareSnapshotBinary()
        {
            var Expected = BCFTestCasesImportData.component_coloring.GetBinaryData("06cf5831-cde5-4b19-b2f9-a319a9590bc2/snapshot.png");
            var Actual = ReadContainer.Topics.First().ViewpointSnapshots.First().Value;
            Assert.IsTrue(Expected.SequenceEqual(Actual));
        }

        [TestMethod]
        public void Viewpoint_NoOrthogonalCamera()
        {
            var Actual = ReadContainer.Topics.First().Viewpoints.First();
            Assert.IsFalse(Actual.ShouldSerializeOrthogonalCamera());
        }

        [TestMethod]
        public void Viewpoint_ComponentsCountCorrect()
        {
            Assert.AreEqual(1, ReadContainer.Topics.First().Viewpoints.First().Components.Count);
        }

        [TestMethod]
        public void Viewpoint_ComponentCorrect_01()
        {
            var Component = ReadContainer.Topics.First().Viewpoints.First().Components.First();
            Assert.IsFalse(Component.ShouldSerializeAuthoringToolId());
            Assert.IsTrue(new byte[] { 255, 0, 255, 0 }.SequenceEqual(Component.Color));
            Assert.AreEqual("1mrgg_O_bBBv_tvdtVwK59", Component.IfcGuid);
            Assert.AreEqual("Allplan", Component.OriginatingSystem);
            Assert.AreEqual(false, Component.Selected);
            Assert.AreEqual(true, Component.SelectedSpecified);
            Assert.AreEqual(true, Component.Visible);
        }

        [TestMethod]
        public void WriteOut()
        {
            var MemStream = new MemoryStream();
            ReadContainer.WriteStream(MemStream);
            var Data = MemStream.ToArray();
            Assert.IsNotNull(Data);
            Assert.IsTrue(Data.Length > 0);
        }

        [TestMethod]
        public void WriteAndCompare()
        {
            var MemStream = new MemoryStream();
            ReadContainer.WriteStream(MemStream);
            var Data = MemStream.ToArray();
            CompareTool.CompareFiles(BCFTestCasesImportData.component_coloring, Data);
        }
    }
}