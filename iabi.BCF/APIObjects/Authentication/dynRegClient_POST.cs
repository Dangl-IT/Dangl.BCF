using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Authentication
{
    [JsonObject(Title = "dynRegClient")]
    public class dynRegClient_POST
    {
        [JsonProperty(Required = Required.Always, PropertyName = "client_name")]
        public string client_name { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "client_description")]
        public string client_description { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "client_url")]
        public string client_url { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "redirect_url")]
        public string redirect_url { get; set; }

        public static dynRegClient_POST CreateFromJSON(string JSON)
        {
            return (dynRegClient_POST) JsonConvert.DeserializeObject(JSON, typeof (dynRegClient_POST));
        }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}