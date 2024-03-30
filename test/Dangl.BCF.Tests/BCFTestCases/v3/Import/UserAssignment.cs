using System.IO;
using System.Linq;
using Dangl.BCF.BCFv3;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v3.Import
{
    public class UserAssignment
    {
        public BCFv3Container ReadContainer;

        public UserAssignment()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.UserAssignment);
        }

        [Fact]
        public void CanConverterToBcfV21Container()
        {
            var converter = new Dangl.BCF.Converter.V3ToV21(ReadContainer);
            var downgradedContainer = converter.Convert();
            Assert.NotNull(downgradedContainer);
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
            var expected = "7ad1a717-bf20-4c12-b511-cbd90370ddba";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCaseV3(BCFv3ImportTestCases.UserAssignment), data, new[] { "documents.xml" });
        }


        public class Topic01
        {
            public BCFv3Container ReadContainer;

            public BCFTopic ReadTopic;

            public Topic01()
            {
                ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainerV3(BCFv3ImportTestCases.UserAssignment);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "7ad1a717-bf20-4c12-b511-cbd90370ddba");
            }

            [Fact]
            public void TopicPresent()
            {
                Assert.NotNull(ReadTopic);
            }

            [Fact]
            public void CheckAssignedTo()
            {
                Assert.Equal("Architect@example.com", ReadTopic.Markup.Topic.AssignedTo);
            }

            [Fact]
            public void CheckCommentCount()
            {
                var expected = 1;
                var actual = ReadTopic.Markup.Topic.Comments.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var expected = "dd6119c6-745b-4575-974d-e0ae54953e0d";
                Assert.Contains(ReadTopic.Markup.Topic.Comments, curr => curr.Guid == expected);
            }

            [Fact]
            public void Markup_HeaderSectionPresent()
            {
                Assert.True(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 1;
                var actual = ReadTopic.Markup.Topic.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var expected = 1;
                var actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }
        }
    }
}