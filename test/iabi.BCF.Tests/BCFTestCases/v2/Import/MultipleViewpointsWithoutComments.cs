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
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.MultipleViewpointsWithoutComments);
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
            var expected = "03d501f8-1025-462f-841b-35846cb36c31";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleViewpointsWithoutComments), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.MultipleViewpointsWithoutComments);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "03d501f8-1025-462f-841b-35846cb36c31");
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
                var expected = 6;
                var actual = ReadTopic.Markup.Comment.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckCommentGuid_01()
            {
                var expected = "2d80a5da-2296-47fc-9fe5-a60b5a02cfb3";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_02()
            {
                var expected = "55f6b17c-2c02-4e0e-99de-64c5f1f255c3";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_03()
            {
                var expected = "e244a1ee-1972-45e3-b919-5ab5002f96ce";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_04()
            {
                var expected = "992a3421-a60b-4594-864b-583feaed9d16";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_05()
            {
                var expected = "12fdc962-6ce8-4494-8224-56642068c84f";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void CheckCommentGuid_06()
            {
                var expected = "ab78330d-c14d-4ca6-9cbb-662be812c9b1";
                Assert.Contains(ReadTopic.Markup.Comment, curr => curr.Guid == expected);
            }

            [Fact]
            public void NoCommentReferencesViewpoint()
            {
                Assert.True(ReadTopic.Markup.Comment.All(curr => !curr.ShouldSerializeViewpoint()));
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Single(ReadTopic.Markup.Header);
            }

            [Fact]
            public void Markup_HeaderFileCorrect_01()
            {
                var headerEntry = ReadTopic.Markup.Header.First();

                Assert.Equal(new DateTime(2015, 06, 09, 06, 39, 06), headerEntry.Date.ToUniversalTime());
                Assert.Equal("C:\\e.ifc", headerEntry.Filename);
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", headerEntry.IfcProject);
                Assert.Null(headerEntry.IfcSpatialStructureElement);
                Assert.True(headerEntry.isExternal);
                Assert.Null(headerEntry.Reference);
                Assert.True(headerEntry.ShouldSerializeDate());
                Assert.True(headerEntry.ShouldSerializeFilename());
                Assert.True(headerEntry.ShouldSerializeIfcProject());
                Assert.False(headerEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "d4568303-f5b8-4d29-b3aa-c79d7262f6d6";
                var actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount_InMarkup()
            {
                var expected = 6;
                var actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void CheckViewpointCount()
            {
                var expected = 6;
                var actual = ReadTopic.Viewpoints.Count;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleViewpointsWithoutComments).GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/d4568303-f5b8-4d29-b3aa-c79d7262f6d6.png");
                var actual = ReadTopic.ViewpointSnapshots["d4568303-f5b8-4d29-b3aa-c79d7262f6d6"];
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_02()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleViewpointsWithoutComments).GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/03c51a6f-13e0-4579-8a54-c6815630d63c.png");
                var actual = ReadTopic.ViewpointSnapshots["03c51a6f-13e0-4579-8a54-c6815630d63c"];
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_03()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleViewpointsWithoutComments).GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["ac1c4776-4a20-49fc-bf58-dcd0c8bc2058"];
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_04()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleViewpointsWithoutComments).GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/0138577b-6590-478a-af2b-7674fa87d821.png");
                var actual = ReadTopic.ViewpointSnapshots["0138577b-6590-478a-af2b-7674fa87d821"];
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_05()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleViewpointsWithoutComments).GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/80adf3ef-d542-40f3-9b9e-5057caabc023.png");
                var actual = ReadTopic.ViewpointSnapshots["80adf3ef-d542-40f3-9b9e-5057caabc023"];
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_CompareSnapshotBinary_06()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.MultipleViewpointsWithoutComments).GetBinaryData("03d501f8-1025-462f-841b-35846cb36c31/1f9ea9fc-0617-43aa-a227-bd8c4abc1c53.png");
                var actual = ReadTopic.ViewpointSnapshots["1f9ea9fc-0617-43aa-a227-bd8c4abc1c53"];
                Assert.True(expected.SequenceEqual(actual));
            }

            [Fact]
            public void Viewpoint_NoOrthogonalCamera()
            {
                var actual = ReadTopic.Viewpoints.First();
                Assert.False(actual.ShouldSerializeOrthogonalCamera());
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