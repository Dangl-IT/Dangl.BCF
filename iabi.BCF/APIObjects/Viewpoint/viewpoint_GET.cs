using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint
{
    [JsonObject(Title = "viewpoint")]
    public class viewpoint_GET : viewpoint_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool snapshot_available { get; set; }

        public static viewpoint_GET CreateFromJSON(string JSON)
        {
            return (viewpoint_GET) JsonConvert.DeserializeObject(JSON, typeof (viewpoint_GET));
        }
    }
}