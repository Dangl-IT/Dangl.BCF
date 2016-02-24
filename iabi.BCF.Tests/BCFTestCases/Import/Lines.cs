using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.Import
{
     
    public class Lines
    {
        public static BCFv2Container ReadContainer;

                public static void Create()
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.Lines);
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
            var Expected = "64124059-3964-4c0b-9987-11036e9f0d54";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.True(Actual);
        }

         
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

                        public static void Create()
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.Lines);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "64124059-3964-4c0b-9987-11036e9f0d54");
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
                var Expected = "ec54ee6d-3ed0-45d8-8c80-398f9f8d80da";
                Assert.True(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
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
                var Expected = "6e225887-aad7-42ec-a9f4-8f8c796832dd";
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
                var Expected = BCFTestCasesImportData.Lines.GetBinaryData("64124059-3964-4c0b-9987-11036e9f0d54/snapshot.png");
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
        }

        [Fact]
        public void Viewpoint_LinesCountCorrect()
        {
            Assert.Equal(4, ReadContainer.Topics.First().Viewpoints.First().Lines.Count);
        }

        [Fact]
        public void Viewpoint_LinesCorrect_01()
        {
            var Actual = ReadContainer.Topics.First().Viewpoints.First().Lines.First();
            Assert.NotNull(Actual);

            Assert.Equal(14.081214128865833, Actual.StartPoint.X);
            Assert.Equal(-15.361069061775849, Actual.StartPoint.Y);
            Assert.Equal(8.124594766348617, Actual.StartPoint.Z);
            Assert.Equal(14.069056488704259, Actual.EndPoint.X);
            Assert.Equal(-15.546558805373634, Actual.EndPoint.Y);
            Assert.Equal(12.340820025706794, Actual.EndPoint.Z);
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Lines, Data);
        }
    }
}
