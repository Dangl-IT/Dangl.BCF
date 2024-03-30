using Xunit;
using Dangl.BCF.BCFv3;
using System.Linq;
using System.IO;

namespace Dangl.BCF.Tests.BCFTestCases.v3.Import
{
    public class SingleInvisibleWall
    {
        public BCFv3Container ReadContainer;

        public SingleInvisibleWall()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.SingleInvisibleWall);
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
            var expected = "d5121f1c-11e0-4f25-9d23-7ace76853a8f";
            var actual = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(actual);
        }

        [Fact]
        public void HasNoDuplicatedGuid_ViewpointAndComment()
        {
            var topicGuids = ReadContainer.Topics.Select(curr => curr.Markup.Topic.Guid);
            var commentGuids = ReadContainer.Topics.SelectMany(curr => curr.Markup.Topic.Comments).Select(curr => curr.Guid);
            var viewpointGuids = ReadContainer.Topics.SelectMany(curr => curr.Viewpoints).Select(curr => curr.Guid);
            var allGuids = commentGuids.Concat(viewpointGuids).Concat(topicGuids);
            Assert.Equal(allGuids.Count(), allGuids.Distinct().Count());
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.SingleInvisibleWall), data);
        }


        public class Topic01
        {
            public static BCFv3Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.SingleInvisibleWall);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "d5121f1c-11e0-4f25-9d23-7ace76853a8f");
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
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
                var expected = "5d1463e3-c6b1-4867-9b32-046461e81bb5";
                Assert.Contains(ReadTopic.Markup.Topic.Comments, curr => curr.Guid == expected);
            }

            [Fact]
            public void Comment_01_ReferencesNoViewpoint()
            {
                var comment = ReadTopic.Markup.Topic.Comments.First(c => c.Guid == "5d1463e3-c6b1-4867-9b32-046461e81bb5");
                Assert.True(comment.ShouldSerializeViewpoint());
            }

            [Fact]
            public void Markup_HeaderSectionPresent()
            {
                Assert.True(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "194f2ccb-9526-4f41-bfe0-635397a79873";
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
                var expected = TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.SingleInvisibleWall).GetBinaryData("d5121f1c-11e0-4f25-9d23-7ace76853a8f/Snapshot_194f2ccb-9526-4f41-bfe0-635397a79873.png");
                var actual = ReadTopic.ViewpointSnapshots["194f2ccb-9526-4f41-bfe0-635397a79873"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}
