using System.IO;
using System.Linq;
using Dangl.BCF.BCFv3;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v3.Import
{
    public class RelatedTopicsWithBothTopicsInSameFile
    {
        public BCFv3Container ReadContainer;

        public RelatedTopicsWithBothTopicsInSameFile()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.RelatedTopicsWithBothTopicsInSameFile);
        }

        [Fact]
        public void CanConverterToBcfV2Container()
        {
            var converter = new Dangl.BCF.Converter.V3ToV21(ReadContainer);
            var downgradedContainer = converter.Convert();
            Assert.NotNull(downgradedContainer);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var expected = 2;
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
        public void CheckTopicGuid_02()
        {
            var expected = "c69c8879-bd4a-4182-a759-f3c8c5b47c94";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.RelatedTopicsWithBothTopicsInSameFile), data, new[] { "documents.xml" });
        }


        public class Topic01
        {
            public static BCFv3Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.RelatedTopicsWithBothTopicsInSameFile);
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
                Assert.Single(ReadTopic.Markup.Topic.RelatedTopics);
                Assert.Equal("c69c8879-bd4a-4182-a759-f3c8c5b47c94", ReadTopic.Markup.Topic.RelatedTopics.First().Guid);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var expected = 1;
                var actual = ReadTopic.Markup.Topic.Comments.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var expected = "da5306a4-b02c-43f9-aeba-602bc4db925a";
                Assert.Contains(ReadTopic.Markup.Topic.Comments, curr => curr.Guid == expected);
            }

            [Fact]
            public void NoCommentReferencesViewpoint()
            {
                Assert.True(ReadTopic.Markup.Topic.Comments.All(curr => !curr.ShouldSerializeViewpoint()));
            }

            [Fact]
            public void Markup_HeaderSectionPresent()
            {
                Assert.True(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 0;
                var actual = ReadTopic.Markup.Topic.Viewpoints.Count;
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

        public class Topic02
        {
            public BCFv3Container ReadContainer;

            public BCFTopic ReadTopic;

            public Topic02()
            {
                ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.RelatedTopicsWithBothTopicsInSameFile);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "c69c8879-bd4a-4182-a759-f3c8c5b47c94");
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var expected = 0;
                var actual = ReadTopic.Markup.Topic.Comments.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void NoCommentReferencesViewpoint()
            {
                Assert.True(ReadTopic.Markup.Topic.Comments.All(curr => !curr.ShouldSerializeViewpoint()));
            }

            [Fact]
            public void Markup_HeaderSectionPresent()
            {
                Assert.True(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 0;
                var actual = ReadTopic.Markup.Topic.Viewpoints.Count;
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