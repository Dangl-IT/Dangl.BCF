using Xunit;
using iabi.BCF.BCFv21;
using System.Linq;
using System.IO;

namespace iabi.BCF.Tests.BCFTestCases.v21.Import
{
    public class SingleInvisibleWall
    {
        public BCFv21Container ReadContainer;

        public SingleInvisibleWall()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.SingleInvisibleWall);
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
            var expected = "e1fff3a6-db0f-48e8-a240-0e2f38b2fc21";
            var actual = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(actual);
        }

        [Fact]
        public void HasNoDuplicatedGuid_ViewpointAndComment()
        {
            var topicGuids = ReadContainer.Topics.Select(curr => curr.Markup.Topic.Guid);
            var commentGuids = ReadContainer.Topics.SelectMany(curr => curr.Markup.Comment).Select(curr => curr.Guid);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleInvisibleWall), data);
        }


        public class Topic01
        {
            public static BCFv21Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.SingleInvisibleWall);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "e1fff3a6-db0f-48e8-a240-0e2f38b2fc21");
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
            public void CheckCommentGuid_01()
            {
                var expected = "4ad2c5e2-f7f1-4b6f-a2c7-5b4f93895e8e";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_02()
            {
                var expected = "5d1463e3-c6b1-4867-9b32-046461e81bb5";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void Comment_01_ReferencesNoViewpoint()
            {
                var comment = ReadTopic.Markup.Comment.First(c => c.Guid == "4ad2c5e2-f7f1-4b6f-a2c7-5b4f93895e8e");
                Assert.False(comment.ShouldSerializeViewpoint());
            }

            [Fact]
            public void Comment_02_ReferencesViewpoint()
            {
                var comment = ReadTopic.Markup.Comment.First(c => c.Guid == "5d1463e3-c6b1-4867-9b32-046461e81bb5");
                Assert.True(comment.ShouldSerializeViewpoint());
                Assert.Equal("194f2ccb-9526-4f41-bfe0-635397a79873", comment.Viewpoint.Guid);
            }
            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "194f2ccb-9526-4f41-bfe0-635397a79873";
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
                var expected = TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleInvisibleWall).GetBinaryData("e1fff3a6-db0f-48e8-a240-0e2f38b2fc21/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["194f2ccb-9526-4f41-bfe0-635397a79873"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}
