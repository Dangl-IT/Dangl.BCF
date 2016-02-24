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
    public class SingleVisibleWall
    {
        public static BCFv2Container ReadContainer;

        [ClassInitialize]
        public static void Create(TestContext GivenContext)
        {
            ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleVisibleWall);
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
            var Expected = "74bca551-875c-4455-adaa-2205f2245af3";
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
                ReadContainer = BCFFilesFactory.GetContainerForTest(BCFImportTest.SingleVisibleWall);
                ReadTopic = ReadContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == "74bca551-875c-4455-adaa-2205f2245af3");
            }

            [TestMethod]
            public void TopicPresent()
            {
                Assert.IsNotNull(ReadTopic);
            }

            [TestMethod]
            public void CheckCommentCount()
            {
                var Expected = 0;
                var Actual = ReadTopic.Markup.Comment.Count;
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void Markup_NoHeaderSectionPresent()
            {
                Assert.IsFalse(ReadTopic.Markup.ShouldSerializeHeader());
            }

            [TestMethod]
            public void CheckViewpointGuid_InMarkup()
            {
                var Expected = "21c7c4a7-8c7a-4b0b-8c93-0026af8bc63c";
                var Actual = ReadTopic.Markup.Viewpoints.First().Guid;
                Assert.AreEqual(Expected, Actual);
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
                var Expected = BCFTestCasesImportData.single_visible_wall.GetBinaryData("74bca551-875c-4455-adaa-2205f2245af3/snapshot.png");
                var Actual = ReadTopic.ViewpointSnapshots["21c7c4a7-8c7a-4b0b-8c93-0026af8bc63c"];
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
            CompareTool.CompareFiles(BCFTestCasesImportData.single_visible_wall, Data);
        }
    }
}