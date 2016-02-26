using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Authentication
{
    /// <summary>
    /// HTTP POST representation of the dynamic client registration
    /// </summary>
    [JsonObject(Title = "dynRegClient")]
    public class dynRegClient_POST
    {
        /// <summary>
        /// Client name
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "client_name")]
        public string client_name { get; set; }

        /// <summary>
        /// Description of the client
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "client_description")]
        public string client_description { get; set; }

        /// <summary>
        /// Url of the client (pointing to a website where informations about the client can be found)
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "client_url")]
        public string client_url { get; set; }

        /// <summary>
        /// Where OAuth2 token requests should be redirected to
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "redirect_url")]
        public string redirect_url { get; set; }
    }
}