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
        public void CanConverterToBcfV21Container()
        {
            var converter = new iabi.BCF.Converter.V2ToV21(ReadContainer);
            var upgradedContainer = converter.Convert();
            Assert.NotNull(upgradedContainer);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicGuid()
        {
            var expected = "8ac78763-2e73-4b88-8549-a5bfb45f7133";
            var actual = ReadContainer.Topics.First().Markup.Topic.Guid;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckCommentGuid_01()
        {
            var expected = "ab0016e8-016c-4bdb-a19f-a1b4957734b1";
            Assert.Contains(ReadContainer.Topics.First().Markup.Comment, curr => curr.Guid == expected);
        }

        [Fact]
        public void CheckCommentGuid_02()
        {
            var expected = "3d56f8d1-149a-4cb5-86df-ec3049648169";
            Assert.Contains(ReadContainer.Topics.First().Markup.Comment, curr => curr.Guid == expected);
        }

        [Fact]
        public void CheckCommentGuid_03()
        {
            var expected = "987dbb75-2d91-4c81-8a3c-aabeb5547f09";
            Assert.Contains(ReadContainer.Topics.First().Markup.Comment, curr => curr.Guid == expected);
        }

        [Fact]
        public void CheckCommentCount()
        {
            var expected = 3;
            var actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckCommentViewpointReference_01()
        {
            var commentGuid = "ab0016e8-016c-4bdb-a19f-a1b4957734b1";
            var comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
            Assert.True(comment.ShouldSerializeViewpoint());
            Assert.Equal("228cdc2d-18d2-402e-9e1a-a758e0b22ed5", comment.Viewpoint.Guid);
        }

        [Fact]
        public void CheckCommentViewpointReference_02()
        {
            var commentGuid = "3d56f8d1-149a-4cb5-86df-ec3049648169";
            var comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
            Assert.False(comment.ShouldSerializeViewpoint());
        }

        [Fact]
        public void CheckCommentViewpointReference_03()
        {
            var commentGuid = "987dbb75-2d91-4c81-8a3c-aabeb5547f09";
            var comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
            Assert.False(comment.ShouldSerializeViewpoint());
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var expected = "228cdc2d-18d2-402e-9e1a-a758e0b22ed5";
            var actual = ReadContainer.Topics.First().Markup.Viewpoints.First().Guid;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckViewpointCount_InMarkup()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().Markup.Viewpoints.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckViewpointCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().Viewpoints.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Viewpoint_CompareSnapshotBinary()
        {
            var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.CommentsWithoutViewpoints).GetBinaryData("8ac78763-2e73-4b88-8549-a5bfb45f7133/snapshot.png");
            var actual = ReadContainer.Topics.First().ViewpointSnapshots.First().Value;
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void Viewpoint_NoOrthogonalCamera()
        {
            var actual = ReadContainer.Topics.First().Viewpoints.First();
            Assert.False(actual.ShouldSerializeOrthogonalCamera());
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.CommentsWithoutViewpoints), data);
        }
    }
}