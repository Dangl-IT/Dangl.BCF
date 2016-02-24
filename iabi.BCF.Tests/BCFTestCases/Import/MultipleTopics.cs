using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class MultipleTopics
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
        }

        [TestMethod]
        public void ReadSuccessfullyNotNull()
        {
            Assert.IsNotNull(ReadContainer);
        }

        [TestMethod]
        public void CheckTopicCount()
        {
            var Expected = 3;
            var Actual = ReadContainer.Topics.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckTopicGuid_01()
        {
            var Expected = "33b0e849-72f1-434c-88e1-b7b3b8c27f38";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.IsTrue(Actual);
        }

        [TestMethod]
        public void CheckTopicGuid_02()
        {
            var Expected = "402d5148-88ef-4510-8b9d-e632602541c6";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.IsTrue(Actual);
        }

        [TestMethod]
        public void CheckTopicGuid_03()
        {
            var Expected = "f5d76cd4-46bc-450c-a513-c1f62ac24026";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.IsTrue(Actual);
        }

        /// <summary>
        /// This topic references the extensions.xsd inside its project.bcfp but there are not extensions
        /// </summary>
        [TestMethod]
        public void DontImportReferenceToExtensions()
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(ReadContainer.BCFProject.ExtensionSchema));
        }

        [TestClass]
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            [ClassInitialize]
            public static void Create(TestContext GivenContext)
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "33b0e849-72f1-434c-88e1-b7b3b8c27f38");
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
                var Expected = "88f24705-2785-4928-8d82-56bb998c91b9";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "88f24705-2785-4928-8d82-56bb998c91b9";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.IsTrue(Comment.ShouldSerializeViewpoint());
                Assert.AreEqual("cb8bccde-ce99-4a95-859c-1b7f5031997d", Comment.Viewpoint.Guid);
            }

            [TestMethod]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.AreEqual(0, ReadTopic.Markup.Header.Count);
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "cb8bccde-ce99-4a95-859c-1b7f5031997d";
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
                var Expected = BCFTestCasesImportData.multiple_topics_bcfzip.GetBinaryData("33b0e849-72f1-434c-88e1-b7b3b8c27f38/snapshot.png");
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

        [TestClass]
        public class Topic_02
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            [ClassInitialize]
            public static void Create(TestContext GivenContext)
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "402d5148-88ef-4510-8b9d-e632602541c6");
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
                var Expected = "a3e4a0d7-49ed-4a9b-93d6-1f99266417c3";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "a3e4a0d7-49ed-4a9b-93d6-1f99266417c3";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.IsTrue(Comment.ShouldSerializeViewpoint());
                Assert.AreEqual("5485542b-5bb6-4314-90df-537f8246473", Comment.Viewpoint.Guid);
            }

            [TestMethod]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.AreEqual(0, ReadTopic.Markup.Header.Count);
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "5485542b-5bb6-4314-90df-537f8246473f";
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
                var Expected = BCFTestCasesImportData.multiple_topics_bcfzip.GetBinaryData("402d5148-88ef-4510-8b9d-e632602541c6/snapshot.png");
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

        [TestClass]
        public class Topic_03
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            [ClassInitialize]
            public static void Create(TestContext GivenContext)
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "f5d76cd4-46bc-450c-a513-c1f62ac24026");
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
                var Expected = "1e7bc348-8807-4d4a-bdbd-fab6761076a1";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "1e7bc348-8807-4d4a-bdbd-fab6761076a1";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.IsTrue(Comment.ShouldSerializeViewpoint());
                Assert.AreEqual("2f5041e7-1b8b-4310-abff-e0e7e620b878", Comment.Viewpoint.Guid);
            }

            [TestMethod]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.AreEqual(0, ReadTopic.Markup.Header.Count);
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "2f5041e7-1b8b-4310-abff-e0e7e620b878";
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
                var Expected = BCFTestCasesImportData.multiple_topics_bcfzip.GetBinaryData("f5d76cd4-46bc-450c-a513-c1f62ac24026/snapshot.png");
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
            CompareTool.CompareFiles(BCFTestCasesImportData.multiple_topics_bcfzip, Data);
        }
    }
}