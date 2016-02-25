using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.Import
{
    public class SelectedComponent
    {
        public BCFv2Container ReadContainer;

        public SelectedComponent()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SelectedComponent);
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
            var Expected = "9c137387-b581-4803-a85d-4931c3b42714";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Selected_component, Data);
        }


        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic_01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SelectedComponent);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "9c137387-b581-4803-a85d-4931c3b42714");
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
                var Expected = 1;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var Expected = "54b2b92a-55d2-4c37-9d83-1582269c3e67";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckComment_ModifiedData()
            {
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == "54b2b92a-55d2-4c37-9d83-1582269c3e67");
                Assert.Equal("pbuts@kubusinfo.nl", Comment.ModifiedAuthor);
                Assert.Equal(new DateTime(2015, 06, 09, 08, 27, 56), Comment.ModifiedDate.ToUniversalTime());
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "54b2b92a-55d2-4c37-9d83-1582269c3e67";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("5324f8f9-199b-42a2-8982-60a1febabded", Comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "5324f8f9-199b-42a2-8982-60a1febabded";
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
                var Expected = BCFTestCasesImportData.Selected_component.GetBinaryData("9c137387-b581-4803-a85d-4931c3b42714/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["5324f8f9-199b-42a2-8982-60a1febabded"];
                Assert.True(Expected.SequenceEqual(Actual));
            }
        }
    }
}