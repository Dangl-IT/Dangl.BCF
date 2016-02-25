using System.Collections.Generic;
using iabi.BCF.APIObjects.Viewpoint.Components;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Viewpoint
{
    public abstract class viewpoint_Base
    {
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public virtual string guid { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "orthogonal_camera")]
        public orthogonal_camera orthogonal_camera { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "perspective_camera")]
        public perspective_camera perspective_camera { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "lines")]
        public lines lines { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "clipping_planes")]
        public clipping_planes clipping_planes { get; set; }

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "bitmaps")]
        public List<bitmap> bitmaps { get; set; }
    }
}