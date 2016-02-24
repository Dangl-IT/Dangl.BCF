using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace iabi.BCF.APIObjects
{
    [Serializable()]
    [JsonObject(Title = "version")]
    public class version_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "version_id")]
        public string version_id { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "detailed_version")]
        public string detailed_version { get; set; }

        public static version_GET CreateFromJSON(string JSON)
        {
            return (version_GET)JsonConvert.DeserializeObject(JSON, typeof(version_GET));
        }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}