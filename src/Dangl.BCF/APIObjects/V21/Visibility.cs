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

namespace Dangl.BCF.APIObjects.V21
{
    public class Visibility
    {
        
        private bool _default_visibility;
        
        private List<Component> _exceptions;
        
        private View_setup_hints _view_setup_hints;
        
        public Visibility()
        {
            _default_visibility = false;
        }
        
        public virtual bool Default_visibility
        {
            get
            {
                return _default_visibility;
            }
            set
            {
                _default_visibility = value;
            }
        }
        
        public virtual List<Component> Exceptions
        {
            get
            {
                return _exceptions;
            }
            set
            {
                _exceptions = value;
            }
        }
        
        public virtual View_setup_hints View_setup_hints
        {
            get
            {
                return _view_setup_hints;
            }
            set
            {
                _view_setup_hints = value;
            }
        }
    }
}
