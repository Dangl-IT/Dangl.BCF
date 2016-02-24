using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    public class perspective_camera
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_view_point")]
        public PointOrVector camera_view_point { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_direction")]
        public PointOrVector camera_direction { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_up_vector")]
        public PointOrVector camera_up_vector { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "field_of_view")]
        public double field_of_view { get; set; }
    }
}