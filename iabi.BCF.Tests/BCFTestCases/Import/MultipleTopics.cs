using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Xunit;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
     
    public class MultipleTopics
    {
        public static BCFv2Container ReadContainer;

                public static void Create()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var Expected = 3;
            var Actual = ReadContainer.Topics.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckTopicGuid_01()
        {
            var Expected = "33b0e849-72f1-434c-88e1-b7b3b8c27f38";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

        [Fact]
        public void CheckTopicGuid_02()
        {
            var Expected = "402d5148-88ef-4510-8b9d-e632602541c6";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

        [Fact]
        public void CheckTopicGuid_03()
        {
            var Expected = "f5d76cd4-46bc-450c-a513-c1f62ac24026";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

        /// <summary>
        /// This topic references the extensions.xsd inside its project.bcfp but there are not extensions
        /// </summary>
        [Fact]
        public void DontImportReferenceToExtensions()
        {
            Assert.True(string.IsNullOrWhiteSpace(ReadContainer.BCFProject.ExtensionSchema));
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "33b0e849-72f1-434c-88e1-b7b3b8c27f38");
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
                var Expected = "88f24705-2785-4928-8d82-56bb998c91b9";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "88f24705-2785-4928-8d82-56bb998c91b9";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("cb8bccde-ce99-4a95-859c-1b7f5031997d", Comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(0, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "cb8bccde-ce99-4a95-859c-1b7f5031997d";
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
                var Expected = BCFTestCasesImportData.multiple_topics_bcfzip.GetBinaryData("33b0e849-72f1-434c-88e1-b7b3b8c27f38/snapshot.png");
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
                Assert.Equal(0, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_DontSerializeComponents()
            {
                Assert.False(ReadTopic.Viewpoints.First().ShouldSerializeComponents());
            }

            [Fact]
            public void Viewpoint_LinesCountCorrect()
            {
                Assert.Equal(0, ReadTopic.Viewpoints.First().Lines.Count);
            }
        }

         
        public class Topic_02
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "402d5148-88ef-4510-8b9d-e632602541c6");
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
                var Expected = "a3e4a0d7-49ed-4a9b-93d6-1f99266417c3";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "a3e4a0d7-49ed-4a9b-93d6-1f99266417c3";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("5485542b-5bb6-4314-90df-537f8246473", Comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(0, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "5485542b-5bb6-4314-90df-537f8246473f";
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
                var Expected = BCFTestCasesImportData.multiple_topics_bcfzip.GetBinaryData("402d5148-88ef-4510-8b9d-e632602541c6/snapshot.png");
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
                Assert.Equal(0, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_DontSerializeComponents()
            {
                Assert.False(ReadTopic.Viewpoints.First().ShouldSerializeComponents());
            }

            [Fact]
            public void Viewpoint_LinesCountCorrect()
            {
                Assert.Equal(0, ReadTopic.Viewpoints.First().Lines.Count);
            }
        }

         
        public class Topic_03
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleTopics);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "f5d76cd4-46bc-450c-a513-c1f62ac24026");
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
                var Expected = "1e7bc348-8807-4d4a-bdbd-fab6761076a1";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "1e7bc348-8807-4d4a-bdbd-fab6761076a1";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("2f5041e7-1b8b-4310-abff-e0e7e620b878", Comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(0, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "2f5041e7-1b8b-4310-abff-e0e7e620b878";
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
                var Expected = BCFTestCasesImportData.multiple_topics_bcfzip.GetBinaryData("f5d76cd4-46bc-450c-a513-c1f62ac24026/snapshot.png");
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
                Assert.Equal(0, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_DontSerializeComponents()
            {
                Assert.False(ReadTopic.Viewpoints.First().ShouldSerializeComponents());
            }

            [Fact]
            public void Viewpoint_LinesCountCorrect()
            {
                Assert.Equal(0, ReadTopic.Viewpoints.First().Lines.Count);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.multiple_topics_bcfzip, Data);
        }
    }
}
