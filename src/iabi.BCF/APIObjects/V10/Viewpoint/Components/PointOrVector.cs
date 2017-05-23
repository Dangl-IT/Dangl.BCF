using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Viewpoint.Components
{
    /// <summary>
    /// This class represents a set of three numbers, representing either a point or a vector
    /// </summary>
    public class PointOrVector
    {
        /// <summary>
        /// x coordinate
        /// </summary>
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "x")]
        public double x { get; set; }

        /// <summary>
        /// y coordinate
        /// </summary>
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "y")]
        public double y { get; set; }

        /// <summary>
        /// z coordinate
        /// </summary>
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "z")]
        public double z { get; set; }
    }
}