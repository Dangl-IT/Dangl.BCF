using System.IO;
using System.Linq;
using Dangl.BCF.BCFv3;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v3.Import
{
    public class SingleVisibleSpace
    {
        public BCFv3Container ReadContainer;

        public SingleVisibleSpace()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.SingleVisibleSpace);
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
            var expected = "aeb8d729-ec2f-4f46-aa9e-e7a9f15582b0";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.SingleVisibleSpace), data);
        }


        public class Topic01
        {
            public BCFv3Container ReadContainer;

            public BCFTopic ReadTopic;

            public Topic01()
            {
                ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.SingleVisibleSpace);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "aeb8d729-ec2f-4f46-aa9e-e7a9f15582b0");
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
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Single(ReadTopic.Markup.Header.Files);
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "4028120e-91f7-435c-bf3c-4aaf97b8a1d1";
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
                var expected = TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.SingleVisibleSpace).GetBinaryData("aeb8d729-ec2f-4f46-aa9e-e7a9f15582b0/4028120e-91f7-435c-bf3c-4aaf97b8a1d1.png");
                var actual = ReadTopic.ViewpointSnapshots["4028120e-91f7-435c-bf3c-4aaf97b8a1d1"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}