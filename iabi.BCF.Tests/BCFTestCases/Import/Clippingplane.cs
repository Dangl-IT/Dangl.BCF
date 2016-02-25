using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.Import
{
    public class Clippingplane
    {
        public BCFv2Container ReadContainer;

        public Clippingplane()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.Clippingplane);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicGuid()
        {
            var Expected = "709b92c2-64e0-40cc-b861-00f8ab0e3945";
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
        public void CheckCommentGuid()
        {
            var Expected = "b2cb4b77-83c8-47b4-a89c-bee37d667e4e";
            var Actual = ReadContainer.Topics.First().Markup.Comment.First().Guid;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckCommentCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "82b38f43-237b-45c2-96d0-8840ffb344ad";
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
            var Expected = BCFTestCasesImportData.Clippingplane.GetBinaryData("709b92c2-64e0-40cc-b861-00f8ab0e3945/snapshot.png");
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
        public void Viewpoint_ClippingPlaneCorrect()
        {
            var Actual = ReadContainer.Topics.First().Viewpoints.First().ClippingPlanes.First();
            Assert.NotNull(Actual);

            Assert.Equal(0, Actual.Direction.X);
            Assert.Equal(0, Actual.Direction.Y);
            Assert.Equal(1, Actual.Direction.Z);
            Assert.Equal(0, Actual.Location.X);
            Assert.Equal(0, Actual.Location.Y);
            Assert.Equal(7.665119721718699, Actual.Location.Z);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Clippingplane, Data);
        }
    }
}