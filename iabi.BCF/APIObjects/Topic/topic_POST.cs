using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Topic
{
    [JsonObject(Title = "topic")]
    public class topic_POST : topic_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public override string title { get; set; }
    }
}