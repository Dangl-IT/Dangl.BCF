using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.Import
{
     
    public class VisibleSpaceAndRestOfModelVisible
    {
        public static BCFv2Container ReadContainer;

                public VisibleSpaceAndRestOfModelVisible()
        {
            if (ReadContainer == null)
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.VisibleSpaceAndRestOfModelVisible);
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
            var Expected = "79d7bbbc-3029-43bf-91fd-47ef0915e7ae";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public Topic_01()
            {
                if (ReadContainer == null)
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.VisibleSpaceAndRestOfModelVisible);
                if (ReadTopic == null)
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "79d7bbbc-3029-43bf-91fd-47ef0915e7ae");
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
                var Expected = "691ed8dc-0d52-498d-b2d3-d858a98b9a43";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void NoCommentReferencesViewpoint()
            {
                Assert.True(ReadTopic.Markup.Comment.All(Curr => !Curr.ShouldSerializeViewpoint()));
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
                Assert.Equal("C:\\e.ifc", HeaderEntry.Filename);
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", HeaderEntry.IfcProject);
                Assert.Equal(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.Equal(true, HeaderEntry.isExternal);
                Assert.Equal(null, HeaderEntry.Reference);
                Assert.Equal(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.Equal(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
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
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var Expected = BCFTestCasesImportData.visible_space_and_the_rest_of_the_model_visible.GetBinaryData("79d7bbbc-3029-43bf-91fd-47ef0915e7ae/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["756d6918-4ee9-4c88-9c74-b00c6c2410e5"];
                Assert.True(Expected.SequenceEqual(Actual));
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
            CompareTool.CompareFiles(BCFTestCasesImportData.visible_space_and_the_rest_of_the_model_visible, Data);
        }
    }
}
