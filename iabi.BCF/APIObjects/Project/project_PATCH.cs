using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    [JsonObject(Title = "project")]
    public class project_PATCH : project_Base
    {
        public static project_PATCH CreateFromJSON(string JSON)
        {
            return (project_PATCH)JsonConvert.DeserializeObject(JSON, typeof(project_PATCH));
        }
    }
}