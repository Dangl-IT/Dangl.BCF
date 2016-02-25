using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    [JsonObject(Title = "project")]
    public class project_POST : project_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public override string name { get; set; }

        public static project_POST CreateFromJSON(string JSON)
        {
            return (project_POST) JsonConvert.DeserializeObject(JSON, typeof (project_POST));
        }
    }
}