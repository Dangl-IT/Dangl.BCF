using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Topic
{
    /// <summary>
    /// Represents a bim_snippet object
    /// </summary>
    [JsonObject(Title = "bim_snippet")]
    public class bim_snippet
    {
        /// <summary>
        /// Indication of the snippet is external to the topic. Will probably be always "TRUE" in
        /// BCF API scenarios since the snippet can only be a reference and there is not concept of
        /// a container within the BCF API in which a topic and a snippet could both be encapsulated.
        /// As of BCF API v2, Snippet and Topic are dedicated resources that require distinct calls.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public bool is_external { get; set; }

        /// <summary>
        /// Url reference to where the snippet is located at
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string reference { get; set; }

        /// <summary>
        /// This is yet to be clearly defined. It will either act as a namespace URI similar to how XML namespaces
        /// are defined or will give a value indicating the type of file, similar to a mimetype.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string reference_schema { get; set; }

        /// <summary>
        /// The type of the snippet. Might also be a mimetype, see <see cref="reference_schema"/> since this is yet
        /// to be clearly defined.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string snippet_type { get; set; }

        /// <summary>
        /// Indicates if this object has any values set
        /// </summary>
        /// <returns></returns>
        public bool HasValues()
        {
            return !string.IsNullOrWhiteSpace(snippet_type)
                   || !string.IsNullOrWhiteSpace(reference)
                   || !string.IsNullOrWhiteSpace(reference_schema);
        }
    }
}