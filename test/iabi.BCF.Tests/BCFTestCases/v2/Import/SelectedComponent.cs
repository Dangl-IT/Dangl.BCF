using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class SelectedComponent
    {
        public BCFv2Container ReadContainer;

        public SelectedComponent()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.SelectedComponent);
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
            var expected = "9c137387-b581-4803-a85d-4931c3b42714";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.SelectedComponent), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.SelectedComponent);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "9c137387-b581-4803-a85d-4931c3b42714");
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
                var expected = "54b2b92a-55d2-4c37-9d83-1582269c3e67";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckComment_ModifiedData()
            {
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == "54b2b92a-55d2-4c37-9d83-1582269c3e67");
                Assert.Equal("pbuts@kubusinfo.nl", comment.ModifiedAuthor);
                Assert.Equal(new DateTime(2015, 06, 09, 08, 27, 56), comment.ModifiedDate.ToUniversalTime());
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var commentGuid = "54b2b92a-55d2-4c37-9d83-1582269c3e67";
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
                Assert.True(comment.ShouldSerializeViewpoint());
                Assert.Equal("5324f8f9-199b-42a2-8982-60a1febabded", comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "5324f8f9-199b-42a2-8982-60a1febabded";
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
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.SelectedComponent).GetBinaryData("9c137387-b581-4803-a85d-4931c3b42714/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["5324f8f9-199b-42a2-8982-60a1febabded"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}