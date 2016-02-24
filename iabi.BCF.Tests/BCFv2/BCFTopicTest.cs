using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using iabi.BCF.Test;
using iabi.BCF.Test.BCFTestCases;
using iabi.BCF.Test.BCFTestCases.CreateAndExport;
using iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory;
using iabi.BCF.Test.BCFv2;
using Xunit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace iabi.Test.BCF_REST_API.BCFv2
{
     
    public class BCFTopicTest
    {
        /// <summary>
        /// Not expecting an exception and the markup should then be automatically created (with the viewpoint data)
        /// </summary>
        [Fact]
        public void AddViewpointWhenThereIsNoMarkupInstance()
        {
            BCFTopic Instance = new BCFTopic();

            // Markup is empty
            Assert.Null(Instance.Markup);

            Instance.Viewpoints.Add(new VisualizationInfo());

            // Viewpoint defined
            Assert.NotNull(Instance.Markup);
            Assert.Equal(Instance.Markup.Viewpoints.First().Guid, Instance.Viewpoints.First().GUID);
        }

        [Fact]
        public void AddviewpointSnapshotReferenceInMarkup()
        {
            BCFTopic Instance = new BCFTopic();
            Instance.Viewpoints.Add(new VisualizationInfo());

            Instance.AddOrUpdateSnapshot(Instance.Viewpoints.First().GUID, new byte[] { 10, 11, 12, 13, 14, 15 });

            //System.Threading.Thread.Sleep(100);

            var CreatedSnapshotReference = Instance.Markup.Viewpoints.FirstOrDefault().Snapshot;

            //Assert.False(string.IsNullOrWhiteSpace(CreatedSnapshotReference), "Reference not created for viewpoint snapshot");
            Assert.False(string.IsNullOrWhiteSpace(Instance.Markup.Viewpoints.FirstOrDefault().Snapshot), "Reference not created for viewpoint snapshot");
        }
    }
}
