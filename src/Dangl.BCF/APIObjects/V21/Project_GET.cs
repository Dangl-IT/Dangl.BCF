//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Dangl.BCF.APIObjects.V21
{
    // Schema for single project GET, BCF REST API.
    public class Project_GET
    {
        
        private string _project_id;
        
        private string _name;
        
        private Project_authorization _authorization;
        
        [Required()]
        public virtual string Project_id
        {
            get
            {
                return _project_id;
            }
            set
            {
                _project_id = value;
            }
        }
        
        [Required()]
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        
        public virtual Project_authorization Authorization
        {
            get
            {
                return _authorization;
            }
            set
            {
                _authorization = value;
            }
        }
    }
}
