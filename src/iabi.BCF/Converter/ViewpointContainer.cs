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
        private List<component_GET> _components;
        private byte[] _snapshot;
        private viewpoint_GET _viewpoint;

        /// <summary>
        ///     The actual viewpoint
        /// </summary>
        public viewpoint_GET Viewpoint
        {
            get { return _viewpoint; }
            set
            {
                _viewpoint = value;
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(_viewpoint.guid))
                    {
                        _viewpoint.guid = Guid.NewGuid().ToString();
                    }
                }
            }
        }

        /// <summary>
        ///     The components of the viewpoint (ifc geometry items)
        /// </summary>
        public List<component_GET> Components
        {
            get { return _components; }
            set
            {
                _components = value;
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
            get { return _snapshot; }
            set
            {
                _snapshot = value;
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