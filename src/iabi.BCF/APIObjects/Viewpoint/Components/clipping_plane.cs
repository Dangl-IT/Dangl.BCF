using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    /// <summary>
    /// Class to represent the geometry of a clipping plane. A clipping plane is just a gemetrical plane which
    /// indicates that the model should be cut along its surface and be hidden on the side of the direction.
    /// </summary>
    public class clipping_plane
    {
        /// <summary>
        /// Coordinate of a point on the plane
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "location")]
        public PointOrVector location { get; set; }

        /// <summary>
        /// Normal vector of the plane (90° angle from the plane surface). Parts on this side of the plane should be invisible
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "direction")]
        public PointOrVector direction { get; set; }
    }
}