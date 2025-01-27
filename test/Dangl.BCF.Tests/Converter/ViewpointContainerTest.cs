﻿using System.Collections.Generic;
using Dangl.BCF.APIObjects.V10.Component;
using Dangl.BCF.APIObjects.V10.Viewpoint;
using Dangl.BCF.Converter;
using Xunit;

namespace Dangl.BCF.Tests.Converter
{
    public class ViewpointContainerTest
    {
        [Fact]
        public void AddViewpointWhenSnapshotIsAddedAndNoViewpoint()
        {
            var viewpointContainer = new ViewpointContainer();
            viewpointContainer.Snapshot = new byte[] {10, 11, 12, 13, 14};
            Assert.NotNull(viewpointContainer.Viewpoint);
        }

        [Fact]
        public void AddViewpointWhenComponentsAreAddedAndNoViewpoint()
        {
            var viewpointContainer = new ViewpointContainer();
            viewpointContainer.Components = new List<component_GET>();
            Assert.NotNull(viewpointContainer.Viewpoint);
        }

        [Fact]
        public void GenerateViewpointGuidOnEmptyGuid()
        {
            var viewpointContainer = new ViewpointContainer();
            viewpointContainer.Viewpoint = new viewpoint_GET();
            Assert.False(string.IsNullOrWhiteSpace(viewpointContainer.Viewpoint.guid));
        }
    }
}