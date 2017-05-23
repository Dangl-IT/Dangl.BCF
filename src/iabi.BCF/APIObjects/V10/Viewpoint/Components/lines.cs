using System.Collections.Generic;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Viewpoint.Components
{
    /// <summary>
    /// Rather odd, this class just presents a list of <see cref="Components.line"/>. Will probably be flattened in the next
    /// version of the BCF API.
    /// </summary>
    public class lines
    {
        /// <summary>
        /// List of <see cref="Components.line"/>
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "line")]
        public List<line> line { get; set; }
    }
}