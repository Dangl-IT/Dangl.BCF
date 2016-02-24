using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    public class PointOrVector
    {
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "x")]
        public double x { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "y")]
        public double y { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "z")]
        public double z { get; set; }
    }
}