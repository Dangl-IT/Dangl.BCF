//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.BCF.APIObjects.V21
{
    public class Extensions_GET
    {
        
        private List<string> _topic_type;
        
        private List<string> _topic_status;
        
        private List<string> _topic_label;
        
        private List<string> _snippet_type;
        
        private List<string> _priority;
        
        private List<string> _user_id_type;
        
        private List<string> _stage;
        
        private List<Project_actions> _project_actions;
        
        private List<Topic_actions> _topic_actions;
        
        private List<Comment_actions> _comment_actions;
        
        [Required()]
        public virtual List<string> Topic_type
        {
            get
            {
                return _topic_type;
            }
            set
            {
                _topic_type = value;
            }
        }
        
        [Required()]
        public virtual List<string> Topic_status
        {
            get
            {
                return _topic_status;
            }
            set
            {
                _topic_status = value;
            }
        }
        
        [Required()]
        public virtual List<string> Topic_label
        {
            get
            {
                return _topic_label;
            }
            set
            {
                _topic_label = value;
            }
        }
        
        [Required()]
        public virtual List<string> Snippet_type
        {
            get
            {
                return _snippet_type;
            }
            set
            {
                _snippet_type = value;
            }
        }
        
        [Required()]
        public virtual List<string> Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }
        
        [Required()]
        public virtual List<string> User_id_type
        {
            get
            {
                return _user_id_type;
            }
            set
            {
                _user_id_type = value;
            }
        }
        
        [Required()]
        public virtual List<string> Stage
        {
            get
            {
                return _stage;
            }
            set
            {
                _stage = value;
            }
        }
        
        public virtual List<Project_actions> Project_actions
        {
            get
            {
                return _project_actions;
            }
            set
            {
                _project_actions = value;
            }
        }
        
        public virtual List<Topic_actions> Topic_actions
        {
            get
            {
                return _topic_actions;
            }
            set
            {
                _topic_actions = value;
            }
        }
        
        public virtual List<Comment_actions> Comment_actions
        {
            get
            {
                return _comment_actions;
            }
            set
            {
                _comment_actions = value;
            }
        }
    }
}
