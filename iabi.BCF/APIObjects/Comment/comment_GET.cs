using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace iabi.BCF.APIObjects.Comment
{
    [JsonObject(Title = "comment")]
    public class comment_GET : comment_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "status")]
        public override string status { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "date")]
        public override DateTime date { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "author")]
        public override string author { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "comment")]
        public override string comment { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "topic_guid")]
        public override string topic_guid { get; set; }

        public static comment_GET CreateFromJSON(string JSON)
        {
            return (comment_GET)JsonConvert.DeserializeObject(JSON, typeof(comment_GET));
        }
    }
}