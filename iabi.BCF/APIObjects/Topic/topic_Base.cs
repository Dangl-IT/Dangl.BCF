using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Topic
{
    public abstract class topic_Base
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public virtual string guid { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_type")]
        public virtual string topic_type { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_status")]
        public virtual string topic_status { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "reference_link")]
        public virtual string reference_link { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "title")]
        public virtual string title { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "priority")]
        public virtual string priority { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "index")]
        public virtual int index { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "labels")]
        public virtual List<string> labels { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "creation_date")]
        public virtual DateTime creation_date { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "creation_author")]
        public virtual string creation_author { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "modified_date")]
        public virtual DateTime? modified_date { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "modified_author")]
        public virtual string modified_author { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "assigned_to")]
        public virtual string assigned_to { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "description")]
        public virtual string description { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "bim_snippet")]
        public virtual bim_snippet bim_snippet { get; set; }
    }

    [JsonObject(Title = "bim_snippet")]
    public class bim_snippet
    {
        [JsonProperty(Required = Required.Always)] public bool is_external;

        [JsonProperty(Required = Required.Always)] public string reference;

        [JsonProperty(Required = Required.Always)] public string reference_schema;

        [JsonProperty(Required = Required.Always)] public string snippet_type;

        public bool HasValues()
        {
            return !string.IsNullOrWhiteSpace(snippet_type)
                   || !string.IsNullOrWhiteSpace(reference)
                   || !string.IsNullOrWhiteSpace(reference_schema);
        }
    }
}