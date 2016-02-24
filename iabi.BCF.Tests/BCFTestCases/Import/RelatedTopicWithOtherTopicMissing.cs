using iabi.BCF.BCFv2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class RelatedTopicWithOtherTopicMissing
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.RelatedTopicsWithOtherTopicMissing);
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
            var Expected = "a2ade93c-7ee7-4d11-bb55-8292b4940fc3";
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
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.RelatedTopicsWithOtherTopicMissing);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "a2ade93c-7ee7-4d11-bb55-8292b4940fc3");
            }

            [TestMethod]
            public void TopicPresent()
            {
                Assert.IsNotNull(ReadTopic);
            }

            [TestMethod]
            public void CheckRelatedTopic()
            {
                Assert.AreEqual(1, ReadTopic.Markup.Topic.RelatedTopics.Count);
                Assert.AreEqual("3ebd3a2c-5bc8-4ff5-9e98-d5fa240719b6", ReadTopic.Markup.Topic.RelatedTopics.First().Guid);
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
                var Expected = "0fa7ffd9-77a5-49db-ab41-b0be8c9881eb";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void NoCommentReferencesViewpoint()
            {
                Assert.IsTrue(ReadTopic.Markup.Comment.All(Curr => !Curr.ShouldSerializeViewpoint()));
            }

            [TestMethod]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.IsFalse(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [TestMethod]
            public void CheckViewpointCount_InMarkup()
            {
                var Expected = 0;
                var Actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckViewpointCount()
            {
                var Expected = 0;
                var Actual = ReadTopic.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.RelatedTopicsWithOtherTopicMissing, Data);
        }
    }
}