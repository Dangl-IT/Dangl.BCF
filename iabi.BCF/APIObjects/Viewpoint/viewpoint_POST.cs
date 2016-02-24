using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint
{
    [JsonObject(Title = "viewpoint")]
    public class viewpoint_POST : viewpoint_Base
    {
        public static viewpoint_POST CreateFromJSON(string JSON)
        {
            return (viewpoint_POST)JsonConvert.DeserializeObject(JSON, typeof(viewpoint_POST));
        }
    }
}