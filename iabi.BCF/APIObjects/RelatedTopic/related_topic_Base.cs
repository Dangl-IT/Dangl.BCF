using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.RelatedTopic
{
    [JsonObject(Title = "related_topic")]
    public abstract class related_topic_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "related_topic_guid")]
        public virtual string related_topic_guid { get; set; }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}