using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.DocumentReference
{
    /// <summary>
    /// HTTP GET representation of the document_reference
    /// </summary>
    [JsonObject(Title = "document_reference")]
    public class document_reference_GET : document_reference_Base
    {
        /// <summary>
        /// Guid
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }
    }
}