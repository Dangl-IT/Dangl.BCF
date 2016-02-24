using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using iabi.BCF.Test;
using iabi.BCF.Test.BCFTestCases;
using iabi.BCF.Test.BCFTestCases.CreateAndExport;
using iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory;
using iabi.BCF.Test.BCFv2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace iabi.Test.BCF_REST_API.BCFv2
{
    [TestClass]
    public class BCFTopicTest
    {
        /// <summary>
        /// Not expecting an exception and the markup should then be automatically created (with the viewpoint data)
        /// </summary>
        [TestMethod]
        public void AddViewpointWhenThereIsNoMarkupInstance()
        {
            BCFTopic Instance = new BCFTopic();

            // Markup is empty
            Assert.IsNull(Instance.Markup, "Markup is present but not yet expected");

            Instance.Viewpoints.Add(new VisualizationInfo());

            // Viewpoint defined
            Assert.IsNotNull(Instance.Markup, "Markup was not automatically created");
            Assert.AreEqual(Instance.Markup.Viewpoints.First().Guid, Instance.Viewpoints.First().GUID);
        }

        [TestMethod]
        public void AddviewpointSnapshotReferenceInMarkup()
        {
            BCFTopic Instance = new BCFTopic();
            Instance.Viewpoints.Add(new VisualizationInfo());

            Instance.AddOrUpdateSnapshot(Instance.Viewpoints.First().GUID, new byte[] { 10, 11, 12, 13, 14, 15 });

            //System.Threading.Thread.Sleep(100);

            var CreatedSnapshotReference = Instance.Markup.Viewpoints.FirstOrDefault().Snapshot;

            //Assert.IsFalse(string.IsNullOrWhiteSpace(CreatedSnapshotReference), "Reference not created for viewpoint snapshot");
            Assert.IsFalse(string.IsNullOrWhiteSpace(Instance.Markup.Viewpoints.FirstOrDefault().Snapshot), "Reference not created for viewpoint snapshot");
        }
    }
}