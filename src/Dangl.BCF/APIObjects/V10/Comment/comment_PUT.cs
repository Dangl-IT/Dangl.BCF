﻿using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Comment
{
    /// <summary>
    /// HTTP PUT representation of the comment
    /// </summary>
    [JsonObject(Title = "comment")]
    public class comment_PUT : comment_Base
    {
        /// <summary>
        /// Required, Status
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "status")]
        public override string status { get; set; }

        /// <summary>
        /// Required, Comment Text
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "comment")]
        public override string comment { get; set; }
    }
}