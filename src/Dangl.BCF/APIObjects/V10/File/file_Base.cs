using System;
using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.File
{
    /// <summary>
    /// Base class for the file object
    /// </summary>
    [JsonObject(Title = "file")]
    public abstract class file_Base
    {
        /// <summary>
        /// Guid of IfcProject
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "ifc_project")]
        public virtual string ifc_project { get; set; }

        /// <summary>
        /// Optional reference to an IfcSpatialStructure, e.g. a storey or a room
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "ifc_spatial_structure_element")]
        public virtual string ifc_spatial_structure_element { get; set; }

        /// <summary>
        /// Filename of the referenced file
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "file_name")]
        public virtual string file_name { get; set; }

        /// <summary>
        /// Creation date of the referenced file
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "date")]
        public virtual DateTime date { get; set; }

        /// <summary>
        /// Url reference to the file location
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "reference")]
        public virtual string reference { get; set; }
    }
}