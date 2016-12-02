using System.Collections.Generic;
using iabi.BCF.APIObjects.Extensions;
using iabi.BCF.APIObjects.Project;

namespace iabi.BCF.Converter
{
    /// <summary>
    ///     This class represents a single BCF file composed of BCF API objects.
    ///     Note that bitmaps are not supported for the API.
    /// </summary>
    public class APIContainer
    {
        private Dictionary<string, byte[]> _FileAttachments;

        private List<TopicContainer> _Topics;

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
                if (_Topics == null)
                {
                    _Topics = new List<TopicContainer>();
                }
                return _Topics;
            }
            set { _Topics = value; }
        }

        /// <summary>
        ///     Binary file attachments with file name serving as key
        /// </summary>
        public Dictionary<string, byte[]> FileAttachments
        {
            get { return _FileAttachments ?? (_FileAttachments = new Dictionary<string, byte[]>()); }
        }
    }
}