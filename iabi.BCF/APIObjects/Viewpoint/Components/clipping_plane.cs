using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    public class clipping_plane
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "location")]
        public PointOrVector location { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "direction")]
        public PointOrVector direction { get; set; }
    }
}