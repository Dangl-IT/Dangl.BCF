using System.IO;
using System.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public class Bitmap
    {
        public BCFv2Container ReadContainer;

        public Bitmap()
        {
            ReadContainer = TestCaseResourceFactory.GetImportTestCaseContainer(BCFv2ImportTestCases.Bitmap);
        }

        [Fact]
        public void ReadSuccessfullyNotNull()
        {
            Assert.NotNull(ReadContainer);
        }

        [Fact]
        public void CheckTopicGuid()
        {
            var Expected = "3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51";
            var Actual = ReadContainer.Topics.First().Markup.Topic.Guid;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckTopicCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckCommentGuid()
        {
            var Expected = "9db6d0fc-3539-485e-9f11-1912de64c408";
            var Actual = ReadContainer.Topics.First().Markup.Comment.First().Guid;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckCommentCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "d1514fd3-290b-4830-b1fa-5bb780ce9e94";
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.First().Guid;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckViewpointCount_InMarkup()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckViewpointCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Viewpoints.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void CheckViewpointBitmapsCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().ViewpointBitmaps.First().Value.Count;
            Assert.Equal(Expected, Actual);
        }

        [Fact]
        public void Viewpoint_CompareSnapshotBinary()
        {
            var Expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Bitmap).GetBinaryData("3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/snapshot.png");
            var Actual = ReadContainer.Topics.First().ViewpointSnapshots.First().Value;
            Assert.True(Expected.SequenceEqual(Actual));
        }

        [Fact]
        public void Viewpoint_CompareBitmapBinary()
        {
            var Expected = TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Bitmap).GetBinaryData("3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png");
            var Actual = ReadContainer.Topics.First().ViewpointBitmaps.First().Value.First();
            Assert.True(Expected.SequenceEqual(Actual));
        }

        [Fact]
        public void Viewpoint_CompareBitmapData()
        {
            var ActualBitmap = ReadContainer.Topics.First().Viewpoints.First().Bitmaps.First();

            Assert.Equal(BitmapFormat.PNG, ActualBitmap.Bitmap);
            Assert.Equal(1666.1814563907683, ActualBitmap.Height);
            // Actual value in file as of 2015-09-24: 3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png
            // Should be made to relative reference when read
            Assert.Equal("bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png", ActualBitmap.Reference); // Should be corrected

            Assert.Equal(10.064999999983305, ActualBitmap.Location.X);
            Assert.Equal(-10.40177106506878, ActualBitmap.Location.Y);
            Assert.Equal(7.011243681990698, ActualBitmap.Location.Z);
            Assert.Equal(-0.9999999999999999, ActualBitmap.Normal.X);
            Assert.Equal(1.253656364893038E-16, ActualBitmap.Normal.Y);
            Assert.Equal(0.0, ActualBitmap.Normal.Z);
            Assert.Equal(-5.43903050550883E-34, ActualBitmap.Up.X);
            Assert.Equal(-4.338533794284917E-18, ActualBitmap.Up.Y);
            Assert.Equal(1.0, ActualBitmap.Up.Z);
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
            CompareTool.CompareFiles(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Bitmap), Data);
        }
    }
}