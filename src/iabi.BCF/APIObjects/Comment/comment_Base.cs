using System;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Comment
{
    /// <summary>
    /// Base class for the comment
    /// </summary>
    public abstract class comment_Base
    {
        /// <summary>
        /// Guid
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public virtual string guid { get; set; }

        /// <summary>
        /// Verbal status; expect this be be removed with the next BCF API version
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "verbal_status")]
        public virtual string verbal_status { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "status")]
        public virtual string status { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "date")]
        public virtual DateTime date { get; set; }

        /// <summary>
        /// Author email
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "author")]
        public virtual string author { get; set; }

        /// <summary>
        /// Comment text
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "comment")]
        public virtual string comment { get; set; }

        /// <summary>
        /// Guid of parent topic
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_guid")]
        public virtual string topic_guid { get; set; }

        /// <summary>
        /// Guid of a referenced viewpoint
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "viewpoint_guid")]
        public virtual string viewpoint_guid { get; set; }

        /// <summary>
        /// Guid of a comment this is a reply to
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "reply_to_comment_guid")]
        public virtual string reply_to_comment_guid { get; set; }

        /// <summary>
        /// Last modification date
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "modified_date")]
        public virtual DateTime? modified_date { get; set; }

        /// <summary>
        /// Last modifications author email
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "modified_author")]
        public virtual string modified_author { get; set; }
    }
}