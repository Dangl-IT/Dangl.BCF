using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    /// <summary>
    /// This class represents a line that should be drawn and made visible in a viewer
    /// </summary>
    public class line
    {
        /// <summary>
        /// Starting point
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "start_point")]
        public PointOrVector start_point { get; set; }

        /// <summary>
        /// End point
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "end_point")]
        public PointOrVector end_point { get; set; }
    }
}