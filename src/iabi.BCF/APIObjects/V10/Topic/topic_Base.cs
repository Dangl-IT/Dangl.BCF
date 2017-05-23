using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Topic
{
    /// <summary>
    /// Base class for a topic object
    /// </summary>
    public abstract class topic_Base
    {
        /// <summary>
        /// Guid of the topic
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public virtual string guid { get; set; }

        /// <summary>
        /// Type of the topic, possible types are found in the <see cref="Extensions.extensions_Base"/>
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_type")]
        public virtual string topic_type { get; set; }

        /// <summary>
        /// Status of the topic, possible stati are found in the <see cref="Extensions.extensions_Base"/>
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_status")]
        public virtual string topic_status { get; set; }

        /// <summary>
        /// Link containing further information. Is expected to be displayed to the user, so it could be, for example, a link
        /// to a technical documentation.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "reference_link")]
        public virtual string reference_link { get; set; }

        /// <summary>
        /// Title of the topic
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "title")]
        public virtual string title { get; set; }

        /// <summary>
        /// Priority of the topic, possible priorities are found in the <see cref="Extensions.extensions_Base"/>
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "priority")]
        public virtual string priority { get; set; }

        /// <summary>
        /// Index of the topic. Intended for ordering topics; originates from the physical BCFzip format. Will probably not
        /// getting really used due to the BCF APIs sorting and filtering capabilities.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "index")]
        public virtual int index { get; set; }

        /// <summary>
        /// List of labels of the topic, possible labels are found in the <see cref="Extensions.extensions_Base"/>
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "labels")]
        public virtual List<string> labels { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "creation_date")]
        public virtual DateTime creation_date { get; set; }

        /// <summary>
        /// Email of the creation author
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "creation_author")]
        public virtual string creation_author { get; set; }

        /// <summary>
        /// Date of last modification
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "modified_date")]
        public virtual DateTime? modified_date { get; set; }

        /// <summary>
        /// Author email of the last modification
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "modified_author")]
        public virtual string modified_author { get; set; }

        /// <summary>
        /// Optional email reference indicating a certain person as being responsible for this topic. Used for example when issues
        /// will be marked to say "to be solved or delegated by person x"
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "assigned_to")]
        public virtual string assigned_to { get; set; }

        /// <summary>
        /// Text description of the topic
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "description")]
        public virtual string description { get; set; }

        /// <summary>
        /// Optional metadata description of <see cref="bim_snippet"/> attached to this topic.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "bim_snippet")]
        public virtual bim_snippet bim_snippet { get; set; }
    }
}