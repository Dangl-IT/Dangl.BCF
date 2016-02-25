using System;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Converter
{
    public static class CameraHelpers
    {
        public static bool AnyValueSet(this OrthogonalCamera Camera)
        {
            return Math.Abs(Camera.ViewToWorldScale) > 0.01
                   || Math.Abs(Camera.CameraViewPoint.X) > 0.01
                   || Math.Abs(Camera.CameraViewPoint.Y) > 0.01
                   || Math.Abs(Camera.CameraViewPoint.Z) > 0.01
                   || Math.Abs(Camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(Camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(Camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(Camera.CameraDirection.Z) > 0.01
                   || Math.Abs(Camera.CameraDirection.Z) > 0.01
                   || Math.Abs(Camera.CameraDirection.Z) > 0.01;
        }

        public static bool AnyValueSet(this PerspectiveCamera Camera)
        {
            return Math.Abs(Camera.FieldOfView) > 0.01
                   || Math.Abs(Camera.CameraViewPoint.X) > 0.01
                   || Math.Abs(Camera.CameraViewPoint.Y) > 0.01
                   || Math.Abs(Camera.CameraViewPoint.Z) > 0.01
                   || Math.Abs(Camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(Camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(Camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(Camera.CameraDirection.Z) > 0.01
                   || Math.Abs(Camera.CameraDirection.Z) > 0.01
                   || Math.Abs(Camera.CameraDirection.Z) > 0.01;
        }
    }
}