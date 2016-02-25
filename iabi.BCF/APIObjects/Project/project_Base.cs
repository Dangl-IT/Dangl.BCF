using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    /// <summary>
    /// Base clase for the project object
    /// </summary>
    public abstract class project_Base
    {
        /// <summary>
        /// Name of the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "name")]
        public virtual string name { get; set; }

        /// <summary>
        /// Guid of the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "project_id")]
        public virtual string project_id { get; set; }
    }
}