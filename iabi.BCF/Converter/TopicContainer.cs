﻿using iabi.BCF.APIObjects.Comment;
using iabi.BCF.APIObjects.DocumentReference;
using iabi.BCF.APIObjects.File;
using iabi.BCF.APIObjects.RelatedTopic;
using iabi.BCF.APIObjects.Topic;
using System.Collections.Generic;

namespace iabi.BCF.Converter
{
    /// <summary>
    /// This class represents a BCF topic composed of BCF API objects
    /// </summary>
    public class TopicContainer
    {
        /// <summary>
        /// The actual topic
        /// </summary>
        public topic_GET Topic { get; set; }

        /// <summary>
        /// The BIM Snippet in binary form
        /// </summary>
        public byte[] SnippetData { get; set; }

        private List<file_GET> _Files;

        /// <summary>
        /// A list of file references
        /// </summary>
        public List<file_GET> Files
        {
            get
            {
                return _Files ?? (_Files = new List<file_GET>());
            }
            set
            {
                _Files = value;
            }
        }

        private List<related_topic_GET> _RelatedTopics;

        /// <summary>
        /// A list of related topics
        /// </summary>
        public List<related_topic_GET> RelatedTopics
        {
            get
            {
                if (_RelatedTopics == null)
                {
                    _RelatedTopics = new List<related_topic_GET>();
                }
                return _RelatedTopics;
            }
            set
            {
                _RelatedTopics = value;
            }
        }

        private List<document_reference_GET> _ReferencedDocuments;

        /// <summary>
        /// A list of referenced documents
        /// </summary>
        public List<document_reference_GET> ReferencedDocuments
        {
            get
            {
                if (_ReferencedDocuments == null)
                {
                    _ReferencedDocuments = new List<document_reference_GET>();
                }
                return _ReferencedDocuments;
            }
            set
            {
                _ReferencedDocuments = value;
            }
        }

        private List<comment_GET> _Comments;

        /// <summary>
        /// Comments within this topic
        /// </summary>
        public List<comment_GET> Comments
        {
            get
            {
                if (_Comments == null)
                {
                    _Comments = new List<comment_GET>();
                }
                return _Comments;
            }
            set
            {
                _Comments = value;
            }
        }

        private List<ViewpointContainer> _Viewpoints;

        /// <summary>
        /// Viewpoints within this topic
        /// </summary>
        public List<ViewpointContainer> Viewpoints
        {
            get
            {
                if (_Viewpoints == null)
                {
                    _Viewpoints = new List<ViewpointContainer>();
                }
                return _Viewpoints;
            }
            set
            {
                _Viewpoints = value;
            }
        }
    }
}