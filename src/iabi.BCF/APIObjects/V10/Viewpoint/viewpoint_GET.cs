using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Viewpoint
{
    /// <summary>
    /// HTTP GET representation of the viewpoint
    /// </summary>
    [JsonObject(Title = "viewpoint")]
    public class viewpoint_GET : viewpoint_Base
    {
        /// <summary>
        /// Guid of the viewpoint
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }

        /// <summary>
        /// Indicates true if the viewpoint has an attached snapshot
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public bool snapshot_available { get; set; }
    }
}