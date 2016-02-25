using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Topic
{
    [JsonObject(Title = "topic")]
    public class topic_POST : topic_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public override string title { get; set; }

        public static topic_POST CreateFromJSON(string JSON)
        {
            return (topic_POST) JsonConvert.DeserializeObject(JSON, typeof (topic_POST));
        }
    }
}