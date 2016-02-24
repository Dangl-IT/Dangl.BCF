using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.Import
{
     
    public class UserAssignment
    {
        public static BCFv2Container ReadContainer;

                public static void Create()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.UserAssignment);
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
            var Expected = "d83f5842-19ea-4ca9-85bf-03d4b8f504b9";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.UserAssignment);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "d83f5842-19ea-4ca9-85bf-03d4b8f504b9");
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckAssignedTo()
            {
                Assert.Equal("assigned@test.com", ReadTopic.Markup.Topic.AssignedTo);
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
                var Expected = "ae4837f0-f9de-43cc-ba81-6330a9d07d33";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var Expected = 0;
                var Actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var Expected = 0;
                var Actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(Expected, Actual);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.user_assignment, Data);
        }
    }
}
