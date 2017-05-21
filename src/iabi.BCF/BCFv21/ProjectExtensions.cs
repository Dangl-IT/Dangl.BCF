using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

// TODO SYNC THIS FILES LOCATION WITH THE ONE FOR V2

namespace iabi.BCF.BCFv21
{
    public class ProjectExtensions
    {
        private List<string> _priority;
        private List<string> _snippetType;
        private List<string> _topicLabel;
        private List<string> _topicStatus;
        private List<string> _topicType;
        private List<string> _userIdType;
        private List<string> _stage;

        /// <summary>
        /// Wil initialize the object from the passed string parameter.
        /// </summary>
        /// <param name="schemaString"></param>
        public ProjectExtensions(string schemaString)
        {
            var schemaXml = XElement.Parse(schemaString);

            var restrictionBaseElement = schemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "TopicType"));
            if (restrictionBaseElement != null)
            {
                TopicType = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(e => e.Attribute("value").Value).ToList();
            }
            restrictionBaseElement = schemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "TopicStatus"));
            if (restrictionBaseElement != null)
            {
                TopicStatus = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(e => e.Attribute("value").Value).ToList();
            }
            restrictionBaseElement = schemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "TopicLabel"));
            if (restrictionBaseElement != null)
            {
                TopicLabel = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(e => e.Attribute("value").Value).ToList();
            }
            restrictionBaseElement = schemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "SnippetType"));
            if (restrictionBaseElement != null)
            {
                SnippetType = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(e => e.Attribute("value").Value).ToList();
            }
            restrictionBaseElement = schemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "Priority"));
            if (restrictionBaseElement != null)
            {
                Priority = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(e => e.Attribute("value").Value).ToList();
            }
            restrictionBaseElement = schemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "UserIdType"));
            if (restrictionBaseElement != null)
            {
                UserIdType = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(e => e.Attribute("value").Value).ToList();
            }
            restrictionBaseElement = schemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "Stage"));
            if (restrictionBaseElement != null)
            {
                Stage = restrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(e => e.Attribute("value").Value).ToList();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectExtensions()
        {
        }

        /// <summary>
        /// List of allowed topic types within the project
        /// </summary>
        public List<string> TopicType
        {
            get { return _topicType ?? (_topicType = new List<string>()); }
            internal set { _topicType = value; }
        }

        /// <summary>
        /// List of allowed topic stati within the project
        /// </summary>
        public List<string> TopicStatus
        {
            get { return _topicStatus ?? (_topicStatus = new List<string>()); }
            internal set { _topicStatus = value; }
        }

        /// <summary>
        /// List of allowed topic labels within the project
        /// </summary>
        public List<string> TopicLabel
        {
            get { return _topicLabel ?? (_topicLabel = new List<string>()); }
            internal set { _topicLabel = value; }
        }

        /// <summary>
        /// List of allowed snippet types within the project
        /// </summary>
        public List<string> SnippetType
        {
            get { return _snippetType ?? (_snippetType = new List<string>()); }
            internal set { _snippetType = value; }
        }

        /// <summary>
        /// List of allowed priorities within the project
        /// </summary>
        public List<string> Priority
        {
            get { return _priority ?? (_priority = new List<string>()); }
            internal set { _priority = value; }
        }

        /// <summary>
        /// List of user emails within the project
        /// </summary>
        public List<string> UserIdType
        {
            get { return _userIdType ?? (_userIdType = new List<string>()); }
            internal set { _userIdType = value; }
        }

        /// <summary>
        /// List of user emails within the project
        /// </summary>
        public List<string> Stage
        {
            get { return _stage ?? (_stage = new List<string>()); }
            internal set { _stage = value; }
        }

        /// <summary>
        /// Returns the string representation of the extension.xsd file
        /// </summary>
        /// <returns></returns>
        public string WriteExtension()
        {
            var extensionsDocument = new XDocument();
            var extensionsRoot = new XElement((XNamespace)"http://www.w3.org/2001/XMLSchema" + "schema");
            extensionsDocument.Add(extensionsRoot);

            var redefineElement = new XElement((XNamespace)"http://www.w3.org/2001/XMLSchema" + "redefine");
            extensionsRoot.Add(redefineElement);
            redefineElement.SetAttributeValue("schemaLocation", "markup.xsd");

            Action<string, IEnumerable<string>> addRedefinition = (name, values) =>
            {
                var valueRedefiningElement = new XElement((XNamespace)"http://www.w3.org/2001/XMLSchema" + "simpleType");
                redefineElement.Add(valueRedefiningElement);
                valueRedefiningElement.SetAttributeValue("name", name);
                var restrictionBaseElement = new XElement((XNamespace)"http://www.w3.org/2001/XMLSchema" + "restriction");
                valueRedefiningElement.Add(restrictionBaseElement);
                restrictionBaseElement.SetAttributeValue("base", name);
                foreach (var value in values)
                {
                    var enumerationElement = new XElement((XNamespace)"http://www.w3.org/2001/XMLSchema" + "enumeration");
                    restrictionBaseElement.Add(enumerationElement);
                    enumerationElement.SetAttributeValue("value", value);
                }
            };

            addRedefinition("TopicType", TopicType);
            addRedefinition("TopicStatus", TopicStatus);
            addRedefinition("TopicLabel", TopicLabel);
            addRedefinition("SnippetType", SnippetType);
            addRedefinition("Priority", Priority);
            addRedefinition("UserIdType", UserIdType);
            addRedefinition("Stage", Stage);

            extensionsDocument.Declaration = new XDeclaration("1.0", "utf-8", "yes");

            using (var memStream = new MemoryStream())
            {
                using (var streamReader = new StreamReader(memStream))
                {
                    extensionsDocument.Save(memStream);
                    memStream.Position = 0;
                    var xmlString = streamReader.ReadToEnd();
                    return xmlString;
                }
            }
        }

        /// <summary>
        /// Indicates true if all list properties are empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return
                (TopicType == null || TopicType.Count == 0)
                && (TopicStatus == null || TopicStatus.Count == 0)
                && (TopicLabel == null || TopicLabel.Count == 0)
                && (Priority == null || Priority.Count == 0)
                && (SnippetType == null || SnippetType.Count == 0)
                && (UserIdType == null || UserIdType.Count == 0)
                && (Stage == null || Stage.Count == 0);
        }
    }
}
