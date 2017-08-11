using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class MultipleFilesInHeader
    {
        public BCFv2Container ReadContainer;

        public MultipleFilesInHeader()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.MultipleFilesInHeader);
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
            var expected = "0656f0fb-d1d2-463d-bfb2-a31590c269fc";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleFilesInHeader), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.MultipleFilesInHeader);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "0656f0fb-d1d2-463d-bfb2-a31590c269fc");
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
                var expected = 1;
                var actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var expected = "d959dd22-83b5-4e72-8c7f-0d62720ce0f1";
                Assert.True(ReadTopic.Markup.Comment.Any(curr => curr.Guid == expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var commentGuid = "d959dd22-83b5-4e72-8c7f-0d62720ce0f1";
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
                Assert.True(comment.ShouldSerializeViewpoint());
                Assert.Equal("5823a9b9-594a-48c3-afab-230e1a1bd0b9", comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(2, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void Markup_HeaderFileCorrect_01()
            {
                var headerEntry = ReadTopic.Markup.Header.First();

                Assert.False(headerEntry.DateSpecified);
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", headerEntry.IfcProject);
                Assert.Null(headerEntry.IfcSpatialStructureElement);
                Assert.True(headerEntry.isExternal);
                Assert.Null(headerEntry.Reference);
                Assert.True(headerEntry.ShouldSerializeDate());
                Assert.False(headerEntry.ShouldSerializeFilename());
                Assert.True(headerEntry.ShouldSerializeIfcProject());
                Assert.False(headerEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void Markup_HeaderFileCorrect_02()
            {
                var headerEntry = ReadTopic.Markup.Header.First();

                Assert.False(headerEntry.DateSpecified);
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", headerEntry.IfcProject);
                Assert.Null(headerEntry.IfcSpatialStructureElement);
                Assert.True(headerEntry.isExternal);
                Assert.Null(headerEntry.Reference);
                Assert.True(headerEntry.ShouldSerializeDate());
                Assert.False(headerEntry.ShouldSerializeFilename());
                Assert.True(headerEntry.ShouldSerializeIfcProject());
                Assert.False(headerEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "5823a9b9-594a-48c3-afab-230e1a1bd0b9";
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
            public void Viewpoint_CompareSnapshotBinary()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleFilesInHeader).GetBinaryData("0656f0fb-d1d2-463d-bfb2-a31590c269fc/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots.First().Value;
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_NoOrthogonalCamera()
            {
                var actual = ReadTopic.Viewpoints.First();
                Assert.False(actual.ShouldSerializeOrthogonalCamera());
            }

            [Fact]
            public void Viewpoint_ComponentsCountCorrect()
            {
                Assert.Equal(6, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_ComponentCorrect_01()
            {
                var component = ReadTopic.Viewpoints.First().Components.First();
                Assert.False(component.ShouldSerializeAuthoringToolId());
                Assert.Null(component.Color);
                Assert.Equal("0VSwrt2fv2LwdJyikI8wPj", component.IfcGuid);
                Assert.False(component.ShouldSerializeOriginatingSystem());
                Assert.True(component.Selected);
                Assert.True(component.SelectedSpecified);
                Assert.True(component.Visible);
            }
        }
    }
}