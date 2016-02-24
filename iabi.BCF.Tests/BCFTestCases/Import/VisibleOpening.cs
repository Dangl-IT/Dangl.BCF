using iabi.BCF.BCFv2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class VisibleOpening
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.VisibleOpening);
        }

        [TestMethod]
        public void ReadSuccessfullyNotNull()
        {
            Assert.IsNotNull(ReadContainer);
        }

        [TestMethod]
        public void CheckTopicCount()
        {
            var Expected = 1;
            var Actual = ReadContainer.Topics.Count;
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void CheckTopicGuid_01()
        {
            var Expected = "b215f0e9-b5fa-490b-8674-d5e01607728d";
            var Actual = ReadContainer.Topics.Any(Curr => Curr.Markup.Topic.Guid == Expected);
            Assert.IsTrue(Actual);
        }

        [TestClass]
        public class Topic_01
        {
            public static BCFv2Container ReadContainer;

            public static BCFTopic ReadTopic;

            [ClassInitialize]
            public static void Create(TestContext GivenContext)
            {
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.VisibleOpening);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "b215f0e9-b5fa-490b-8674-d5e01607728d");
            }

            [TestMethod]
            public void TopicPresent()
            {
                Assert.IsNotNull(ReadTopic);
            }

            [TestMethod]
            public void CheckCommentCount()
            {
                var Expected = 1;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckCommentGuid_01()
            {
                var Expected = "2143a906-d693-4c2b-8970-619d01c28a97";
                Assert.IsTrue(ReadTopic.Markup.Comment.Any(Curr => Curr.Guid == Expected));
            }

            [TestMethod]
            public void NoCommentReferencesViewpoint()
            {
                Assert.IsTrue(ReadTopic.Markup.Comment.All(Curr => !Curr.ShouldSerializeViewpoint()));
            }

            [TestMethod]
            public void Markup_HeaderFilesCountCorrect()
            {
                Assert.AreEqual(1, ReadTopic.Markup.Header.Count);
            }

            [TestMethod]
            public void Markup_HeaderFileCorrect_01()
            {
                var HeaderEntry = ReadTopic.Markup.Header.First();

                Assert.AreEqual(new DateTime(2015, 06, 09, 06, 39, 06), HeaderEntry.Date.ToUniversalTime());
                Assert.AreEqual("C:\\e.ifc", HeaderEntry.Filename);
                Assert.AreEqual("2SugUv4EX5LAhcVpDp2dUH", HeaderEntry.IfcProject);
                Assert.AreEqual(null, HeaderEntry.IfcSpatialStructureElement);
                Assert.AreEqual(true, HeaderEntry.isExternal);
                Assert.AreEqual(null, HeaderEntry.Reference);
                Assert.AreEqual(true, HeaderEntry.ShouldSerializeIfcProject());
                Assert.AreEqual(false, HeaderEntry.ShouldSerializeIfcSpatialStructureElement());
            }

            [TestMethod]
            public void CheckViewpointCount_InMarkup()
            {
                var Expected = 1;
                var Actual = ReadTopic.Markup.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void CheckViewpointCount()
            {
                var Expected = 1;
                var Actual = ReadTopic.Viewpoints.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void Viewpoint_CompareSnapshotBinary_01()
            {
                var Expected = BCFTestCasesImportData.Visible_Opening.GetBinaryData("b215f0e9-b5fa-490b-8674-d5e01607728d/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["68190c8c-01e4-4852-a01d-66d756a520f3"];
                Assert.IsTrue(Expected.SequenceEqual(Actual));
            }
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
            CompareTool.CompareFiles(BCFTestCasesImportData.Visible_Opening, Data);
        }
    }
}