using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class PerspectiveView
    {
        public BCFv2Container ReadContainer;

        public PerspectiveView()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.PerspectiveView);
        }

        [Fact]
        public void CanConverterToBcfV21Container()
        {
            var converter = new iabi.BCF.Converter.V2ToV21(ReadContainer);
            var upgradedContainer = converter.Convert();
            Assert.NotNull(upgradedContainer);
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
            var expected = "8bcb4942-a716-4e4f-b699-e1c150a50594";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.PerspectiveView), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.PerspectiveView);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "8bcb4942-a716-4e4f-b699-e1c150a50594");
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
                var expected = "4b181afe-c628-4516-9c19-b0dce3a49a47";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckComment_ModifiedData()
            {
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == "4b181afe-c628-4516-9c19-b0dce3a49a47");
                Assert.Equal("pbuts@kubusinfo.nl", comment.ModifiedAuthor);
                Assert.Equal(new DateTime(2015, 06, 09, 07, 58, 02), comment.ModifiedDate.ToUniversalTime());
            }

            [Fact]
            public void CheckCommentViewpointReference_01()
            {
                var commentGuid = "4b181afe-c628-4516-9c19-b0dce3a49a47";
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
                Assert.True(comment.ShouldSerializeViewpoint());
                Assert.Equal("8ab3bf5c-3c8d-4f49-b974-8968f94b59d4", comment.Viewpoint.Guid);
            }

            [Fact]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.False(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "8ab3bf5c-3c8d-4f49-b974-8968f94b59d4";
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
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.PerspectiveView).GetBinaryData("8bcb4942-a716-4e4f-b699-e1c150a50594/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["8ab3bf5c-3c8d-4f49-b974-8968f94b59d4"];
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_NoOrthogonalCamera()
            {
                var actual = ReadTopic.Viewpoints.First();
                Assert.False(actual.ShouldSerializeOrthogonalCamera());
            }

            [Fact]
            public void Viewpoint_PerspectiveCamera()
            {
                var actual = ReadTopic.Viewpoints.First();
                Assert.True(actual.ShouldSerializePerspectiveCamera());

                Assert.Equal(21.75835377599418, actual.PerspectiveCamera.CameraViewPoint.X);
                Assert.Equal(-19.69042708255157, actual.PerspectiveCamera.CameraViewPoint.Y);
                Assert.Equal(19.20322065558115, actual.PerspectiveCamera.CameraViewPoint.Z);
                Assert.Equal(-0.53672118533613, actual.PerspectiveCamera.CameraDirection.X);
                Assert.Equal(0.35874211235957, actual.PerspectiveCamera.CameraDirection.Y);
                Assert.Equal(-0.76369788924101, actual.PerspectiveCamera.CameraDirection.Z);
                Assert.Equal(-0.63492792770306, actual.PerspectiveCamera.CameraUpVector.X);
                Assert.Equal(0.42438307300583, actual.PerspectiveCamera.CameraUpVector.Y);
                Assert.Equal(0.64557380210850, actual.PerspectiveCamera.CameraUpVector.Z);
                Assert.Equal(70, actual.PerspectiveCamera.FieldOfView);
            }

            [Fact]
            public void Viewpoint_ComponentsCountCorrect()
            {
                Assert.Empty(ReadTopic.Viewpoints.First().Components);
            }

            [Fact]
            public void Viewpoint_DontSerializeComponents()
            {
                Assert.False(ReadTopic.Viewpoints.First().ShouldSerializeComponents());
            }

            [Fact]
            public void Viewpoint_LinesCountCorrect()
            {
                Assert.Empty(ReadTopic.Viewpoints.First().Lines);
            }
        }
    }
}