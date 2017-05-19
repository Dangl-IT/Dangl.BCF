using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class DefaultComponentVisibility
    {
        public BCFv2Container ReadContainer;

        public DefaultComponentVisibility()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DefaultComponentVisibility);
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
            var expected = "8127b587-2b97-477e-8a82-fb5a2facd171";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DefaultComponentVisibility), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DefaultComponentVisibility);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "8127b587-2b97-477e-8a82-fb5a2facd171");
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
                var expected = "9050d65a-6e84-492c-9820-0caeaf2a4ada";
                Assert.True(ReadTopic.Markup.Comment.Any(curr => curr.Guid == expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var commentGuid = "9050d65a-6e84-492c-9820-0caeaf2a4ada";
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
                Assert.True(comment.ShouldSerializeViewpoint());
                Assert.Equal("e8d2035a-a30e-40a5-947c-6f0c8f6d8b13", comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(1, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void Markup_HeaderFileCorrect_01()
            {
                var headerEntry = ReadTopic.Markup.Header.First();

                Assert.Equal(false, headerEntry.DateSpecified);
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", headerEntry.IfcProject);
                Assert.Equal(null, headerEntry.IfcSpatialStructureElement);
                Assert.Equal(true, headerEntry.isExternal);
                Assert.Equal(null, headerEntry.Reference);
                Assert.Equal(true, headerEntry.ShouldSerializeDate());
                Assert.Equal(false, headerEntry.ShouldSerializeFilename());
                Assert.Equal(true, headerEntry.ShouldSerializeIfcProject());
                Assert.Equal(false, headerEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "e8d2035a-a30e-40a5-947c-6f0c8f6d8b13";
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
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DefaultComponentVisibility).GetBinaryData("8127b587-2b97-477e-8a82-fb5a2facd171/snapshot.png");
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
                Assert.Equal(1, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_ComponentCorrect_01()
            {
                var component = ReadTopic.Viewpoints.First().Components.First();
                Assert.False(component.ShouldSerializeAuthoringToolId());
                Assert.Null(component.Color);
                Assert.Equal("1E8YkwPMfB$h99jtn_uAjI", component.IfcGuid);
                Assert.False(component.ShouldSerializeOriginatingSystem());
                Assert.Equal(true, component.Selected);
                Assert.Equal(true, component.SelectedSpecified);
                Assert.Equal(true, component.Visible);
            }
        }
    }
}