using System.IO;
using System.Linq;
using iabi.BCF.BCFv21;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v21.Import
{
    public class RelatedTopicWithOtherTopicMissing
    {
        public BCFv21Container ReadContainer;

        public RelatedTopicWithOtherTopicMissing()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.RelatedTopicsWithOtherTopicMissing);
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
            var expected = "a6f801b9-6bf6-4cb9-8b89-1ae24b76074a";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.RelatedTopicsWithOtherTopicMissing), data);
        }


        public class Topic01
        {
            public static BCFv21Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.RelatedTopicsWithOtherTopicMissing);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "a6f801b9-6bf6-4cb9-8b89-1ae24b76074a");
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
                Assert.Equal(1, ReadTopic.Markup.Topic.RelatedTopic.Count);
                Assert.Equal("c69c8879-bd4a-4182-a759-f3c8c5b47c94", ReadTopic.Markup.Topic.RelatedTopic.First().Guid);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var expected = 2;
                var actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var expected = "7269cc66-6412-49ef-9ef2-7a116f98c866";
                Assert.True(ReadTopic.Markup.Comment.Any(curr => curr.Guid == expected));
            }

            [Fact]
            public void CheckCommentGuid_02()
            {
                var expected = "da5306a4-b02c-43f9-aeba-602bc4db925a";
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