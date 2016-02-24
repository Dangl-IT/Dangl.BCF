using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace iabi.BCF.APIObjects.Extensions
{
    [JsonObject(Title = "extensions")]
    public abstract class extensions_Base
    {
        private List<string> _topic_type;

        [JsonProperty(Required = Required.Always, PropertyName = "topic_type")]
        public virtual List<string> topic_type
        {
            get
            {
                return _topic_type ?? (_topic_type = new List<string>());
            }
            set
            {
                _topic_type = value;
            }
        }

        private List<string> _topic_status;

        [JsonProperty(Required = Required.Always, PropertyName = "topic_status")]
        public virtual List<string> topic_status
        {
            get
            {
                return _topic_status ?? (_topic_status = new List<string>());
            }
            set
            {
                _topic_status = value;
            }
        }

        private List<string> _topic_label;

        [JsonProperty(Required = Required.Always, PropertyName = "topic_label")]
        public virtual List<string> topic_label
        {
            get
            {
                return _topic_label ?? (_topic_label = new List<string>());
            }
            set
            {
                _topic_label = value;
            }
        }

        private List<string> _snippet_type;

        [JsonProperty(Required = Required.Always, PropertyName = "snippet_type")]
        public virtual List<string> snippet_type
        {
            get
            {
                return _snippet_type ?? (_snippet_type = new List<string>());
            }
            set
            {
                _snippet_type = value;
            }
        }

        private List<string> _priority;

        [JsonProperty(Required = Required.Always, PropertyName = "priority")]
        public virtual List<string> priority
        {
            get
            {
                return _priority ?? (_priority = new List<string>());
            }
            set
            {
                _priority = value;
            }
        }

        private List<string> _user_id_type;

        [JsonProperty(Required = Required.Always, PropertyName = "user_id_type")]
        public virtual List<string> user_id_type
        {
            get
            {
                return _user_id_type ?? (_user_id_type = new List<string>());
            }
            set
            {
                _user_id_type = value;
            }
        }

        public bool IsEmpty()
        {
            return !(topic_status.Any()
                || topic_type.Any()
                || topic_label.Any()
                || snippet_type.Any()
                || priority.Any()
                || user_id_type.Any());
        }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}