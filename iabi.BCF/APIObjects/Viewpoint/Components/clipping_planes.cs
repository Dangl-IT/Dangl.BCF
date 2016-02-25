using System.Collections.Generic;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    public class clipping_planes
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "clipping_plane")]
        public List<clipping_plane> clipping_plane { get; set; }
    }
}