using System;
using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class VisibleSpaceAndRestOfModelVisible
    {
        public BCFv2Container ReadContainer;

        public VisibleSpaceAndRestOfModelVisible()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.VisibleSpaceAndRestOfModelVisible);
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
            var expected = "79d7bbbc-3029-43bf-91fd-47ef0915e7ae";
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.VisibleSpaceAndRestOfModelVisible), data);
        }


        public class Topic01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            public Topic01()
            {
                if (ReadContainer == null)
                {
                    ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.VisibleSpaceAndRestOfModelVisible);
                }
                if (ReadTopic == null)
                {
                    ReadTopic = ReadContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == "79d7bbbc-3029-43bf-91fd-47ef0915e7ae");
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
                var expected = "691ed8dc-0d52-498d-b2d3-d858a98b9a43";
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
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.VisibleSpaceAndRestOfModelVisible).GetBinaryData("79d7bbbc-3029-43bf-91fd-47ef0915e7ae/snapshot.png");
                var actual = ReadTopic.ViewpointSnapshots["756d6918-4ee9-4c88-9c74-b00c6c2410e5"];
                Assert.True(expected.SequenceEqual(actual));
            }
        }
    }
}