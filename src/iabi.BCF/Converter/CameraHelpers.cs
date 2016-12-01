using System;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Converter
{
    /// <summary>
    /// Static methods to provide informations about BCF cameras
    /// </summary>
    public static class CameraHelpers
    {
        /// <summary>
        /// Returns true if this camera has any values set and should therefore be serialized
        /// </summary>
        /// <param name="Camera"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns true if this camera has any values set and should therefore be serialized
        /// </summary>
        /// <param name="Camera"></param>
        /// <returns></returns>
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