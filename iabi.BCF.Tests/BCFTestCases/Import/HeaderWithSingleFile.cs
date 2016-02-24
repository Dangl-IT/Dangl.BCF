using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Xunit;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
     
    public class HeaderWithSingleFile
    {
        public static BCFv2Container ReadContainer;

                public static void Create()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.HeaderWithSingleFile);
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
            var Expected = "6888c011-4ea7-4025-8858-476538cb9f39";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.HeaderWithSingleFile);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "6888c011-4ea7-4025-8858-476538cb9f39");
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
                var Expected = "0ab3fac7-6d85-4e5f-b9e4-13ded9ac9ae6";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "0ab3fac7-6d85-4e5f-b9e4-13ded9ac9ae6";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("cb0869f2-4934-41da-8776-6233c1d43779", Comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(1, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void Markup_HeaderFileCorrect_01()
            {
                var HeaderEntry = ReadTopic.Markup.Header.First();

                Assert.Equal(false, HeaderEntry.DateSpecified);
                Assert.Equal("2TaLqCNHvEn9_7cUVrypdX", HeaderEntry.IfcProject);
                Assert.Equal(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.Equal(true, HeaderEntry.isExternal);
                Assert.Equal(null, HeaderEntry.Reference);
                Assert.Equal(true, HeaderEntry.ShouldSerializeDate());
                Assert.Equal(false, HeaderEntry.ShouldSerializeFilename());
                Assert.Equal(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.Equal(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "cb0869f2-4934-41da-8776-6233c1d43779";
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
                var Expected = BCFTestCasesImportData.header_with_single_file.GetBinaryData("6888c011-4ea7-4025-8858-476538cb9f39/snapshot.png");
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
                Assert.Equal(1, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_ComponentCorrect_01()
            {
                var Component = ReadTopic.Viewpoints.First().Components.First();
                Assert.False(Component.ShouldSerializeAuthoringToolId());
                Assert.Null(Component.Color);
                Assert.Equal("0VSwrt2fv2LwdJyikI8wPj", Component.IfcGuid);
                Assert.False(Component.ShouldSerializeOriginatingSystem());
                Assert.Equal(true, Component.Selected);
                Assert.Equal(true, Component.SelectedSpecified);
                Assert.Equal(true, Component.Visible);
            }
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
            CompareTool.CompareFiles(BCFTestCasesImportData.header_with_single_file, Data);
        }
    }
}
