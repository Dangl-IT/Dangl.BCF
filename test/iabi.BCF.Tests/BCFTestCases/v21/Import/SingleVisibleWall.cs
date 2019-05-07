using System.IO;
using System.Linq;
using iabi.BCF.BCFv21;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v21.Import
{
    public class SingleVisibleWall
    {
        public BCFv21Container ReadContainer;

        public SingleVisibleWall()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.SingleVisibleWall);
        }

        [Fact]
        public void CanConverterToBcfV2Container()
        {
            var converter = new iabi.BCF.Converter.V21ToV2(ReadContainer);
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
            var expected = 1;
            var actual = ReadContainer.Topics.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckTopicGuid_01()
        {
            var expected = "d029895e-2bdc-4f48-8bf4-8e540425f238";
            var topicWithExpectedIdPresent = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(topicWithExpectedIdPresent);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleVisibleWall), data);
        }


        public class Topic01
        {
            public static BCFv21Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.SingleVisibleWall);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "d029895e-2bdc-4f48-8bf4-8e540425f238");
                }
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var expected = 2;
                var actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "96604a56-c32d-49a9-9e81-70bc1c961568";
                var actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 1;
                var actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var expected = 1;
                var actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var expected = TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleVisibleWall).GetBinaryData("d029895e-2bdc-4f48-8bf4-8e540425f238/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["96604a56-c32d-49a9-9e81-70bc1c961568"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}