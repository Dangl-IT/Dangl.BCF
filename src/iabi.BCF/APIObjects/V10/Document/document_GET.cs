using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Document
{
    /// <summary>
    /// HTTP GET representation for the document
    /// </summary>
    [JsonObject(Title = "document")]
    public class document_GET
    {
        /// <summary>
        /// Guid
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public string guid { get; set; }

        /// <summary>
        /// Filename
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "filename")]
        public string filename { get; set; }
    }
}