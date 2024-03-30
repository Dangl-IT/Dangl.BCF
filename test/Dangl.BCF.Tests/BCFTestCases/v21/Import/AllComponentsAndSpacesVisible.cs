using Xunit;
using Dangl.BCF.BCFv21;
using System.Linq;
using System.IO;

namespace Dangl.BCF.Tests.BCFTestCases.v21.Import
{
    public class AllComponentsAndSpacesVisible
    {
        public BCFv21Container ReadContainer;

        public AllComponentsAndSpacesVisible()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.AllComponentsAndSpacesVisible);
        }

        [Fact]
        public void CanConverterToBcfV2Container()
        {
            var converter = new Dangl.BCF.Converter.V21ToV2(ReadContainer);
            var downgradedContainer = converter.Convert();
            Assert.NotNull(downgradedContainer);
        }

        [Fact]
        public void CanConverterToBcfV3Container()
        {
            var converter = new Dangl.BCF.Converter.V21ToV3(ReadContainer);
            var upgradedContainer = converter.Convert();
            Assert.NotNull(upgradedContainer);
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
            var expected = "2c02d25f-6cb7-4c14-8737-9c340699d351";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.AllComponentsAndSpacesVisible), data);
        }


        public class Topic01
        {
            public static BCFv21Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.AllComponentsAndSpacesVisible);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "2c02d25f-6cb7-4c14-8737-9c340699d351");
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
                var expected = "82575370-2b8a-4731-a9d9-960b6da7e5fd";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_02()
            {
                var expected = "d05376b9-5ad4-488e-b839-4c4d63408c27";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void Comment_01_ReferencesNoViewpoint()
            {
                var comment = ReadTopic.Markup.Comment.First(c => c.Guid == "82575370-2b8a-4731-a9d9-960b6da7e5fd");
                Assert.False(comment.ShouldSerializeViewpoint());
            }

            [Fact]
            public void Comment_02_ReferencesViewpoint()
            {
                var comment = ReadTopic.Markup.Comment.First(c => c.Guid == "d05376b9-5ad4-488e-b839-4c4d63408c27");
                Assert.True(comment.ShouldSerializeViewpoint());
                Assert.Equal("20d8e1e0-7987-4baf-aaa1-e79246f4ca0b", comment.Viewpoint.Guid);
            }
            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "20d8e1e0-7987-4baf-aaa1-e79246f4ca0b";
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
                var expected = TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.AllComponentsAndSpacesVisible).GetBinaryData("2c02d25f-6cb7-4c14-8737-9c340699d351/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["20d8e1e0-7987-4baf-aaa1-e79246f4ca0b"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}
