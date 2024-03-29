using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dangl.BCF.APIObjects.V10.Extensions
{
    /// <summary>
    ///     PATCH representation of the extensions object
    /// </summary>
    [JsonObject(Title = "extensions")]
    public class extensions_PATCH : extensions_Base
    {
        private List<string> _priority;

        private List<string> _snippet_type;

        private List<string> _topic_label;

        private List<string> _topic_status;

        private List<string> _topic_type;

        private List<string> _user_id_type;

        /// <summary>
        /// Allowed topic types in the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_type")]
        public override List<string> topic_type
        {
            get { return _topic_type ?? (_topic_type = new List<string>()); }
            set { _topic_type = value; }
        }

        /// <summary>
        /// Allowed topic stati in the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_status")]
        public override List<string> topic_status
        {
            get { return _topic_status ?? (_topic_status = new List<string>()); }
            set { _topic_status = value; }
        }

        /// <summary>
        /// Allowed topic labels in the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "topic_label")]
        public override List<string> topic_label
        {
            get { return _topic_label ?? (_topic_label = new List<string>()); }
            set { _topic_label = value; }
        }

        /// <summary>
        /// Allowed snippet types in the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "snippet_type")]
        public override List<string> snippet_type
        {
            get { return _snippet_type ?? (_snippet_type = new List<string>()); }
            set { _snippet_type = value; }
        }

        /// <summary>
        /// Allowed priorities in the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "priority")]
        public override List<string> priority
        {
            get { return _priority ?? (_priority = new List<string>()); }
            set { _priority = value; }
        }

        /// <summary>
        /// List of user emails in the project
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "user_id_type")]
        public override List<string> user_id_type
        {
            get { return _user_id_type ?? (_user_id_type = new List<string>()); }
            set { _user_id_type = value; }
        }
    }
}