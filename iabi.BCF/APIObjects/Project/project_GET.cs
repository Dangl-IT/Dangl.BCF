using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    [JsonObject(Title = "project")]
    public class project_GET : project_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "project_id")]
        public override string project_id { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public override string name { get; set; }

        public static project_GET CreateFromJSON(string JSON)
        {
            return (project_GET) JsonConvert.DeserializeObject(JSON, typeof (project_GET));
        }
    }
}