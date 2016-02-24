using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iabi.BCF.APIObjects.Topic
{
    [JsonObject(Title = "topic")]
    public class topic_PUT : topic_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "title")]
        public override string title { get; set; }

        public static topic_PUT CreateFromJSON(string JSON)
        {
            return (topic_PUT)JsonConvert.DeserializeObject(JSON, typeof(topic_PUT));
        }
    }
}