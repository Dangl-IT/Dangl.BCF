using iabi.BCF.BCFv2;
using iabi.BCF.Converter;
using iabi.BCF.Test.BCFTestCases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace iabi.Test.BCF_REST_API.Converter
{
    [TestClass]
    public class ViewpointContainerTest
    {
        [TestMethod]
        public void AddViewpointWhenSnapshotIsAddedAndNoViewpoint()
        {
            var Instance = new ViewpointContainer();
            Instance.Snapshot = new byte[] { 10, 11, 12, 13, 14 };
            Assert.IsNotNull(Instance.Viewpoint);
        }

        [TestMethod]
        public void AddViewpointWhenComponentsAreAddedAndNoViewpoint()
        {
            var Instance = new ViewpointContainer();
            Instance.Components = new System.Collections.Generic.List<BCF.APIObjects.Component.component_GET>();
            Assert.IsNotNull(Instance.Viewpoint);
        }

        [TestMethod]
        public void GenerateViewpointGuidOnEmptyGuid()
        {
            var Instance = new ViewpointContainer();
            Instance.Viewpoint = new BCF.APIObjects.Viewpoint.viewpoint_GET();
            Assert.IsFalse(string.IsNullOrWhiteSpace(Instance.Viewpoint.guid));
        }
    }
}