using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.RelatedTopic
{
    /// <summary>
    /// HTTP PATCH representation of the related topic
    /// </summary>
    [JsonObject(Title = "related_topic")]
    public class related_topic_PATCH : related_topic_Base
    {
        /// <summary>
        /// Guid of the topic to create a reference to
        /// </summary>
        [JsonProperty(Required = Required.Default, PropertyName = "related_topic_guid")]
        public override string related_topic_guid { get; set; }
    }
}