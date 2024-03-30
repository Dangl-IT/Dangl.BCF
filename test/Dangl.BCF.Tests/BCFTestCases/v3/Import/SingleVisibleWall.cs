using System.IO;
using System.Linq;
using Dangl.BCF.BCFv3;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v3.Import
{
    public class SingleVisibleWall
    {
        public BCFv3Container ReadContainer;

        public SingleVisibleWall()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.SingleVisibleWall);
        }

        [Fact]
        public void CanConverterToBcfV21Container()
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
            var expected = 1;
            var actual = ReadContainer.Topics.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckTopicGuid_01()
        {
            var expected = "456b122e-3c6c-480c-9f60-30b4bd47ae08";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.SingleVisibleWall), data);
        }


        public class Topic01
        {
            public BCFv3Container ReadContainer;

            public BCFTopic ReadTopic;

            public Topic01()
            {
                ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.SingleVisibleWall);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "456b122e-3c6c-480c-9f60-30b4bd47ae08");
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
            public void Markup_HeaderSectionPresent()
            {
                Assert.True(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "9e913da8-860c-4d48-9d94-ccccc2e1d9ca";
                var actual = ReadTopic.Markup.Topic.Viewpoints.First().Guid;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 1;
                var actual = ReadTopic.Markup.Topic.Viewpoints.Count;
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
                var expected = TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.SingleVisibleWall).GetBinaryData("456b122e-3c6c-480c-9f60-30b4bd47ae08/snapshot-9e913da8-860c-4d48-9d94-ccccc2e1d9ca.png");
                var actual = ReadTopic.ViewpointSnapshots["9e913da8-860c-4d48-9d94-ccccc2e1d9ca"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}