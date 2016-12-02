using System;
using System.Collections.Generic;
using iabi.BCF.APIObjects.Component;
using iabi.BCF.APIObjects.Viewpoint;

namespace iabi.BCF.Converter
{
    /// <summary>
    ///     This class represents a BCF viewpoint composed of BCF API objects
    /// </summary>
    public class ViewpointContainer
    {
        private List<component_GET> _Components;

        private byte[] _Snapshot;
        private viewpoint_GET _Viewpoint;

        /// <summary>
        ///     The actual viewpoint
        /// </summary>
        public viewpoint_GET Viewpoint
        {
            get { return _Viewpoint; }
            set
            {
                _Viewpoint = value;
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(_Viewpoint.guid))
                    {
                        _Viewpoint.guid = Guid.NewGuid().ToString();
                    }
                }
            }
        }

        /// <summary>
        ///     The components of the viewpoint (ifc geometry items)
        /// </summary>
        public List<component_GET> Components
        {
            get { return _Components; }
            set
            {
                _Components = value;
                if (value != null)
                {
                    if (Viewpoint == null)
                    {
                        Viewpoint = new viewpoint_GET();
                    }
                }
            }
        }

        /// <summary>
        ///     The screenshot for the viewpoint
        /// </summary>
        public byte[] Snapshot
        {
            get { return _Snapshot; }
            set
            {
                _Snapshot = value;
                if (value != null)
                {
                    if (Viewpoint == null)
                    {
                        Viewpoint = new viewpoint_GET();
                    }
                }
            }
        }
    }
}