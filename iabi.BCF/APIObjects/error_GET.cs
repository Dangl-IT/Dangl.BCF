using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace iabi.BCF.APIObjects
{
    [Serializable()]
    [JsonObject(Title = "version")]
    public class error_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "message")]
        public string message { get; set; }

        public static error_GET CreateFromJSON(string JSON)
        {
            return (error_GET)JsonConvert.DeserializeObject(JSON, typeof(error_GET));
        }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}