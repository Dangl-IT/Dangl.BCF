using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Viewpoint.Components
{
    /// <summary>
    /// This class represents a perspective camera
    /// </summary>
    public class perspective_camera
    {
        /// <summary>
        /// Viewpoint (base point) of the camera
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_view_point")]
        public PointOrVector camera_view_point { get; set; }

        /// <summary>
        /// Direction vector of the camera
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_direction")]
        public PointOrVector camera_direction { get; set; }

        /// <summary>
        /// Up vector of the camera
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "camera_up_vector")]
        public PointOrVector camera_up_vector { get; set; }

        /// <summary>
        /// Field of View of the camera
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "field_of_view")]
        public double field_of_view { get; set; }
    }
}