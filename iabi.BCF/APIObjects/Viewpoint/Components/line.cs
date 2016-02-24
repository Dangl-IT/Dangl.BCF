using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    public class line
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "start_point")]
        public PointOrVector start_point { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "end_point")]
        public PointOrVector end_point { get; set; }
    }
}