using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.DocumentReference
{
    /// <summary>
    /// Base class for the document_reference
    /// </summary>
    public abstract class document_reference_Base
    {
        /// <summary>
        /// Guid
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public virtual string guid { get; set; }

        /// <summary>
        /// Url reference to the actual document
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "referenced_document")]
        public virtual string referenced_document { get; set; }

        /// <summary>
        /// Guid of the document
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "document_guid")]
        public virtual string document_guid { get; set; }

        /// <summary>
        /// Description of the document
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "description")]
        public virtual string description { get; set; }
    }
}