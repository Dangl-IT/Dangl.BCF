using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.Import
{
     
    public class SingleInvisibleWall
    {
        public static BCFv2Container ReadContainer;

                public static void Create()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleInvisibleWall);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckTopicGuid_01()
        {
            var Expected = "0425bfd9-3982-471d-b963-abd07622b191";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleInvisibleWall);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "0425bfd9-3982-471d-b963-abd07622b191");
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var Expected = 1;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var Expected = "451f78bf-42f4-425b-afb0-3a957672740f";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void NoCommentReferencesViewpoint()
            {
                Assert.True(ReadTopic.Markup.Comment.All(Curr => !Curr.ShouldSerializeViewpoint()));
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "451f78bf-42f4-425b-afb0-3a957672740f";
                var Actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var Expected = 1;
                var Actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var Expected = 1;
                var Actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var Expected = BCFTestCasesImportData.SingleInvisibleWall.GetBinaryData("0425bfd9-3982-471d-b963-abd07622b191/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["451f78bf-42f4-425b-afb0-3a957672740f"];
                Assert.True(Expected.SequenceEqual(Actual));
            }
        }

        [Fact]
        public void NoDuplicatedViewpoints()
        {
            var TopicGuids = ReadContainer.Topics.Select(Curr => Curr.Markup.Topic.Guid);
            var CommentGuids = ReadContainer.Topics.SelectMany(Curr => Curr.Markup.Comment).Select(Curr => Curr.Guid);
            var ViewpointGuids = ReadContainer.Topics.SelectMany(Curr => Curr.Viewpoints).Select(Curr => Curr.GUID);
            var AllGuids = CommentGuids.Concat(ViewpointGuids).Concat(TopicGuids);
            Assert.Equal(AllGuids.Count(), AllGuids.Distinct().Count());
        }

        [Fact]
        public void WriteOut()
        {
            var MemStream = new MemoryStream();
            ReadContainer.WriteStream(MemStream);
            var Data = MemStream.ToArray();
            Assert.NotNull(Data);
            Assert.True(Data.Length > 0);
        }

        [Fact]
        public void WriteAndCompare()
        {
            var MemStream = new MemoryStream();
            ReadContainer.WriteStream(MemStream);
            var Data = MemStream.ToArray();
            CompareTool.CompareFiles(BCFTestCasesImportData.SingleInvisibleWall, Data);
        }
    }
}
