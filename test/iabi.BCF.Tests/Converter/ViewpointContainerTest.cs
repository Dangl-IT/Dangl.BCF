using System.Collections.Generic;
using iabi.BCF.APIObjects.Component;
using iabi.BCF.APIObjects.Viewpoint;
using iabi.BCF.Converter;
using Xunit;

namespace iabi.BCF.Tests.Converter
{
    public class ViewpointContainerTest
    {
        [Fact]
        public void AddViewpointWhenSnapshotIsAddedAndNoViewpoint()
        {
            var Instance = new ViewpointContainer();
            Instance.Snapshot = new byte[] {10, 11, 12, 13, 14};
            Assert.NotNull(Instance.Viewpoint);
        }

        [Fact]
        public void AddViewpointWhenComponentsAreAddedAndNoViewpoint()
        {
            var Instance = new ViewpointContainer();
            Instance.Components = new List<component_GET>();
            Assert.NotNull(Instance.Viewpoint);
        }

        [Fact]
        public void GenerateViewpointGuidOnEmptyGuid()
        {
            var Instance = new ViewpointContainer();
            Instance.Viewpoint = new viewpoint_GET();
            Assert.False(string.IsNullOrWhiteSpace(Instance.Viewpoint.guid));
        }
    }
}