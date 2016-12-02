using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects
{
    /// <summary>
    /// Error object for the BCF REST API
    /// </summary>
    [JsonObject(Title = "version")]
    public class error_GET
    {
        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "message")]
        public string message { get; set; }
    }
}