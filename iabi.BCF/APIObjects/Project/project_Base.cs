using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    public abstract class project_Base
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "name")]
        public virtual string name { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "project_id")]
        public virtual string project_id { get; set; }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}