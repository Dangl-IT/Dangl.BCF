using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    [JsonObject(Title = "project")]
    public class project_POST : project_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public override string name { get; set; }
    }
}