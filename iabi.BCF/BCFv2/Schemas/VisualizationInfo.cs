using System;
using System.Xml.Serialization;

namespace iabi.BCF.BCFv2.Schemas
{
    public partial class VisualizationInfo
    {
        [XmlIgnore]
        private string _GUID;

        /// <summary>
        /// Implemented due to the actual VisualizationInfo XSD schema not defining a viewpoint
        /// and therefore not allowing to link the Viewpoints in the Markup with the actual
        /// VisualizationInfo instances.
        /// </summary>
        [XmlIgnore]
        public string GUID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_GUID))
                {
                    _GUID = Guid.NewGuid().ToString();
                }
                return _GUID;
            }
            set
            {
                _GUID = value;
            }
        }
    }
}