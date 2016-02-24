using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class PerspectiveView
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.PerspectiveView);
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
            var Expected = "8bcb4942-a716-4e4f-b699-e1c150a50594";
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
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.PerspectiveView);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "8bcb4942-a716-4e4f-b699-e1c150a50594");
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
                var Expected = "4b181afe-c628-4516-9c19-b0dce3a49a47";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckComment_ModifiedData()
            {
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == "4b181afe-c628-4516-9c19-b0dce3a49a47");
                Assert.AreEqual("pbuts@kubusinfo.nl", Comment.ModifiedAuthor);
                Assert.AreEqual(new DateTime(2015, 06, 09, 07, 58, 02), Comment.ModifiedDate.ToUniversalTime());
            }

            [TestMethod]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "4b181afe-c628-4516-9c19-b0dce3a49a47";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.IsTrue(Comment.ShouldSerializeViewpoint());
                Assert.AreEqual("8ab3bf5c-3c8d-4f49-b974-8968f94b59d4", Comment.Viewpoint.Guid);
            }

            [TestMethod]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.IsFalse(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "8ab3bf5c-3c8d-4f49-b974-8968f94b59d4";
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
                var Expected = BCFTestCasesImportData.Perspective_view.GetBinaryData("8bcb4942-a716-4e4f-b699-e1c150a50594/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["8ab3bf5c-3c8d-4f49-b974-8968f94b59d4"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }

            [TestMethod]
            public void Viewpoint_NoOrthogonalCamera()
            {
                var Actual = ReadTopic.Viewpoints.First();
                Assert.IsFalse(Actual.ShouldSerializeOrthogonalCamera());
            }

            [TestMethod]
            public void Viewpoint_PerspectiveCamera()
            {
                var Actual = ReadTopic.Viewpoints.First();
                Assert.IsTrue(Actual.ShouldSerializePerspectiveCamera());

                Assert.AreEqual(21.75835377599418, Actual.PerspectiveCamera.CameraViewPoint.X);
                Assert.AreEqual(-19.69042708255157, Actual.PerspectiveCamera.CameraViewPoint.Y);
                Assert.AreEqual(19.20322065558115, Actual.PerspectiveCamera.CameraViewPoint.Z);
                Assert.AreEqual(-0.53672118533613, Actual.PerspectiveCamera.CameraDirection.X);
                Assert.AreEqual(0.35874211235957, Actual.PerspectiveCamera.CameraDirection.Y);
                Assert.AreEqual(-0.76369788924101, Actual.PerspectiveCamera.CameraDirection.Z);
                Assert.AreEqual(-0.63492792770306, Actual.PerspectiveCamera.CameraUpVector.X);
                Assert.AreEqual(0.42438307300583, Actual.PerspectiveCamera.CameraUpVector.Y);
                Assert.AreEqual(0.64557380210850, Actual.PerspectiveCamera.CameraUpVector.Z);
                Assert.AreEqual(70, Actual.PerspectiveCamera.FieldOfView);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Perspective_view, Data);
        }
    }
}