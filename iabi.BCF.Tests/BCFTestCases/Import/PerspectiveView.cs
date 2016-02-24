using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Xunit;
using System;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
     
    public class PerspectiveView
    {
        public static BCFv2Container ReadContainer;

                public static void Create()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.PerspectiveView);
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
            var Expected = "8bcb4942-a716-4e4f-b699-e1c150a50594";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.PerspectiveView);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "8bcb4942-a716-4e4f-b699-e1c150a50594");
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
                var Expected = "4b181afe-c628-4516-9c19-b0dce3a49a47";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckComment_ModifiedData()
            {
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == "4b181afe-c628-4516-9c19-b0dce3a49a47");
                Assert.Equal("pbuts@kubusinfo.nl", Comment.ModifiedAuthor);
                Assert.Equal(new DateTime(2015, 06, 09, 07, 58, 02), Comment.ModifiedDate.ToUniversalTime());
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var CommentGuid = "4b181afe-c628-4516-9c19-b0dce3a49a47";
                var Comment = ReadTopic.Markup.Comment.FirstOrDefault(Curr => Curr.Guid == CommentGuid);
                Assert.True(Comment.ShouldSerializeViewpoint());
                Assert.Equal("8ab3bf5c-3c8d-4f49-b974-8968f94b59d4", Comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "8ab3bf5c-3c8d-4f49-b974-8968f94b59d4";
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
                var Expected = BCFTestCasesImportData.Perspective_view.GetBinaryData("8bcb4942-a716-4e4f-b699-e1c150a50594/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["8ab3bf5c-3c8d-4f49-b974-8968f94b59d4"];
                Assert.True(Expected.SequenceEqual(Actual));
            }

            [Fact]
            public void Viewpoint_NoOrthogonalCamera()
            {
                var Actual = ReadTopic.Viewpoints.First();
                Assert.False(Actual.ShouldSerializeOrthogonalCamera());
            }

            [Fact]
            public void Viewpoint_PerspectiveCamera()
            {
                var Actual = ReadTopic.Viewpoints.First();
                Assert.True(Actual.ShouldSerializePerspectiveCamera());

                Assert.Equal(21.75835377599418, Actual.PerspectiveCamera.CameraViewPoint.X);
                Assert.Equal(-19.69042708255157, Actual.PerspectiveCamera.CameraViewPoint.Y);
                Assert.Equal(19.20322065558115, Actual.PerspectiveCamera.CameraViewPoint.Z);
                Assert.Equal(-0.53672118533613, Actual.PerspectiveCamera.CameraDirection.X);
                Assert.Equal(0.35874211235957, Actual.PerspectiveCamera.CameraDirection.Y);
                Assert.Equal(-0.76369788924101, Actual.PerspectiveCamera.CameraDirection.Z);
                Assert.Equal(-0.63492792770306, Actual.PerspectiveCamera.CameraUpVector.X);
                Assert.Equal(0.42438307300583, Actual.PerspectiveCamera.CameraUpVector.Y);
                Assert.Equal(0.64557380210850, Actual.PerspectiveCamera.CameraUpVector.Z);
                Assert.Equal(70, Actual.PerspectiveCamera.FieldOfView);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Perspective_view, Data);
        }
    }
}
