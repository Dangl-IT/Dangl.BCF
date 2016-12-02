using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

// TODO ADD COMMENT FOR ORIGINATING APPLICATION AND TEST IT
// TODO ADD COMMENTS ON ALL GENERATED XML FILES (MARKUP, VIEWPOINT ETC) AND TEST IT
// TODO TEST FOR ALL XMLS IF DECLARATIONS ARE PRESENT

namespace iabi.BCF.BCFv2.Schemas
{
    /// <summary>
    /// Container for the project extensions schema. This schema is basically just having lists of allowed property values within a project
    /// </summary>
    public class ProjectExtensions
    {
        private List<string> _Priority;

        private List<string> _SnippetType;

        private List<string> _TopicLabel;

        private List<string> _TopicStatus;

        private List<string> _TopicType;

        private List<string> _UserIdType;

        /// <summary>
        /// Wil initialize the object from the passed string parameter.
        /// </summary>
        /// <param name="schemaString"></param>
        public ProjectExtensions(string schemaString)
        {
            var SchemaXml = XElement.Parse(schemaString);

            var RestrictionBaseElement = SchemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicType"));
            if (RestrictionBaseElement != null)
            {
                TopicType = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value).ToList();
            }
            RestrictionBaseElement = SchemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicStatus"));
            if (RestrictionBaseElement != null)
            {
                TopicStatus = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value).ToList();
            }
            RestrictionBaseElement = SchemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "TopicLabel"));
            if (RestrictionBaseElement != null)
            {
                TopicLabel = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value).ToList();
            }
            RestrictionBaseElement = SchemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "SnippetType"));
            if (RestrictionBaseElement != null)
            {
                SnippetType = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value).ToList();
            }
            RestrictionBaseElement = SchemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "Priority"));
            if (RestrictionBaseElement != null)
            {
                Priority = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value).ToList();
            }
            RestrictionBaseElement = SchemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "UserIdType"));
            if (RestrictionBaseElement != null)
            {
                UserIdType = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value).ToList();
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
            get { return _TopicType ?? (_TopicType = new List<string>()); }
            internal set { _TopicType = value; }
        }

        /// <summary>
        /// List of allowed topic stati within the project
        /// </summary>
        public List<string> TopicStatus
        {
            get { return _TopicStatus ?? (_TopicStatus = new List<string>()); }
            internal set { _TopicStatus = value; }
        }

        /// <summary>
        /// List of allowed topic labels within the project
        /// </summary>
        public List<string> TopicLabel
        {
            get { return _TopicLabel ?? (_TopicLabel = new List<string>()); }
            internal set { _TopicLabel = value; }
        }

        /// <summary>
        /// List of allowed snippet types within the project
        /// </summary>
        public List<string> SnippetType
        {
            get { return _SnippetType ?? (_SnippetType = new List<string>()); }
            internal set { _SnippetType = value; }
        }

        /// <summary>
        /// List of allowed priorities within the project
        /// </summary>
        public List<string> Priority
        {
            get { return _Priority ?? (_Priority = new List<string>()); }
            internal set { _Priority = value; }
        }

        /// <summary>
        /// List of user emails within the project
        /// </summary>
        public List<string> UserIdType
        {
            get { return _UserIdType ?? (_UserIdType = new List<string>()); }
            internal set { _UserIdType = value; }
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
                && (UserIdType == null || UserIdType.Count == 0);
        }
    }
}