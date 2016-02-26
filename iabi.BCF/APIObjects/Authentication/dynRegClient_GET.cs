using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Authentication
{
    /// <summary>
    /// Response object of the dynamic client registration
    /// </summary>
    [JsonObject(Title = "dynRegClient")]
    public class dynRegClient_GET
    {
        /// <summary>
        /// Assigned client identifier
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "client_id")]
        public string client_id { get; set; }

        /// <summary>
        /// Assigned client secret
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "client_secret")]
        public string client_secret { get; set; }
    }
}