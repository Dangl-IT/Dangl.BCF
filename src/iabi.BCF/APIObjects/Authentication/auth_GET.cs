using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Authentication
{
    /// <summary>
    /// Contains server authentication endpoints
    /// </summary>
    [JsonObject(Title = "auth")]
    public class auth_GET
    {
        /// <summary>
        /// Url where user OAuth2 user authorization should be redirected to
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "oauth2_auth_url")]
        public string oauth2_auth_url { get; set; }

        /// <summary>
        /// Url where OAuth2 tokens are exchanged
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "oauth2_token_url")]
        public string oauth2_token_url { get; set; }

        /// <summary>
        /// Optional, Url where OAuth2 clients register dynamically
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "oauth2_dynamic_client_reg_url")]
        public string oauth2_dynamic_client_reg_url { get; set; }
    }
}