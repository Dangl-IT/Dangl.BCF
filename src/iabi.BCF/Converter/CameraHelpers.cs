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
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsAnyValueSet(this OrthogonalCamera camera)
        {
            return Math.Abs(camera.ViewToWorldScale) > 0.01
                   || Math.Abs(camera.CameraViewPoint.X) > 0.01
                   || Math.Abs(camera.CameraViewPoint.Y) > 0.01
                   || Math.Abs(camera.CameraViewPoint.Z) > 0.01
                   || Math.Abs(camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(camera.CameraDirection.Z) > 0.01
                   || Math.Abs(camera.CameraDirection.Z) > 0.01
                   || Math.Abs(camera.CameraDirection.Z) > 0.01;
        }

        /// <summary>
        /// Returns true if this camera has any values set and should therefore be serialized
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsAnyValueSet(this PerspectiveCamera camera)
        {
            return Math.Abs(camera.FieldOfView) > 0.01
                   || Math.Abs(camera.CameraViewPoint.X) > 0.01
                   || Math.Abs(camera.CameraViewPoint.Y) > 0.01
                   || Math.Abs(camera.CameraViewPoint.Z) > 0.01
                   || Math.Abs(camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(camera.CameraUpVector.Z) > 0.01
                   || Math.Abs(camera.CameraDirection.Z) > 0.01
                   || Math.Abs(camera.CameraDirection.Z) > 0.01
                   || Math.Abs(camera.CameraDirection.Z) > 0.01;
        }
    }
}