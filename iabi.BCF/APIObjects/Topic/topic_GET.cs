using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Topic
{
    /// <summary>
    /// HTTP GET representation of the topic
    /// </summary>
    [JsonObject(Title = "topic")]
    public class topic_GET : topic_Base
    {
        /// <summary>
        /// Guid of the topic
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }

        /// <summary>
        /// Title of the topic
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "title")]
        public override string title { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "creation_date")]
        public override DateTime creation_date { get; set; }

        /// <summary>
        /// Email of creation author
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "creation_author")]
        public override string creation_author { get; set; }
    }
}