using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Comment
{
    /// <summary>
    /// HTTP GET representation of the comment
    /// </summary>
    [JsonObject(Title = "comment")]
    public class comment_GET : comment_Base
    {
        /// <summary>
        /// Required, Guid
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }

        /// <summary>
        /// Required, Status
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "status")]
        public override string status { get; set; }

        /// <summary>
        /// Required, Creation date
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "date")]
        public override DateTime date { get; set; }

        /// <summary>
        /// Required, Author email
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "author")]
        public override string author { get; set; }

        /// <summary>
        /// Required, Comment Text
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "comment")]
        public override string comment { get; set; }

        /// <summary>
        /// Required, Guid of parent topic
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "topic_guid")]
        public override string topic_guid { get; set; }
    }
}