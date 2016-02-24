using iabi.BCF.BCFv2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class MultipleViewpointsWithoutComments
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleViewpointsWithoutComments);
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
            var Expected = "03d501f8-1025-462f-841b-35846cb36c31";
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
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleViewpointsWithoutComments);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "03d501f8-1025-462f-841b-35846cb36c31");
            }

            [TestMethod]
            public void TopicPresent()
            {
                Assert.IsNotNull(ReadTopic);
            }

            [TestMethod]
            public void CheckCommentCount()
            {
                var Expected = 6;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckCommentGuid_01()
            {
                var Expected = "2d80a5da-2296-47fc-9fe5-a60b5a02cfb3";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentGuid_02()
            {
                var Expected = "55f6b17c-2c02-4e0e-99de-64c5f1f255c3";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentGuid_03()
            {
                var Expected = "e244a1ee-1972-45e3-b919-5ab5002f96ce";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentGuid_04()
            {
                var Expected = "992a3421-a60b-4594-864b-583feaed9d16";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentGuid_05()
            {
                var Expected = "12fdc962-6ce8-4494-8224-56642068c84f";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentGuid_06()
            {
                var Expected = "ab78330d-c14d-4ca6-9cbb-662be812c9b1";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void NoCommentReferencesViewpoint()
            {
                Assert.IsTrue(ReadTopic.Markup.Comment.All(Curr => !Curr.ShouldSerializeViewpoint()));
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

                Assert.AreEqual(new DateTime(2015, 06, 09, 06, 39, 06), HeaderEntry.Date.ToUniversalTime());
                Assert.AreEqual("C:\\e.ifc", HeaderEntry.Filename);
                Assert.AreEqual("2SugUv4EX5LAhcVpDp2dUH", HeaderEntry.IfcProject);
                Assert.AreEqual(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.AreEqual(true, HeaderEntry.isExternal);
                Assert.AreEqual(null, HeaderEntry.Reference);
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeDate());
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeFilename());
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "d4568303-f5b8-4d29-b3aa-c79d7262f6d6";
                var Actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckViewpointCount_InMarkup()
            {
                var Expected = 6;
                var Actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckViewpointCount()
            {
                var Expected = 6;
                var Actual = ReadTopic.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/d4568303-f5b8-4d29-b3aa-c79d7262f6d6.png");
                var Actual = ReadTopic.ViewpointSnapshots["d4568303-f5b8-4d29-b3aa-c79d7262f6d6"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary_02()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/03c51a6f-13e0-4579-8a54-c6815630d63c.png");
                var Actual = ReadTopic.ViewpointSnapshots["03c51a6f-13e0-4579-8a54-c6815630d63c"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary_03()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["ac1c4776-4a20-49fc-bf58-dcd0c8bc2058"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary_04()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/0138577b-6590-478a-af2b-7674fa87d821.png");
                var Actual = ReadTopic.ViewpointSnapshots["0138577b-6590-478a-af2b-7674fa87d821"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary_05()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/80adf3ef-d542-40f3-9b9e-5057caabc023.png");
                var Actual = ReadTopic.ViewpointSnapshots["80adf3ef-d542-40f3-9b9e-5057caabc023"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary_06()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/1f9ea9fc-0617-43aa-a227-bd8c4abc1c53.png");
                var Actual = ReadTopic.ViewpointSnapshots["1f9ea9fc-0617-43aa-a227-bd8c4abc1c53"];
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
                Assert.AreEqual(0, ReadTopic.Viewpoints.First().Components.Count);
            }

            [TestMethod]
            public void Viewpoint_DontSerializeComponents()
            {
                Assert.IsFalse(ReadTopic.Viewpoints.First().ShouldSerializeComponents());
            }

            [TestMethod]
            public void Viewpoint_LinesCountCorrect()
            {
                Assert.AreEqual(0, ReadTopic.Viewpoints.First().Lines.Count);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.multiple_viewpoints_without_comments, Data);
        }
    }
}