using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.V10.Extensions
{
    /// <summary>
    /// Base class of the extensions object
    /// </summary>
    [JsonObject(Title = "extensions")]
    public abstract class extensions_Base
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
        [JsonProperty(Required = Required.Always, PropertyName = "topic_type")]
        public virtual List<string> topic_type
        {
            get { return _topic_type ?? (_topic_type = new List<string>()); }
            set { _topic_type = value; }
        }

        /// <summary>
        /// Allowed topic stati in the project
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "topic_status")]
        public virtual List<string> topic_status
        {
            get { return _topic_status ?? (_topic_status = new List<string>()); }
            set { _topic_status = value; }
        }

        /// <summary>
        /// Allowed topic labels in the project
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "topic_label")]
        public virtual List<string> topic_label
        {
            get { return _topic_label ?? (_topic_label = new List<string>()); }
            set { _topic_label = value; }
        }

        /// <summary>
        /// Allowed snippet types in the project
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "snippet_type")]
        public virtual List<string> snippet_type
        {
            get { return _snippet_type ?? (_snippet_type = new List<string>()); }
            set { _snippet_type = value; }
        }

        /// <summary>
        /// Allowed priorities in the project
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "priority")]
        public virtual List<string> priority
        {
            get { return _priority ?? (_priority = new List<string>()); }
            set { _priority = value; }
        }

        /// <summary>
        /// List of user emails in the project
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "user_id_type")]
        public virtual List<string> user_id_type
        {
            get { return _user_id_type ?? (_user_id_type = new List<string>()); }
            set { _user_id_type = value; }
        }

        /// <summary>
        /// Indicates true if all list properties are empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return !(topic_status.Any()
                     || topic_type.Any()
                     || topic_label.Any()
                     || snippet_type.Any()
                     || priority.Any()
                     || user_id_type.Any());
        }
    }
}