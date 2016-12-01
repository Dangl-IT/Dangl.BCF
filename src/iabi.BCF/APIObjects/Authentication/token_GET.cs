using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Authentication
{
    /// <summary>
    /// Representation of the token object
    /// </summary>
    [JsonObject(Title = "token")]
    public class token_GET
    {
        /// <summary>
        /// The actual token to be used for the Beaerer authentication
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "access_token")]
        public string access_token { get; set; }

        /// <summary>
        /// Type of the token, should always be Bearer as of the current API
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "token_type")]
        public string token_type { get; set; }

        /// <summary>
        /// Seconds until the token will be expired
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "expires_in")]
        public int expires_in { get; set; }

        /// <summary>
        /// One time use token to request a new access token
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "refresh_token")]
        public string refresh_token { get; set; }
    }
}