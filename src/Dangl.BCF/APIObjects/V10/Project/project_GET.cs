using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Project
{
    /// <summary>
    /// HTTP GET representation of the project
    /// </summary>
    [JsonObject(Title = "project")]
    public class project_GET : project_Base
    {
        /// <summary>
        /// Guid of the project
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "project_id")]
        public override string project_id { get; set; }

        /// <summary>
        /// Name of the project
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public override string name { get; set; }
    }
}