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
using System;

namespace Dangl.BCF.APIObjects.V21
{
    public class Document_GET
    {
        
        private Guid _guid;
        
        private string _filename;
        
        [Required()]
        public virtual Guid Guid
        {
            get
            {
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }
        
        [Required()]
        public virtual string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
            }
        }
    }
}
