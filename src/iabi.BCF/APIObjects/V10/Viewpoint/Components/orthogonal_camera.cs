using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Viewpoint.Components
{
    /// <summary>
    /// This class represents an orthoganl_camera
    /// </summary>
    public class orthogonal_camera
    {
        /// <summary>
        /// Viewpoint (Basepoint) of the camera
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_view_point")]
        public PointOrVector camera_view_point { get; set; }

        /// <summary>
        /// Camera direction vector
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_direction")]
        public PointOrVector camera_direction { get; set; }

        /// <summary>
        /// Camera up vector
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_up_vector")]
        public PointOrVector camera_up_vector { get; set; }

        /// <summary>
        /// Camera "View to World" scale
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "view_to_world_scale")]
        public double view_to_world_scale { get; set; }
    }
}