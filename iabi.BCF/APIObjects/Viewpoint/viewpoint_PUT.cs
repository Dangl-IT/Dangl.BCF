using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint
{
    [JsonObject(Title = "viewpoint")]
    public class viewpoint_PUT : viewpoint_Base
    {
        public static viewpoint_PUT CreateFromJSON(string JSON)
        {
            return (viewpoint_PUT)JsonConvert.DeserializeObject(JSON, typeof(viewpoint_PUT));
        }
    }
}