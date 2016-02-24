using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iabi.BCF.APIObjects.RelatedTopic
{
    [JsonObject(Title = "related_topic")]
    public class related_topic_PATCH : related_topic_Base
    {
        [JsonProperty(Required = Required.Default, PropertyName = "related_topic_guid")]
        public override string related_topic_guid { get; set; }

        public static related_topic_PATCH CreateFromJSON(string JSON)
        {
            return (related_topic_PATCH)JsonConvert.DeserializeObject(JSON, typeof(related_topic_PATCH));
        }
    }
}