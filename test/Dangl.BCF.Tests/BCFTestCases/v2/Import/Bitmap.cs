using System.IO;
using System.Linq;
using Dangl.BCF.BCFv2;
using Dangl.BCF.BCFv2.Schemas;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v2.Import
{
    public class Bitmap
    {
        public BCFv2Container ReadContainer;

        public Bitmap()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.Bitmap);
        }

        [Fact]
        public void CanConverterToBcfV21Container()
        {
            var converter = new Dangl.BCF.Converter.V2ToV21(ReadContainer);
            var upgradedContainer = converter.Convert();
            Assert.NotNull(upgradedContainer);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicGuid()
        {
            var expected = "3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51";
            var actual = ReadContainer.Topics.First().Markup.Topic.Guid;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckCommentGuid()
        {
            var expected = "9db6d0fc-3539-485e-9f11-1912de64c408";
            var actual = ReadContainer.Topics.First().Markup.Comment.First().Guid;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckCommentCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var expected = "d1514fd3-290b-4830-b1fa-5bb780ce9e94";
            var actual = ReadContainer.Topics.First().Markup.Viewpoints.First().Guid;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckViewpointCount_InMarkup()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().Markup.Viewpoints.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckViewpointCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().Viewpoints.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckViewpointBitmapsCount()
        {
            var expected = 1;
            var actual = ReadContainer.Topics.First().ViewpointBitmaps.First().Value.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Viewpoint_CompareSnapshotBinary()
        {
            var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Bitmap).GetBinaryData("3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/snapshot.png");
            var actual = ReadContainer.Topics.First().ViewpointSnapshots.First().Value;
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void Viewpoint_CompareBitmapBinary()
        {
            var expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Bitmap).GetBinaryData("3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png");
            var actual = ReadContainer.Topics.First().ViewpointBitmaps.First().Value.First();
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void Viewpoint_CompareBitmapData()
        {
            var actualBitmap = ReadContainer.Topics.First().Viewpoints.First().Bitmaps.First();

            Assert.Equal(BitmapFormat.PNG, actualBitmap.Bitmap);
            Assert.Equal(1666.1814563907683, actualBitmap.Height);
            // Actual value in file as of 2015-09-24: 3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png
            // Should be made to relative reference when read
            Assert.Equal("bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png", actualBitmap.Reference); // Should be corrected

            Assert.Equal(10.064999999983305, actualBitmap.Location.X);
            Assert.Equal(-10.40177106506878, actualBitmap.Location.Y);
            Assert.Equal(7.011243681990698, actualBitmap.Location.Z);
            Assert.Equal(-0.9999999999999999, actualBitmap.Normal.X);
            Assert.Equal(1.253656364893038E-16, actualBitmap.Normal.Y);
            Assert.Equal(0.0, actualBitmap.Normal.Z);
            Assert.Equal(-5.43903050550883E-34, actualBitmap.Up.X);
            Assert.Equal(-4.338533794284917E-18, actualBitmap.Up.Y);
            Assert.Equal(1.0, actualBitmap.Up.Z);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Bitmap), data);
        }
    }
}