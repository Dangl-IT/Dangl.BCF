using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Component
{
    /// <summary>
    /// Base class for the component
    /// </summary>
    [JsonObject(Title = "component")]
    public abstract class component_Base
    {
        /// <summary>
        /// IfcGuid of referenced object
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "ifc_guid")]
        public virtual string ifc_guid { get; set; }

        /// <summary>
        /// Indication if referenced object is selected
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "selected")]
        public virtual bool selected { get; set; }

        /// <summary>
        /// Indication if referenced object is visible
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "visible")]
        public virtual bool visible { get; set; }

        /// <summary>
        /// (A)RGB color of referenced object (6 or 8 bytes)
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "color")]
        public virtual string color { get; set; }

        /// <summary>
        /// System where the referenced object originates from, e.g. creating CAD application
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "originating_system")]
        public virtual string originating_system { get; set; }

        /// <summary>
        /// Id of the tool creating the component, e.g. an IFC viewer
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "authoring_tool_id")]
        public virtual string authoring_tool_id { get; set; }
    }
}