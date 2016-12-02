using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class CommentsWithoutViewpoints
    {
        public BCFv2Container ReadContainer;

        public CommentsWithoutViewpoints()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.CommentsWithoutViewpoints);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicGuid()
        {
            var Expected = "8ac78763-2e73-4b88-8549-a5bfb45f7133";
            var Actual = ReadContainer.Topics.First().Markup.Topic.Guid;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckCommentGuid_01()
        {
            var Expected = "ab0016e8-016c-4bdb-a19f-a1b4957734b1";
            Assert.True(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [Fact]
        public void CheckCommentGuid_02()
        {
            var Expected = "3d56f8d1-149a-4cb5-86df-ec3049648169";
            Assert.True(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [Fact]
        public void CheckCommentGuid_03()
        {
            var Expected = "987dbb75-2d91-4c81-8a3c-aabeb5547f09";
            Assert.True(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [Fact]
        public void CheckCommentCount()
        {
            var Expected = 3;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckCommentViewpointReference_01()
        {
            var CommentGuid = "ab0016e8-016c-4bdb-a19f-a1b4957734b1";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.True(Comment.ShouldSerializeViewpoint());
            Assert.Equal("228cdc2d-18d2-402e-9e1a-a758e0b22ed5", Comment.Viewpoint.Guid);
        }

        [Fact]
        public void CheckCommentViewpointReference_02()
        {
            var CommentGuid = "3d56f8d1-149a-4cb5-86df-ec3049648169";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.False(Comment.ShouldSerializeViewpoint());
        }

        [Fact]
        public void CheckCommentViewpointReference_03()
        {
            var CommentGuid = "987dbb75-2d91-4c81-8a3c-aabeb5547f09";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.False(Comment.ShouldSerializeViewpoint());
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "228cdc2d-18d2-402e-9e1a-a758e0b22ed5";
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.First().Guid;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckViewpointCount_InMarkup()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckViewpointCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Viewpoints.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void Viewpoint_CompareSnapshotBinary()
        {
            var Expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.CommentsWithoutViewpoints).GetBinaryData("8ac78763-2e73-4b88-8549-a5bfb45f7133/snapshot.png");
            var Actual = ReadContainer.Topics.First().ViewpointSnapshots.First().Value;
            Assert.True(Expected.SequenceEqual(Actual));
        }

        [Fact]
        public void Viewpoint_NoOrthogonalCamera()
        {
            var Actual = ReadContainer.Topics.First().Viewpoints.First();
            Assert.False(Actual.ShouldSerializeOrthogonalCamera());
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.CommentsWithoutViewpoints), Data);
        }
    }
}