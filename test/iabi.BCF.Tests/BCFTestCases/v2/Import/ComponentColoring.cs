using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class ComponentColoring
    {
        public BCFv2Container ReadContainer;

        public ComponentColoring()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.ComponentColoring);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicGuid()
        {
            var Expected = "06cf5831-cde5-4b19-b2f9-a319a9590bc2";
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
            var Expected = "fc72d354-8534-44b4-9686-f7b9c4a19adf";
            Assert.True(ReadContainer.Topics.First().Markup.Comment.Any(Curr => Curr.Guid == Expected));
        }

        [Fact]
        public void CheckCommentCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckCommentViewpointReference_01()
        {
            var CommentGuid = "fc72d354-8534-44b4-9686-f7b9c4a19adf";
            var Comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
            Assert.False(Comment.ShouldSerializeViewpoint());
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "812da616-a239-404d-adeb-66dc69fa7cf1";
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
            var Expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.ComponentColoring).GetBinaryData("06cf5831-cde5-4b19-b2f9-a319a9590bc2/snapshot.png");
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
        public void Viewpoint_ComponentsCountCorrect()
        {
            Assert.Equal(1, ReadContainer.Topics.First().Viewpoints.First().Components.Count);
        }

        [Fact]
        public void Viewpoint_ComponentCorrect_01()
        {
            var Component = ReadContainer.Topics.First().Viewpoints.First().Components.First();
            Assert.False(Component.ShouldSerializeAuthoringToolId());
            Assert.True(new byte[] {255, 0, 255, 0}.SequenceEqual(Component.Color));
            Assert.Equal("1mrgg_O_bBBv_tvdtVwK59", Component.IfcGuid);
            Assert.Equal("Allplan", Component.OriginatingSystem);
            Assert.Equal(false, Component.Selected);
            Assert.Equal(true, Component.SelectedSpecified);
            Assert.Equal(true, Component.Visible);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.ComponentColoring), Data);
        }
    }
}