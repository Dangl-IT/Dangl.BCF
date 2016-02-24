using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint
{
    [JsonObject(Title = "viewpoint")]
    public class viewpoint_PATCH : viewpoint_Base
    {
        public static viewpoint_PATCH CreateFromJSON(string JSON)
        {
            return (viewpoint_PATCH)JsonConvert.DeserializeObject(JSON, typeof(viewpoint_PATCH));
        }
    }
}