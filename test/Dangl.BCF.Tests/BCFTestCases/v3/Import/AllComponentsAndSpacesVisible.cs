using Xunit;
using Dangl.BCF.BCFv3;
using System.Linq;
using System.IO;

namespace Dangl.BCF.Tests.BCFTestCases.v3.Import
{
    public class AllComponentsAndSpacesVisible
    {
        public BCFv3Container ReadContainer;

        public AllComponentsAndSpacesVisible()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.AllComponentsAndSpacesVisible);
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
            var expected = "9af7d7db-2cd3-4b32-bec6-3edf21d86d50";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.AllComponentsAndSpacesVisible), data);
        }


        public class Topic01
        {
            public BCFv3Container ReadContainer;

            public BCFTopic ReadTopic;

            public Topic01()
            {
                ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.AllComponentsAndSpacesVisible);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "9af7d7db-2cd3-4b32-bec6-3edf21d86d50");
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
            public void CheckComments()
            {
                Assert.Empty(ReadTopic.Markup.Topic.Comments);
            }

            [Fact]
            public void Markup_HeaderSectionPresent()
            {
                Assert.True(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "bb62a667-15a1-4942-a372-12ad9519994d";
                var actual = ReadTopic.Markup.Topic.Viewpoints.Single().Guid;
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
                var expected = TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.AllComponentsAndSpacesVisible).GetBinaryData("9af7d7db-2cd3-4b32-bec6-3edf21d86d50/snapshot-bb62a667-15a1-4942-a372-12ad9519994d.png");
                var actual = ReadTopic.ViewpointSnapshots["bb62a667-15a1-4942-a372-12ad9519994d"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}
