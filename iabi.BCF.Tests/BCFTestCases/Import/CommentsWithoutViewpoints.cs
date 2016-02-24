using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class CommentsWithoutViewpoints
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.CommentsWithoutViewpoints);
        }

        [TestMethod]
        public void ReadSuccessfullyNotNull()
        {
            Assert.IsNotNull(ReadContainer);
        }

        [TestMethod]
        public void CheckTopicGuid()
        {
            var Expected = "8ac78763-2e73-4b88-8549-a5bfb45f7133";
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
            var Expected = "ab0016e8-016c-4bdb-a19f-a1b4957734b1";
            Assert.IsTrue(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [TestMethod]
        public void CheckCommentGuid_02()
        {
            var Expected = "3d56f8d1-149a-4cb5-86df-ec3049648169";
            Assert.IsTrue(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [TestMethod]
        public void CheckCommentGuid_03()
        {
            var Expected = "987dbb75-2d91-4c81-8a3c-aabeb5547f09";
            Assert.IsTrue(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [TestMethod]
        public void CheckCommentCount()
        {
            var Expected = 3;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckCommentViewpointReference_01()
        {
            var CommentGuid = "ab0016e8-016c-4bdb-a19f-a1b4957734b1";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.IsTrue(Comment.ShouldSerializeViewpoint());
            Assert.AreEqual("228cdc2d-18d2-402e-9e1a-a758e0b22ed5", Comment.Viewpoint.Guid);
        }

        [TestMethod]
        public void CheckCommentViewpointReference_02()
        {
            var CommentGuid = "3d56f8d1-149a-4cb5-86df-ec3049648169";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.IsFalse(Comment.ShouldSerializeViewpoint());
        }

        [TestMethod]
        public void CheckCommentViewpointReference_03()
        {
            var CommentGuid = "987dbb75-2d91-4c81-8a3c-aabeb5547f09";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.IsFalse(Comment.ShouldSerializeViewpoint());
        }

        [TestMethod]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "228cdc2d-18d2-402e-9e1a-a758e0b22ed5";
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
            var Expected = BCFTestCasesImportData.comments_without_viewpoints.GetBinaryData("8ac78763-2e73-4b88-8549-a5bfb45f7133/snapshot.png");
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
            CompareTool.CompareFiles(BCFTestCasesImportData.comments_without_viewpoints, Data);
        }
    }
}