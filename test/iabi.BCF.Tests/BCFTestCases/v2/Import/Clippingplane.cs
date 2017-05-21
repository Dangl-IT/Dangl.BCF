using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class Clippingplane
    {
        public BCFv2Container ReadContainer;

        public Clippingplane()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.Clippingplane);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicGuid()
        {
            var expected = "709b92c2-64e0-40cc-b861-00f8ab0e3945";
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
        public void CheckCommentGuid()
        {
            var expected = "b2cb4b77-83c8-47b4-a89c-bee37d667e4e";
            var actual = ReadContainer.Topics.First().Markup.Comment.First().Guid;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckCommentCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var expected = "82b38f43-237b-45c2-96d0-8840ffb344ad";
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
            var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Clippingplane).GetBinaryData("709b92c2-64e0-40cc-b861-00f8ab0e3945/snapshot.png");
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
        public void Viewpoint_ClippingPlaneCorrect()
        {
            var actual = ReadContainer.Topics.First().Viewpoints.First().ClippingPlanes.First();
            Assert.NotNull(actual);

            Assert.Equal(0, actual.Direction.X);
            Assert.Equal(0, actual.Direction.Y);
            Assert.Equal(1, actual.Direction.Z);
            Assert.Equal(0, actual.Location.X);
            Assert.Equal(0, actual.Location.Y);
            Assert.Equal(7.665119721718699, actual.Location.Z);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Clippingplane), data);
        }
    }
}