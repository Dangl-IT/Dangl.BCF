using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.RelatedTopic
{
    /// <summary>
    /// Base class for the related topic
    /// </summary>
    [JsonObject(Title = "related_topic")]
    public abstract class related_topic_Base
    {
        /// <summary>
        /// Guid of the related topic
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "related_topic_guid")]
        public virtual string related_topic_guid { get; set; }
    }
}