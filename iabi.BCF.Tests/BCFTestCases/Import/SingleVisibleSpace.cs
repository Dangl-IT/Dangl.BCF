using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.Import
{
     
    public class SingleVisibleSpace
    {
        public  BCFv2Container ReadContainer;

                public SingleVisibleSpace()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleVisibleSpace);
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
            var Expected = "5fce171d-f840-42c8-961b-e775b4195902";
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
                    ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleVisibleSpace);
                if (ReadTopic == null)
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "5fce171d-f840-42c8-961b-e775b4195902");
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
                var Expected = "1676fc6a-2f5e-45bb-9c29-9eb4e492d6b1";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "1676fc6a-2f5e-45bb-9c29-9eb4e492d6b1";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("ab8d56c2-b0e7-4f0b-8045-970c6fb51d1f", Comment.Viewpoint.Guid);
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
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", HeaderEntry.IfcProject);
                Assert.Equal(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.Equal(true, HeaderEntry.isExternal);
                Assert.Equal(null, HeaderEntry.Reference);
                Assert.Equal(false, HeaderEntry.ShouldSerializeFilename());
                Assert.Equal(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.Equal(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "ab8d56c2-b0e7-4f0b-8045-970c6fb51d1f";
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
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var Expected = BCFTestCasesImportData.single_visible_space.GetBinaryData("5fce171d-f840-42c8-961b-e775b4195902/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["ab8d56c2-b0e7-4f0b-8045-970c6fb51d1f"];
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
                CompareTool.CompareFiles(BCFTestCasesImportData.single_visible_space, Data);
            }
        
    }
}
