using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace iabi.BCF.APIObjects.File
{
    [JsonObject(Title = "file")]
    public abstract class file_Base
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "ifc_project")]
        public virtual string ifc_project { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "ifc_spatial_structure_element")]
        public virtual string ifc_spatial_structure_element { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "file_name")]
        public virtual string file_name { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "date")]
        public virtual DateTime date { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "reference")]
        public virtual string reference { get; set; }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}