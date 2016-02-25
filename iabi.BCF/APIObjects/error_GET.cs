using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects
{
    [Serializable]
    [JsonObject(Title = "version")]
    public class error_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "message")]
        public string message { get; set; }
    }
}