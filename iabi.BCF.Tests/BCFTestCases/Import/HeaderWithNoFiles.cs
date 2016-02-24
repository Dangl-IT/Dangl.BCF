using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class HeaderWithNoFiles
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.HeaderWithNoFiles);
        }

        [TestMethod]
        public void ReadSuccessfullyNotNull()
        {
            Assert.IsNotNull(ReadContainer);
        }

        [TestMethod]
        public void CheckTopicCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckTopicGuid_01()
        {
            var Expected = "1243de1b-2257-4d0c-8b82-ec09d5dfb350";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.IsTrue(Actual);
        }

        [TestClass]
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            [ClassInitialize]
            public static void Create(TestContext GivenContext)
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.HeaderWithNoFiles);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "1243de1b-2257-4d0c-8b82-ec09d5dfb350");
            }

            [TestMethod]
            public void TopicPresent()
            {
                Assert.IsNotNull(ReadTopic);
            }

            [TestMethod]
            public void CheckCommentCount()
            {
                var Expected = 1;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckCommentGuid_01()
            {
                var Expected = "54b178f3-2787-4a9b-a260-e425596e6cd7";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "54b178f3-2787-4a9b-a260-e425596e6cd7";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.IsTrue(Comment.ShouldSerializeViewpoint());
                Assert.AreEqual("2d8de4f3-3658-4011-9ef6-c110259b75c6", Comment.Viewpoint.Guid);
            }

            [TestMethod]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.AreEqual(0, ReadTopic.Markup.Header.Count);
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "2d8de4f3-3658-4011-9ef6-c110259b75c6";
                var Actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckViewpointCount_InMarkup()
            {
                var Expected = 1;
                var Actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckViewpointCount()
            {
                var Expected = 1;
                var Actual = ReadTopic.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary()
            {
                var Expected = BCFTestCasesImportData.header_with_no_files.GetBinaryData("1243de1b-2257-4d0c-8b82-ec09d5dfb350/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots.First().Value;
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }

            [TestMethod]
            public void Viewpoint_NoOrthogonalCamera()
            {
                var Actual = ReadTopic.Viewpoints.First();
                Assert.IsFalse(Actual.ShouldSerializeOrthogonalCamera());
            }

            [TestMethod]
            public void Viewpoint_ComponentsCountCorrect()
            {
                Assert.AreEqual(3, ReadTopic.Viewpoints.First().Components.Count);
            }

            [TestMethod]
            public void Viewpoint_ComponentCorrect_01()
            {
                var Component = ReadTopic.Viewpoints.First().Components.First();
                Assert.IsFalse(Component.ShouldSerializeAuthoringToolId());
                Assert.IsNull(Component.Color);
                Assert.AreEqual("0hm_TZ7fj2wQv5AxxV1KqF", Component.IfcGuid);
                Assert.IsFalse(Component.ShouldSerializeOriginatingSystem());
                Assert.AreEqual(true, Component.Selected);
                Assert.AreEqual(true, Component.SelectedSpecified);
                Assert.AreEqual(false, Component.Visible);
            }
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
            CompareTool.CompareFiles(BCFTestCasesImportData.header_with_no_files, Data);
        }
    }
}