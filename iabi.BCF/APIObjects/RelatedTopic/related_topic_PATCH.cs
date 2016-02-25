using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.RelatedTopic
{
    [JsonObject(Title = "related_topic")]
    public class related_topic_PATCH : related_topic_Base
    {
        [JsonProperty(Required = Required.Default, PropertyName = "related_topic_guid")]
        public override string related_topic_guid { get; set; }
    }
}