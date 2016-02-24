using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iabi.BCF.APIObjects.Authentication
{
    [JsonObject(Title = "token")]
    public class token_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "access_token")]
        public string access_token { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "token_type")]
        public string token_type { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "expires_in")]
        public int expires_in { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "refresh_token")]
        public string refresh_token { get; set; }

        public static token_GET CreateFromJSON(string JSON)
        {
            return (token_GET)JsonConvert.DeserializeObject(JSON, typeof(token_GET));
        }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}