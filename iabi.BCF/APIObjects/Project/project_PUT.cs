using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    [JsonObject(Title = "project")]
    public class project_PUT : project_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public override string name { get; set; }

        public static project_PUT CreateFromJSON(string JSON)
        {
            return (project_PUT) JsonConvert.DeserializeObject(JSON, typeof (project_PUT));
        }
    }
}