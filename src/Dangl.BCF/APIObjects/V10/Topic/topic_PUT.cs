using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Topic
{
    /// <summary>
    /// HTTP PUT representation of the topic
    /// </summary>
    [JsonObject(Title = "topic")]
    public class topic_PUT : topic_Base
    {
        /// <summary>
        /// Title of the topic
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "title")]
        public override string title { get; set; }
    }
}