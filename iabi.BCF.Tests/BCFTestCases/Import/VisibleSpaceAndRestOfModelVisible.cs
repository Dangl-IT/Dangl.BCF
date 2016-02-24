using iabi.BCF.BCFv2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.Import
{
    [TestClass]
    public class VisibleSpaceAndRestOfModelVisible
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.VisibleSpaceAndRestOfModelVisible);
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
            var Expected = "79d7bbbc-3029-43bf-91fd-47ef0915e7ae";
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
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.VisibleSpaceAndRestOfModelVisible);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "79d7bbbc-3029-43bf-91fd-47ef0915e7ae");
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
                var Expected = "691ed8dc-0d52-498d-b2d3-d858a98b9a43";
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
                var Expected = BCFTestCasesImportData.visible_space_and_the_rest_of_the_model_visible.GetBinaryData("79d7bbbc-3029-43bf-91fd-47ef0915e7ae/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["756d6918-4ee9-4c88-9c74-b00c6c2410e5"];
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
            CompareTool.CompareFiles(BCFTestCasesImportData.visible_space_and_the_rest_of_the_model_visible, Data);
        }
    }
}