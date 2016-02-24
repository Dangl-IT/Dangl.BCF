using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iabi.BCF.APIObjects.Authentication
{
    [JsonObject(Title = "dynRegClient")]
    public class dynRegClient_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "client_id")]
        public string client_id { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "client_secret")]
        public string client_secret { get; set; }

        public static dynRegClient_GET CreateFromJSON(string JSON)
        {
            return (dynRegClient_GET)JsonConvert.DeserializeObject(JSON, typeof(dynRegClient_GET));
        }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}