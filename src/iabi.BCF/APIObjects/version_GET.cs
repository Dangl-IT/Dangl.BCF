using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects
{
    /// <summary>
    /// BCF REST API Version information
    /// </summary>
    [JsonObject(Title = "version")]
    public class version_GET
    {
        /// <summary>
        /// Version Id
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "version_id")]
        public string version_id { get; set; }

        /// <summary>
        /// Version name
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "detailed_version")]
        public string detailed_version { get; set; }
    }
}