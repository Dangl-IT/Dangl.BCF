using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iabi.BCF.APIObjects.Comment
{
    [JsonObject(Title = "comment")]
    public class comment_PUT : comment_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "status")]
        public override string status { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "comment")]
        public override string comment { get; set; }

        public static comment_PUT CreateFromJSON(string JSON)
        {
            return (comment_PUT)JsonConvert.DeserializeObject(JSON, typeof(comment_PUT));
        }
    }
}