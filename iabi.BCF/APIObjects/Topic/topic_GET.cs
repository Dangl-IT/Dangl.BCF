using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Topic
{
    [JsonObject(Title = "topic")]
    public class topic_GET : topic_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "title")]
        public override string title { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "creation_date")]
        public override DateTime creation_date { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "creation_author")]
        public override string creation_author { get; set; }

        public static topic_GET CreateFromJSON(string JSON)
        {
            return (topic_GET) JsonConvert.DeserializeObject(JSON, typeof (topic_GET));
        }
    }
}