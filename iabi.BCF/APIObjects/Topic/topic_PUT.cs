using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Topic
{
    [JsonObject(Title = "topic")]
    public class topic_PUT : topic_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "title")]
        public override string title { get; set; }
    }
}