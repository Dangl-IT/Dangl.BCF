using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class MultipleFilesInHeader
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleFilesInHeader);
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
            var Expected = "0656f0fb-d1d2-463d-bfb2-a31590c269fc";
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
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleFilesInHeader);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "0656f0fb-d1d2-463d-bfb2-a31590c269fc");
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
                var Expected = "d959dd22-83b5-4e72-8c7f-0d62720ce0f1";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "d959dd22-83b5-4e72-8c7f-0d62720ce0f1";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.IsTrue(Comment.ShouldSerializeViewpoint());
                Assert.AreEqual("5823a9b9-594a-48c3-afab-230e1a1bd0b9", Comment.Viewpoint.Guid);
            }

            [TestMethod]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.AreEqual(2, ReadTopic.Markup.Header.Count);
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
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeDate());
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeFilename());
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [TestMethod]
            public void Markup_HeaderFileCorrect_02()
            {
                var HeaderEntry = ReadTopic.Markup.Header.First();

                Assert.AreEqual(false, HeaderEntry.DateSpecified);
                Assert.AreEqual("2SugUv4EX5LAhcVpDp2dUH", HeaderEntry.IfcProject);
                Assert.AreEqual(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.AreEqual(true, HeaderEntry.isExternal);
                Assert.AreEqual(null, HeaderEntry.Reference);
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeDate());
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeFilename());
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "5823a9b9-594a-48c3-afab-230e1a1bd0b9";
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
                var Expected = BCFTestCasesImportData.multiple_files_in_header.GetBinaryData("0656f0fb-d1d2-463d-bfb2-a31590c269fc/snapshot.png");
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
                Assert.AreEqual(6, ReadTopic.Viewpoints.First().Components.Count);
            }

            [TestMethod]
            public void Viewpoint_ComponentCorrect_01()
            {
                var Component = ReadTopic.Viewpoints.First().Components.First();
                Assert.IsFalse(Component.ShouldSerializeAuthoringToolId());
                Assert.IsNull(Component.Color);
                Assert.AreEqual("0VSwrt2fv2LwdJyikI8wPj", Component.IfcGuid);
                Assert.IsFalse(Component.ShouldSerializeOriginatingSystem());
                Assert.AreEqual(true, Component.Selected);
                Assert.AreEqual(true, Component.SelectedSpecified);
                Assert.AreEqual(true, Component.Visible);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.multiple_files_in_header, Data);
        }
    }
}