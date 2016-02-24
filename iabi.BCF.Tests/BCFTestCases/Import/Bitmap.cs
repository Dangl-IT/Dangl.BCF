using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class Bitmap
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.Bitmap);
        }

        [TestMethod]
        public void ReadSuccessfullyNotNull()
        {
            Assert.IsNotNull(ReadContainer);
        }

        [TestMethod]
        public void CheckTopicGuid()
        {
            var Expected = "3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51";
            var Actual = ReadContainer.Topics.First().Markup.Topic.Guid;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckTopicCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckCommentGuid()
        {
            var Expected = "9db6d0fc-3539-485e-9f11-1912de64c408";
            var Actual = ReadContainer.Topics.First().Markup.Comment.First().Guid;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckCommentCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Comment.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckViewpointGuid_InMarkup()
        {
            var Expected = "d1514fd3-290b-4830-b1fa-5bb780ce9e94";
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.First().Guid;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckViewpointCount_InMarkup()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Markup.Viewpoints.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckViewpointCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().Viewpoints.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckViewpointBitmapsCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.First().ViewpointBitmaps.First().Value.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void Viewpoint_CompareSnapshotBinary()
        {
            var Expected = BCFTestCasesImportData.Bitmap.GetBinaryData("3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/snapshot.png");
            var Actual = ReadContainer.Topics.First().ViewpointSnapshots.First().Value;
            Assert.IsTrue(Expected.SequenceEqual(Actual));
        }

        [TestMethod]
        public void Viewpoint_CompareBitmapBinary()
        {
            var Expected = BCFTestCasesImportData.Bitmap.GetBinaryData("3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png");
            var Actual = ReadContainer.Topics.First().ViewpointBitmaps.First().Value.First();
            Assert.IsTrue(Expected.SequenceEqual(Actual));
        }

        [TestMethod]
        public void Viewpoint_CompareBitmapData()
        {
            var ActualBitmap = ReadContainer.Topics.First().Viewpoints.First().Bitmaps.First();

            Assert.AreEqual(BitmapFormat.PNG, ActualBitmap.Bitmap);
            Assert.AreEqual(1666.1814563907683, ActualBitmap.Height);
            // Actual value in file as of 2015-09-24: 3f6ac03e-de8e-4c5e-b3f0-c7bf5f87fe51/bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png
            // Should be made to relative reference when read
            Assert.AreEqual("bitmaps-d1514fd3-290b-4830-b1fa-5bb780ce9e94-0.png", ActualBitmap.Reference); // Should be corrected

            Assert.AreEqual(10.064999999983305, ActualBitmap.Location.X);
            Assert.AreEqual(-10.40177106506878, ActualBitmap.Location.Y);
            Assert.AreEqual(7.011243681990698, ActualBitmap.Location.Z);
            Assert.AreEqual(-0.9999999999999999, ActualBitmap.Normal.X);
            Assert.AreEqual(1.253656364893038E-16, ActualBitmap.Normal.Y);
            Assert.AreEqual(0.0, ActualBitmap.Normal.Z);
            Assert.AreEqual(-5.43903050550883E-34, ActualBitmap.Up.X);
            Assert.AreEqual(-4.338533794284917E-18, ActualBitmap.Up.Y);
            Assert.AreEqual(1.0, ActualBitmap.Up.Z);
        }

        [TestMethod]
        public void WriteOut()
        {
            var MemStream = new MemoryStream();
            ReadContainer.WriteStream(MemStream);
            var Data = MemStream.ToArray();
            Assert.IsNotNull(Data);
            Assert.IsTrue(Data.Length > 0);
        }

        [TestMethod]
        public void WriteAndCompare()
        {
            var MemStream = new MemoryStream();
            ReadContainer.WriteStream(MemStream);
            var Data = MemStream.ToArray();
            CompareTool.CompareFiles(BCFTestCasesImportData.Bitmap, Data);
        }
    }
}