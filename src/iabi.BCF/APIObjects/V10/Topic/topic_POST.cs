using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Topic
{
    /// <summary>
    /// HTTP POST representation of the topic
    /// </summary>
    [JsonObject(Title = "topic")]
    public class topic_POST : topic_Base
    {
        /// <summary>
        /// Title of the topic
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public override string title { get; set; }
    }
}