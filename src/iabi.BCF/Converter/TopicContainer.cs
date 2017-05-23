using System.Collections.Generic;
using iabi.BCF.APIObjects.V10.Comment;
using iabi.BCF.APIObjects.V10.DocumentReference;
using iabi.BCF.APIObjects.V10.File;
using iabi.BCF.APIObjects.V10.RelatedTopic;
using iabi.BCF.APIObjects.V10.Topic;

namespace iabi.BCF.Converter
{
    /// <summary>
    ///     This class represents a BCF topic composed of BCF API objects
    /// </summary>
    public class TopicContainer
    {
        private List<comment_GET> _comments;
        private List<file_GET> _files;
        private List<document_reference_GET> _referencedDocuments;
        private List<related_topic_GET> _relatedTopics;
        private List<ViewpointContainer> _viewpoints;

        /// <summary>
        ///     The actual topic
        /// </summary>
        public topic_GET Topic { get; set; }

        /// <summary>
        ///     The BIM Snippet in binary form
        /// </summary>
        public byte[] SnippetData { get; set; }

        /// <summary>
        ///     A list of file references
        /// </summary>
        public List<file_GET> Files
        {
            get { return _files ?? (_files = new List<file_GET>()); }
            set { _files = value; }
        }

        /// <summary>
        ///     A list of related topics
        /// </summary>
        public List<related_topic_GET> RelatedTopics
        {
            get
            {
                if (_relatedTopics == null)
                {
                    _relatedTopics = new List<related_topic_GET>();
                }
                return _relatedTopics;
            }
            set { _relatedTopics = value; }
        }

        /// <summary>
        ///     A list of referenced documents
        /// </summary>
        public List<document_reference_GET> ReferencedDocuments
        {
            get
            {
                if (_referencedDocuments == null)
                {
                    _referencedDocuments = new List<document_reference_GET>();
                }
                return _referencedDocuments;
            }
            set { _referencedDocuments = value; }
        }

        /// <summary>
        ///     Comments within this topic
        /// </summary>
        public List<comment_GET> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new List<comment_GET>();
                }
                return _comments;
            }
            set { _comments = value; }
        }

        /// <summary>
        ///     Viewpoints within this topic
        /// </summary>
        public List<ViewpointContainer> Viewpoints
        {
            get
            {
                if (_viewpoints == null)
                {
                    _viewpoints = new List<ViewpointContainer>();
                }
                return _viewpoints;
            }
            set { _viewpoints = value; }
        }
    }
}