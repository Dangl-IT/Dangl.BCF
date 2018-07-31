using System.IO;
using System.Linq;
using iabi.BCF.BCFv21;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v21.Import
{
    public class UserAssignment
    {
        public BCFv21Container ReadContainer;

        public UserAssignment()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.UserAssignment);
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
            var expected = "12628303-e7a2-4c5c-bc3c-fb088fd24077";
            var topicWithExpectedIdPresent = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(topicWithExpectedIdPresent);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV21(BCFv21ImportTestCases.UserAssignment), data);
        }


        public class Topic01
        {
            public static BCFv21Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV21(BCFv21ImportTestCases.UserAssignment);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "12628303-e7a2-4c5c-bc3c-fb088fd24077");
                }
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckAssignedTo()
            {
                Assert.Equal("jon.anders.sollien@catenda.no", ReadTopic.Markup.Topic.AssignedTo);
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
                var expected = "7a02a14b-5c22-433c-b98c-ac5b9153a62b";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 0;
                var actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var expected = 0;
                var actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }
        }
    }
}