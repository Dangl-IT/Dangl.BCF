using iabi.BCF.BCFv2;
using iabi.BCF.Converter;
using iabi.BCF.Test.BCFTestCases;
using Xunit;
using System;
using System.Linq;

namespace iabi.Test.BCF_REST_API.Converter
{
     
    public class ViewpointContainerTest
    {
        [Fact]
        public void AddViewpointWhenSnapshotIsAddedAndNoViewpoint()
        {
            var Instance = new ViewpointContainer();
            Instance.Snapshot = new byte[] { 10, 11, 12, 13, 14 };
            Assert.NotNull(Instance.Viewpoint);
        }

        [Fact]
        public void AddViewpointWhenComponentsAreAddedAndNoViewpoint()
        {
            var Instance = new ViewpointContainer();
            Instance.Components = new System.Collections.Generic.List<BCF.APIObjects.Component.component_GET>();
            Assert.NotNull(Instance.Viewpoint);
        }

        [Fact]
        public void GenerateViewpointGuidOnEmptyGuid()
        {
            var Instance = new ViewpointContainer();
            Instance.Viewpoint = new BCF.APIObjects.Viewpoint.viewpoint_GET();
            Assert.False(string.IsNullOrWhiteSpace(Instance.Viewpoint.guid));
        }
    }
}
