using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects
{
    [Serializable]
    [JsonObject(Title = "version")]
    public class version_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "version_id")]
        public string version_id { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "detailed_version")]
        public string detailed_version { get; set; }
    }
}