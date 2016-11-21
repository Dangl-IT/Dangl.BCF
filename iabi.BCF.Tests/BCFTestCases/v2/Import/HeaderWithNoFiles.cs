using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class HeaderWithNoFiles
    {
        public BCFv2Container ReadContainer;

        public HeaderWithNoFiles()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.HeaderWithNoFiles);
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
            var Expected = "1243de1b-2257-4d0c-8b82-ec09d5dfb350";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.header_with_no_files, Data);
        }


        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic_01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.HeaderWithNoFiles);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "1243de1b-2257-4d0c-8b82-ec09d5dfb350");
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
                var Expected = 1;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var Expected = "54b178f3-2787-4a9b-a260-e425596e6cd7";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "54b178f3-2787-4a9b-a260-e425596e6cd7";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("2d8de4f3-3658-4011-9ef6-c110259b75c6", Comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(0, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "2d8de4f3-3658-4011-9ef6-c110259b75c6";
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
            public void Viewpoint_CompareSnapshotBinary()
            {
                var Expected = BCFTestCasesImportData.header_with_no_files.GetBinaryData("1243de1b-2257-4d0c-8b82-ec09d5dfb350/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots.First().Value;
                Assert.True(Expected.SequenceEqual(Actual));
            }

            [Fact]
            public void Viewpoint_NoOrthogonalCamera()
            {
                var Actual = ReadTopic.Viewpoints.First();
                Assert.False(Actual.ShouldSerializeOrthogonalCamera());
            }

            [Fact]
            public void Viewpoint_ComponentsCountCorrect()
            {
                Assert.Equal(3, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_ComponentCorrect_01()
            {
                var Component = ReadTopic.Viewpoints.First().Components.First();
                Assert.False(Component.ShouldSerializeAuthoringToolId());
                Assert.Null(Component.Color);
                Assert.Equal("0hm_TZ7fj2wQv5AxxV1KqF", Component.IfcGuid);
                Assert.False(Component.ShouldSerializeOriginatingSystem());
                Assert.Equal(true, Component.Selected);
                Assert.Equal(true, Component.SelectedSpecified);
                Assert.Equal(false, Component.Visible);
            }
        }
    }
}