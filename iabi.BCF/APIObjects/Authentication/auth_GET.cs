using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Authentication
{
    [Serializable]
    [JsonObject(Title = "auth")]
    public class auth_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "oauth2_auth_url")]
        public string oauth2_auth_url { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "oauth2_token_url")]
        public string oauth2_token_url { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "oauth2_dynamic_client_reg_url")]
        public string oauth2_dynamic_client_reg_url { get; set; }

        public static auth_GET CreateFromJSON(string JSON)
        {
            return (auth_GET) JsonConvert.DeserializeObject(JSON, typeof (auth_GET));
        }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}