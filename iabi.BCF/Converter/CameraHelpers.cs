using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Converter
{
    public static class CameraHelpers
    {
        public static bool AnyValueSet(this OrthogonalCamera Camera)
        {
            return Camera.ViewToWorldScale != 0
                   || Camera.CameraViewPoint.X != 0
                   || Camera.CameraViewPoint.Y != 0
                   || Camera.CameraViewPoint.Z != 0
                   || Camera.CameraUpVector.Z != 0
                   || Camera.CameraUpVector.Z != 0
                   || Camera.CameraUpVector.Z != 0
                   || Camera.CameraDirection.Z != 0
                   || Camera.CameraDirection.Z != 0
                   || Camera.CameraDirection.Z != 0;
        }

        public static bool AnyValueSet(this PerspectiveCamera Camera)
        {
            return Camera.FieldOfView != 0
                   || Camera.CameraViewPoint.X != 0
                   || Camera.CameraViewPoint.Y != 0
                   || Camera.CameraViewPoint.Z != 0
                   || Camera.CameraUpVector.Z != 0
                   || Camera.CameraUpVector.Z != 0
                   || Camera.CameraUpVector.Z != 0
                   || Camera.CameraDirection.Z != 0
                   || Camera.CameraDirection.Z != 0
                   || Camera.CameraDirection.Z != 0;
        }
    }
}