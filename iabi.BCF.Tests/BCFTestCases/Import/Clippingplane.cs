using iabi.BCF.BCFv2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class Clippingplane
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.Clippingplane);
        }

        [TestMethod]
        public void ReadSuccessfullyNotNull()
        {
            Assert.IsNotNull(ReadContainer);
        }

        [TestMethod]
        public void CheckTopicGuid()
        {
            var Expected = "709b92c2-64e0-40cc-b861-00f8ab0e3945";
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
        public void CheckCommentGuid()
        {
            var Expected = "b2cb4b77-83c8-47b4-a89c-bee37d667e4e";
            var Actual = ReadContainer.Topics.First().Markup.Comment.First().Guid;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckCommentCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "82b38f43-237b-45c2-96d0-8840ffb344ad";
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
            var Expected = BCFTestCasesImportData.Clippingplane.GetBinaryData("709b92c2-64e0-40cc-b861-00f8ab0e3945/snapshot.png");
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
        public void Viewpoint_ClippingPlaneCorrect()
        {
            var Actual = ReadContainer.Topics.First().Viewpoints.First().ClippingPlanes.First();
            Assert.IsNotNull(Actual);

            Assert.AreEqual(0, Actual.Direction.X);
            Assert.AreEqual(0, Actual.Direction.Y);
            Assert.AreEqual(1, Actual.Direction.Z);
            Assert.AreEqual(0, Actual.Location.X);
            Assert.AreEqual(0, Actual.Location.Y);
            Assert.AreEqual(7.665119721718699, Actual.Location.Z);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Clippingplane, Data);
        }
    }
}