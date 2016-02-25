using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Authentication
{
    [JsonObject(Title = "dynRegClient")]
    public class dynRegClient_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "client_id")]
        public string client_id { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "client_secret")]
        public string client_secret { get; set; }
    }
}