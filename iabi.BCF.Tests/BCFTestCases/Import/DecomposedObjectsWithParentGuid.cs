using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Xunit;
using System;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
     
    public class DecomposedObjectsWithParentGuid
    {
        public static BCFv2Container ReadContainer;

                public static void Create()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.DecomposedObjectsWithParentGuid);
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
            var Expected = "a23e8824-137a-4bea-a1ad-541f87d274e7";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.DecomposedObjectsWithParentGuid);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "a23e8824-137a-4bea-a1ad-541f87d274e7");
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var Expected = 0;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(Expected, Actual);
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

                Assert.Equal(new DateTime(2015, 06, 09, 06, 39, 06), HeaderEntry.Date.ToUniversalTime());
                Assert.Equal(true, HeaderEntry.DateSpecified);
                Assert.Equal("C:\\e.ifc", HeaderEntry.Filename);
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", HeaderEntry.IfcProject);
                Assert.Equal(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.Equal(true, HeaderEntry.isExternal);
                Assert.Equal(null, HeaderEntry.Reference);
                Assert.Equal(true, HeaderEntry.ShouldSerializeDate());
                Assert.Equal(true, HeaderEntry.ShouldSerializeFilename());
                Assert.Equal(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.Equal(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "53968000-0c3e-41c8-864b-83ede8d7b443";
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
                var Expected = BCFTestCasesImportData.Decomposed_object_with_parent_guid.GetBinaryData("a23e8824-137a-4bea-a1ad-541f87d274e7/snapshot.png");
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
                Assert.Equal("2_hQ1Rixj6lgHTra$L72O4", Component.IfcGuid);
                Assert.Equal("Allplan", Component.OriginatingSystem);
                Assert.Equal(false, Component.Selected);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Decomposed_object_with_parent_guid, Data);
        }
    }
}
