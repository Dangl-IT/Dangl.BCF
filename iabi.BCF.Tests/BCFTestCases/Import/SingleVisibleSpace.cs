using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class SingleVisibleSpace
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleVisibleSpace);
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
            var Expected = "5fce171d-f840-42c8-961b-e775b4195902";
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
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleVisibleSpace);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "5fce171d-f840-42c8-961b-e775b4195902");
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
                var Expected = "1676fc6a-2f5e-45bb-9c29-9eb4e492d6b1";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "1676fc6a-2f5e-45bb-9c29-9eb4e492d6b1";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.IsTrue(Comment.ShouldSerializeViewpoint());
                Assert.AreEqual("ab8d56c2-b0e7-4f0b-8045-970c6fb51d1f", Comment.Viewpoint.Guid);
            }

            [TestMethod]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.AreEqual(1, ReadTopic.Markup.Header.Count);
            }

            [TestMethod]
            public void Markup_HeaderFileCorrect_01()
            {
                var HeaderEntry = ReadTopic.Markup.Header.First();
                Assert.AreEqual(false, HeaderEntry.DateSpecified);
                Assert.AreEqual("2SugUv4EX5LAhcVpDp2dUH", HeaderEntry.IfcProject);
                Assert.AreEqual(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.AreEqual(true, HeaderEntry.isExternal);
                Assert.AreEqual(null, HeaderEntry.Reference);
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeFilename(), "Filename error");
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "ab8d56c2-b0e7-4f0b-8045-970c6fb51d1f";
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
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var Expected = BCFTestCasesImportData.single_visible_space.GetBinaryData("5fce171d-f840-42c8-961b-e775b4195902/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["ab8d56c2-b0e7-4f0b-8045-970c6fb51d1f"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
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
            CompareTool.CompareFiles(BCFTestCasesImportData.single_visible_space, Data);
        }
    }
}