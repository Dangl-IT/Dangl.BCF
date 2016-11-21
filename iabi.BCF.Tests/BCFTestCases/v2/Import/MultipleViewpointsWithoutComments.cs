using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class MultipleViewpointsWithoutComments
    {
        public BCFv2Container ReadContainer;

        public MultipleViewpointsWithoutComments()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleViewpointsWithoutComments);
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
            var Expected = "03d501f8-1025-462f-841b-35846cb36c31";
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
            CompareTool.CompareFiles(BCFTestCasesImportData.multiple_viewpoints_without_comments, Data);
        }


        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic_01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.MultipleViewpointsWithoutComments);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "03d501f8-1025-462f-841b-35846cb36c31");
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
                var Expected = 6;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var Expected = "2d80a5da-2296-47fc-9fe5-a60b5a02cfb3";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentGuid_02()
            {
                var Expected = "55f6b17c-2c02-4e0e-99de-64c5f1f255c3";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentGuid_03()
            {
                var Expected = "e244a1ee-1972-45e3-b919-5ab5002f96ce";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentGuid_04()
            {
                var Expected = "992a3421-a60b-4594-864b-583feaed9d16";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentGuid_05()
            {
                var Expected = "12fdc962-6ce8-4494-8224-56642068c84f";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [Fact]
            public void CheckCommentGuid_06()
            {
                var Expected = "ab78330d-c14d-4ca6-9cbb-662be812c9b1";
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
                Assert.Equal(true, HeaderEntry.ShouldSerializeDate());
                Assert.Equal(true, HeaderEntry.ShouldSerializeFilename());
                Assert.Equal(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.Equal(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "d4568303-f5b8-4d29-b3aa-c79d7262f6d6";
                var Actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var Expected = 6;
                var Actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var Expected = 6;
                var Actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/d4568303-f5b8-4d29-b3aa-c79d7262f6d6.png");
                var Actual = ReadTopic.ViewpointSnapshots["d4568303-f5b8-4d29-b3aa-c79d7262f6d6"];
                Assert.True(Expected.SequenceEqual(Actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_02()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/03c51a6f-13e0-4579-8a54-c6815630d63c.png");
                var Actual = ReadTopic.ViewpointSnapshots["03c51a6f-13e0-4579-8a54-c6815630d63c"];
                Assert.True(Expected.SequenceEqual(Actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_03()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["ac1c4776-4a20-49fc-bf58-dcd0c8bc2058"];
                Assert.True(Expected.SequenceEqual(Actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_04()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/0138577b-6590-478a-af2b-7674fa87d821.png");
                var Actual = ReadTopic.ViewpointSnapshots["0138577b-6590-478a-af2b-7674fa87d821"];
                Assert.True(Expected.SequenceEqual(Actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_05()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/80adf3ef-d542-40f3-9b9e-5057caabc023.png");
                var Actual = ReadTopic.ViewpointSnapshots["80adf3ef-d542-40f3-9b9e-5057caabc023"];
                Assert.True(Expected.SequenceEqual(Actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_06()
            {
                var Expected = BCFTestCasesImportData.multiple_viewpoints_without_comments.GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/1f9ea9fc-0617-43aa-a227-bd8c4abc1c53.png");
                var Actual = ReadTopic.ViewpointSnapshots["1f9ea9fc-0617-43aa-a227-bd8c4abc1c53"];
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
    }
}