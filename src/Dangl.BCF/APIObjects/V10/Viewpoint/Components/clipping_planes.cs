using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Viewpoint.Components
{
    /// <summary>
    /// Rather odd, this object just contains a list of <see cref="Components.clipping_plane"/>. Future versions of the API
    /// will likely pretty format the schema.
    /// </summary>
    public class clipping_planes
    {
        /// <summary>
        /// List of <see cref="Components.clipping_plane"/>
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "clipping_plane")]
        public List<clipping_plane> clipping_plane { get; set; }
    }
}