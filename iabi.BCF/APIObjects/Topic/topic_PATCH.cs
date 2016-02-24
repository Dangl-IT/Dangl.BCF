using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Topic
{
    [JsonObject(Title = "topic")]
    public class topic_PATCH : topic_Base
    {
        public static topic_PATCH CreateFromJSON(string JSON)
        {
            return (topic_PATCH)JsonConvert.DeserializeObject(JSON, typeof(topic_PATCH));
        }
    }
}