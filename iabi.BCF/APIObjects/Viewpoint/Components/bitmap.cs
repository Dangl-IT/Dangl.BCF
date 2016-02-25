using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    public class bitmap
    {
        [JsonProperty(Required = Required.Default, PropertyName = "bitmap_type")]
        [JsonConverter(typeof (StringEnumConverter))]
        public bitmap_type bitmap_type { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public string guid { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "location")]
        public PointOrVector location { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "normal")]
        public PointOrVector normal { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "up")]
        public PointOrVector up { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "height")]
        public double height { get; set; }
    }
}