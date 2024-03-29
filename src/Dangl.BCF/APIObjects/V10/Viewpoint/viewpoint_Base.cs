using System.Collections.Generic;
using Dangl.BCF.APIObjects.V10.Viewpoint.Components;
using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Viewpoint
{
    /// <summary>
    /// Base class for the viewpoint
    /// </summary>
    public abstract class viewpoint_Base
    {
        /// <summary>
        /// Guid
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "guid")]
        public virtual string guid { get; set; }

        /// <summary>
        /// Orthogonal camera (only if no perspective camera present)
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "orthogonal_camera")]
        public orthogonal_camera orthogonal_camera { get; set; }

        /// <summary>
        /// perspective camera (only if no orthogonal camera present)
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "perspective_camera")]
        public perspective_camera perspective_camera { get; set; }

        /// <summary>
        /// Object containing the lines to be drawn in a viewer
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "lines")]
        public lines lines { get; set; }

        /// <summary>
        /// Object containing the clipping planes
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "clipping_planes")]
        public clipping_planes clipping_planes { get; set; }

        /// <summary>
        /// Bitmaps that should be included in this viewpoint
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "bitmaps")]
        public List<bitmap> bitmaps { get; set; }
    }
}