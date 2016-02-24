using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Comment
{
    [JsonObject(Title = "comment")]
    public class comment_PATCH : comment_Base
    {
        public static comment_PATCH CreateFromJSON(string JSON)
        {
            return (comment_PATCH)JsonConvert.DeserializeObject(JSON, typeof(comment_PATCH));
        }
    }
}