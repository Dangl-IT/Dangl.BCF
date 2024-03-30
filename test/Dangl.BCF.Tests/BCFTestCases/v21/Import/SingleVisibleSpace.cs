using System.IO;
using System.Linq;
using Dangl.BCF.BCFv21;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v21.Import
{
    public class SingleVisibleSpace
    {
        public BCFv21Container ReadContainer;

        public SingleVisibleSpace()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.SingleVisibleSpace);
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
            var expected = "a3befb7d-8395-4e52-bbca-382fc4198217";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleVisibleSpace), data);
        }


        public class Topic01
        {
            public static BCFv21Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.SingleVisibleSpace);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "a3befb7d-8395-4e52-bbca-382fc4198217");
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
                var expected = "a6d6a846-4bdf-497f-a0a1-a4049ccdd6eb";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_02()
            {
                var expected = "b5595959-2956-48a8-acbc-fcdd47f78c93";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var commentGuid = "a6d6a846-4bdf-497f-a0a1-a4049ccdd6eb";
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
                Assert.False(comment.ShouldSerializeViewpoint());
            }

            [Fact]
            public void CheckCommentViewpointReference_02()
            {
                var commentGuid = "b5595959-2956-48a8-acbc-fcdd47f78c93";
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
                Assert.True(comment.ShouldSerializeViewpoint());
                Assert.Equal("cd21c8ce-c0b8-4554-b623-2ae1d3798806", comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Empty(ReadTopic.Markup.Header);
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "cd21c8ce-c0b8-4554-b623-2ae1d3798806";
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
                var expected = TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.SingleVisibleSpace).GetBinaryData("a3befb7d-8395-4e52-bbca-382fc4198217/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["cd21c8ce-c0b8-4554-b623-2ae1d3798806"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}