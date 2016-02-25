using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Comment
{
    [JsonObject(Title = "comment")]
    public class comment_POST : comment_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "status")]
        public override string status { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "comment")]
        public override string comment { get; set; }

        public static comment_POST CreateFromJSON(string JSON)
        {
            return (comment_POST) JsonConvert.DeserializeObject(JSON, typeof (comment_POST));
        }
    }
}