using Newtonsoft.Json;
using System.Collections.Generic;

namespace iabi.BCF.APIObjects.Viewpoint.Components
{
    public class lines
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "line")]
        public List<line> line { get; set; }
    }
}