﻿using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class DecomposedObjectsWithParentGuid
    {
        public BCFv2Container ReadContainer;

        public DecomposedObjectsWithParentGuid()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DecomposedObjectsWithParentGuid);
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
            var expected = "a23e8824-137a-4bea-a1ad-541f87d274e7";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DecomposedObjectsWithParentGuid), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.DecomposedObjectsWithParentGuid);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "a23e8824-137a-4bea-a1ad-541f87d274e7");
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
                var expected = 0;
                var actual = ReadTopic.Markup.Comment.Count;
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
                Assert.Equal(true, headerEntry.DateSpecified);
                Assert.Equal("C:\\e.ifc", headerEntry.Filename);
                Assert.Equal("2SugUv4EX5LAhcVpDp2dUH", headerEntry.IfcProject);
                Assert.Equal(null, headerEntry.IfcSpatialStructureElement);
                Assert.Equal(true, headerEntry.isExternal);
                Assert.Equal(null, headerEntry.Reference);
                Assert.Equal(true, headerEntry.ShouldSerializeDate());
                Assert.Equal(true, headerEntry.ShouldSerializeFilename());
                Assert.Equal(true, headerEntry.ShouldSerializeIfcProject());
                Assert.Equal(false, headerEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [Fact]
            public void CheckViewpointGuid_InMarkup()
            {
                var expected = "53968000-0c3e-41c8-864b-83ede8d7b443";
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
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.DecomposedObjectsWithParentGuid).GetBinaryData("a23e8824-137a-4bea-a1ad-541f87d274e7/snapshot.png");
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
                Assert.Equal("2_hQ1Rixj6lgHTra$L72O4", component.IfcGuid);
                Assert.Equal("Allplan", component.OriginatingSystem);
                Assert.Equal(false, component.Selected);
                Assert.Equal(true, component.SelectedSpecified);
                Assert.Equal(true, component.Visible);
            }
        }
    }
}