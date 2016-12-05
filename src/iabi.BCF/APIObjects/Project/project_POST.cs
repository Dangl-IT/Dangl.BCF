using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    /// <summary>
    /// HTTP POST representation of the project
    /// </summary>
    [JsonObject(Title = "project")]
    public class project_POST : project_Base
    {
        /// <summary>
        /// Name of the project to be created
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public override string name { get; set; }
    }
}