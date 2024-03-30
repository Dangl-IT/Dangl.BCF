using System.Linq;
using Dangl.BCF.BCFv3;
using Dangl.BCF.BCFv3.Schemas;
using Xunit;

namespace Dangl.BCF.Tests.BCFv3
{
    public class BCFTopicTest
    {
        /// <summary>
        ///     Not expecting an exception and the markup should then be automatically created (with the viewpoint data)
        /// </summary>
        [Fact]
        public void AddViewpointWhenThereIsNoMarkupInstance()
        {
            var bcfTopic = new BCFTopic();
            // Markup is empty
            Assert.Null(bcfTopic.Markup);
            bcfTopic.Viewpoints.Add(new VisualizationInfo());
            // Viewpoint defined
            Assert.NotNull(bcfTopic.Markup);
            Assert.Equal(bcfTopic.Markup.Topic.Viewpoints.First().Guid, bcfTopic.Viewpoints.First().Guid);
        }

        [Fact]
        public void AddViewpointSnapshotReferenceInMarkup()
        {
            var bcfTopic = new BCFTopic();
            bcfTopic.Viewpoints.Add(new VisualizationInfo());
            bcfTopic.AddOrUpdateSnapshot(bcfTopic.Viewpoints.First().Guid, new byte[] {10, 11, 12, 13, 14, 15});
            Assert.False(string.IsNullOrWhiteSpace(bcfTopic.Markup.Topic.Viewpoints.FirstOrDefault().Snapshot), "Reference not created for viewpoint snapshot");
        }
    }
}