using System.Collections.Generic;
using Dangl.BCF.APIObjects.V10.Extensions;
using Dangl.BCF.APIObjects.V10.Project;

namespace Dangl.BCF.Converter
{
    /// <summary>
    ///     This class represents a single BCF file composed of BCF API objects.
    ///     Note that bitmaps are not supported for the API.
    /// </summary>
    public class APIContainer
    {
        private Dictionary<string, byte[]> _fileAttachments;
        private List<TopicContainer> _topics;

        /// <summary>
        ///     Information about the BCF project
        /// </summary>
        public project_GET Project { get; set; }

        /// <summary>
        ///     BCF project extensions
        /// </summary>
        public extensions_GET Extensions { get; set; }

        /// <summary>
        ///     Contains topics for this container
        /// </summary>
        public List<TopicContainer> Topics
        {
            get
            {
                if (_topics == null)
                {
                    _topics = new List<TopicContainer>();
                }
                return _topics;
            }
            set { _topics = value; }
        }

        /// <summary>
        ///     Binary file attachments with file name serving as key
        /// </summary>
        public Dictionary<string, byte[]> FileAttachments
        {
            get { return _fileAttachments ?? (_fileAttachments = new Dictionary<string, byte[]>()); }
        }
    }
}