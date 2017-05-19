using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class Lines
    {
        public BCFv2Container ReadContainer;

        public Lines()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.Lines);
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
            var expected = "64124059-3964-4c0b-9987-11036e9f0d54";
            var actual = ReadContainer.Topics.Any(curr => curr.Markup.Topic.Guid == expected);
            Assert.True(actual);
        }

        [Fact]
        public void Viewpoint_LinesCountCorrect()
        {
            Assert.Equal(4, ReadContainer.Topics.First().Viewpoints.First().Lines.Count);
        }

        [Fact]
        public void Viewpoint_LinesCorrect_01()
        {
            var actual = ReadContainer.Topics.First().Viewpoints.First().Lines.First();
            Assert.NotNull(actual);

            Assert.Equal(14.081214128865833, actual.StartPoint.X);
            Assert.Equal(-15.361069061775849, actual.StartPoint.Y);
            Assert.Equal(8.124594766348617, actual.StartPoint.Z);
            Assert.Equal(14.069056488704259, actual.EndPoint.X);
            Assert.Equal(-15.546558805373634, actual.EndPoint.Y);
            Assert.Equal(12.340820025706794, actual.EndPoint.Z);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Lines), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.Lines);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "64124059-3964-4c0b-9987-11036e9f0d54");
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
                var expected = "ec54ee6d-3ed0-45d8-8c80-398f9f8d80da";
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
                var expected = "6e225887-aad7-42ec-a9f4-8f8c796832dd";
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
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Lines).GetBinaryData("64124059-3964-4c0b-9987-11036e9f0d54/snapshot.png");
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
                Assert.Equal(0, ReadTopic.Viewpoints.First().Components.Count);
            }

            [Fact]
            public void Viewpoint_DontSerializeComponents()
            {
                Assert.False(ReadTopic.Viewpoints.First().ShouldSerializeComponents());
            }
        }
    }
}