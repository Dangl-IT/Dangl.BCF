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
            var expected = "06cf5831-cde5-4b19-b2f9-a319a9590bc2";
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
            var expected = "fc72d354-8534-44b4-9686-f7b9c4a19adf";
            Assert.Contains(ReadContainer.Topics.First().Markup.Comment, curr => curr.Guid == expected);
        }

        [Fact]
        public void CheckCommentCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckCommentViewpointReference_01()
        {
            var commentGuid = "fc72d354-8534-44b4-9686-f7b9c4a19adf";
            var comment = ReadContainer.Topics.First().Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
            Assert.False(comment.ShouldSerializeViewpoint());
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var expected = "812da616-a239-404d-adeb-66dc69fa7cf1";
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
            var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.ComponentColoring).GetBinaryData("06cf5831-cde5-4b19-b2f9-a319a9590bc2/snapshot.png");
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
        public void Viewpoint_ComponentsCountCorrect()
        {
            Assert.Single(ReadContainer.Topics.First().Viewpoints.First().Components);
        }

        [Fact]
        public void Viewpoint_ComponentCorrect_01()
        {
            var component = ReadContainer.Topics.First().Viewpoints.First().Components.First();
            Assert.False(component.ShouldSerializeAuthoringToolId());
            Assert.True(new byte[] {255, 0, 255, 0}.SequenceEqual(component.Color));
            Assert.Equal("1mrgg_O_bBBv_tvdtVwK59", component.IfcGuid);
            Assert.Equal("Allplan", component.OriginatingSystem);
            Assert.False(component.Selected);
            Assert.True(component.SelectedSpecified);
            Assert.True(component.Visible);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.ComponentColoring), data);
        }
    }
}