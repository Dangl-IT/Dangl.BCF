//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dangl.BCF.APIObjects.V21
{
    public class Orthogonal_camera
    {
        
        private Point _camera_view_point;
        
        private Direction _camera_direction;
        
        private Direction _camera_up_vector;
        
        private float _view_to_world_scale;
        
        public virtual Point Camera_view_point
        {
            get
            {
                return _camera_view_point;
            }
            set
            {
                _camera_view_point = value;
            }
        }
        
        public virtual Direction Camera_direction
        {
            get
            {
                return _camera_direction;
            }
            set
            {
                _camera_direction = value;
            }
        }
        
        public virtual Direction Camera_up_vector
        {
            get
            {
                return _camera_up_vector;
            }
            set
            {
                _camera_up_vector = value;
            }
        }
        
        public virtual float View_to_world_scale
        {
            get
            {
                return _view_to_world_scale;
            }
            set
            {
                _view_to_world_scale = value;
            }
        }
    }
}
