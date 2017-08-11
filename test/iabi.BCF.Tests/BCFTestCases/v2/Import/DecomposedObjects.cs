using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class DecomposedObjects
    {
        public BCFv2Container ReadContainer;

        public DecomposedObjects()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DecomposedObjects);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var expected = 3;
            var actual = ReadContainer.Topics.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckTopicGuid_01()
        {
            var expected = "6da43897-f4ff-4694-97dc-fc4a43770749";
            var actual = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(actual);
        }

        [Fact]
        public void CheckTopicGuid_02()
        {
            var expected = "24e5625c-8ff1-40f9-81f2-f31cfa48cf74";
            var actual = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(actual);
        }

        [Fact]
        public void CheckTopicGuid_03()
        {
            var expected = "eb59ce15-5713-47ed-8505-ccccd91b4170";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DecomposedObjects), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DecomposedObjects);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "eb59ce15-5713-47ed-8505-ccccd91b4170");
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
                var expected = "8124dadb-2470-4cda-b010-d5a75c77bb64";
                Assert.True(ReadTopic.Markup.Comment.Any(curr => curr.Guid == expected));
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(1, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void Markup_HeaderFileCorrect_01()
            {
                var headerEntry = ReadTopic.Markup.Header.First();

                Assert.Equal(new DateTime(2015, 06, 09, 06, 39, 06), headerEntry.Date.ToUniversalTime());
                Assert.True(headerEntry.DateSpecified);
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
            public void CheckCommentViewpointReference_01()
            {
                var commentGuid = "8124dadb-2470-4cda-b010-d5a75c77bb64";
                var comment = ReadTopic.Markup.Comment.FirstOrDefault(curr => curr.Guid == commentGuid);
                Assert.False(comment.ShouldSerializeViewpoint());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "9c1d0d48-2255-46de-b979-f31811a2d9c5";
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
            public void Viewpoint_CompareSnapshotBinary()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DecomposedObjects).GetBinaryData("eb59ce15-5713-47ed-8505-ccccd91b4170/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots.First().Value;
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
                Assert.Equal(1, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_ComponentCorrect_01()
            {
                var component = ReadTopic.Viewpoints.First().Components.First();
                Assert.False(component.ShouldSerializeAuthoringToolId());
                Assert.Null(component.Color);
                Assert.Equal("14rw$C3xD3ZeUJL3YaqCEK", component.IfcGuid);
                Assert.Equal("Allplan", component.OriginatingSystem);
                Assert.False(component.Selected);
                Assert.True(component.SelectedSpecified);
                Assert.True(component.Visible);
            }
        }


        public class Topic02
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic02()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DecomposedObjects);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "24e5625c-8ff1-40f9-81f2-f31cfa48cf74");
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
                var expected = "c7548286-b8cc-4f00-8dad-401e50d23921";
                Assert.True(ReadTopic.Markup.Comment.Any(curr => curr.Guid == expected));
            }

            [Fact]
            public void CheckCommentTestEmpty()
            {
                var comment = ReadTopic.Markup.Comment.FirstOrDefault();
                Assert.False(comment.ShouldSerializeComment1());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "e19d7207-89a7-4755-97c6-452d7bfd0abf";
                var actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(1, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void Markup_HeaderFileCorrect_01()
            {
                var headerEntry = ReadTopic.Markup.Header.First();

                Assert.Equal(new DateTime(2015, 06, 09, 06, 39, 06), headerEntry.Date.ToUniversalTime());
                Assert.True(headerEntry.DateSpecified);
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
            public void Viewpoint_CompareSnapshotBinary()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DecomposedObjects).GetBinaryData("24e5625c-8ff1-40f9-81f2-f31cfa48cf74/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots.First().Value;
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
                Assert.Equal(36, ReadTopic.Viewpoints.First().Components.Count);
            }
        }


        public class Topic03
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic03()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DecomposedObjects);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "6da43897-f4ff-4694-97dc-fc4a43770749");
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
                var expected = "e0b7a3d5-75c6-4a7e-a25e-54cd699d4f0e";
                Assert.True(ReadTopic.Markup.Comment.Any(curr => curr.Guid == expected));
            }

            [Fact]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.Equal(1, ReadTopic.Markup.Header.Count);
            }

            [Fact]
            public void Markup_HeaderFileCorrect_01()
            {
                var headerEntry = ReadTopic.Markup.Header.First();

                Assert.Equal(new DateTime(2015, 06, 09, 06, 39, 06), headerEntry.Date.ToUniversalTime());
                Assert.True(headerEntry.DateSpecified);
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
                var expected = "8a626543-d4ec-4fec-9d1c-7d1a8811a69d";
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
            public void Viewpoint_CompareSnapshotBinary()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DecomposedObjects).GetBinaryData("6da43897-f4ff-4694-97dc-fc4a43770749/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots.First().Value;
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
                Assert.Equal(37, ReadTopic.Viewpoints.First().Components.Count);
            }
        }
    }
}