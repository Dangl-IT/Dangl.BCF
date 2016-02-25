using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.RelatedTopic
{
    [JsonObject(Title = "related_topic")]
    public class related_topic_GET : related_topic_Base
    {
        public static related_topic_GET CreateFromJSON(string JSON)
        {
            return (related_topic_GET) JsonConvert.DeserializeObject(JSON, typeof (related_topic_GET));
        }
    }
}