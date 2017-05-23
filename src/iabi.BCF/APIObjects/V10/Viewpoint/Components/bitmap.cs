using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace iabi.BCF.APIObjects.V10.Viewpoint.Components
{
    /// <summary>
    /// Representation of a bitmap object, an embedded picture to be drawn in a viewer.
    /// </summary>
    public class bitmap
    {
        /// <summary>
        /// Type of the bitmap (e.g. PNG or JPG)
        /// </summary>
        [JsonProperty(Required = Required.Default, PropertyName = "bitmap_type")]
        [JsonConverter(typeof (StringEnumConverter))]
        public bitmap_type bitmap_type { get; set; }

        /// <summary>
        /// Guid of the bitmap
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public string guid { get; set; }

        /// <summary>
        /// Location coordinate of the bitmap
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "location")]
        public PointOrVector location { get; set; }

        /// <summary>
        /// Normal vector (e.g. 90° on bitmap plane)
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "normal")]
        public PointOrVector normal { get; set; }

        /// <summary>
        /// Up vector of the bitmap (which side is up basically)
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "up")]
        public PointOrVector up { get; set; }

        /// <summary>
        /// Height of the bitmap, expetected to be given in the project units of the Ifc file in which context
        /// the bitmap is shown. E.g. if the Ifc file has "metres" as unit and the height is given as 2.5, the
        /// bitmap will be visualized with a height of 2.5 meteres. Other dimensions are expected to be scaled while
        /// keeping the aspect ratio.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "height")]
        public double height { get; set; }
    }
}