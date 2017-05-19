using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class RelatedTopicWithOtherTopicMissing
    {
        public BCFv2Container ReadContainer;

        public RelatedTopicWithOtherTopicMissing()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.RelatedTopicsWithOtherTopicMissing);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckTopicGuid_01()
        {
            var expected = "a2ade93c-7ee7-4d11-bb55-8292b4940fc3";
            var actual = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(actual);
        }


        [Fact]
        public void WriteOut()
        {
            var memStream = new MemoryStream();
            ReadContainer.WriteStream(memStream);
            var data = memStream.ToArray();
            Assert.NotNull(data);
            Assert.True(data.Length > 0);
        }

        [Fact]
        public void WriteAndCompare()
        {
            var memStream = new MemoryStream();
            ReadContainer.WriteStream(memStream);
            var data = memStream.ToArray();
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.RelatedTopicsWithOtherTopicMissing), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.RelatedTopicsWithOtherTopicMissing);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "a2ade93c-7ee7-4d11-bb55-8292b4940fc3");
                }
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckRelatedTopic()
            {
                Assert.Equal(1, ReadTopic.Markup.Topic.RelatedTopics.Count);
                Assert.Equal("3ebd3a2c-5bc8-4ff5-9e98-d5fa240719b6", ReadTopic.Markup.Topic.RelatedTopics.First().Guid);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var expected = 1;
                var actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var expected = "0fa7ffd9-77a5-49db-ab41-b0be8c9881eb";
                Assert.True(ReadTopic.Markup.Comment.Any(curr => curr.Guid == expected));
            }

            [Fact]
            public void NoCommentReferencesViewpoint()
            {
                Assert.True(ReadTopic.Markup.Comment.All(curr => !curr.ShouldSerializeViewpoint()));
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 0;
                var actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var expected = 0;
                var actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }
        }
    }
}