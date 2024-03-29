using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10
{
    /// <summary>
    /// Error object for the BCF REST API
    /// </summary>
    [JsonObject(Title = "error")]
    public class error_GET
    {
        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "message")]
        public string message { get; set; }
    }
}