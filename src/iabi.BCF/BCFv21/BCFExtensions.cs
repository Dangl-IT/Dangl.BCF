using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;

// TODO RENAME CLASS
// TODO ADD COMMENT FOR ORIGINATING APPLICATION AND TEST IT
// TODO ADD COMMENTS ON ALL GENERATED XML FILES (MARKUP, VIEWPOINT ETC) AND TEST IT

namespace iabi.BCF.BCFv21
{
    public class BCFExtensions
    {
        private List<string> _Priority;

        private List<string> _SnippetType;

        private List<string> _TopicLabel;

        private List<string> _TopicStatus;

        private List<string> _TopicType;

        private List<string> _UserIdType;

        private List<string> _Stage;

        /// <summary>
        /// Wil initialize the object from the passed string parameter.
        /// </summary>
        /// <param name="schemaString"></param>
        public BCFExtensions(string schemaString)
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
            RestrictionBaseElement = SchemaXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Attributes().Any(Attr => Attr.Name.LocalName == "name" && Attr.Value == "Stage"));
            if (RestrictionBaseElement != null)
            {
                Stage = RestrictionBaseElement.Nodes().OfType<XElement>().First().Nodes().OfType<XElement>().Select(Curr => Curr.Attribute("value").Value).ToList();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BCFExtensions()
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
        /// List of user emails within the project
        /// </summary>
        public List<string> Stage
        {
            get { return _Stage ?? (_Stage = new List<string>()); }
            internal set { _Stage = value; }
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

            return extensionsDocument.ToString();

            throw new NotImplementedException();
            try
            {
                //var ExtensionsSchemaToWrite = new XmlSchema();
                //// Add the redfines tag
                //var SchemaRedefine = new XmlSchemaRedefine();
                //SchemaRedefine.SchemaLocation = "markup.xsd";
                //ExtensionsSchemaToWrite.Includes.Add(SchemaRedefine);
                //// Add the items
                //SchemaRedefine.Items.Add(TopicTypeSchemas());
                //SchemaRedefine.Items.Add(TopicStatusSchemas());
                //SchemaRedefine.Items.Add(TopicLabelSchemas());
                //SchemaRedefine.Items.Add(SnippetTypeSchemas());
                //SchemaRedefine.Items.Add(PrioritySchemas());
                //SchemaRedefine.Items.Add(UserIdSchemas());
                //SchemaRedefine.Items.Add(StageSchemas());
                //using (var CurrentMemoryStream = new MemoryStream())
                //{
                //    ExtensionsSchemaToWrite.Write(CurrentMemoryStream);

                //    using (var reader = new StreamReader(CurrentMemoryStream))
                //    {
                //        CurrentMemoryStream.Position = 0;
                //        var SchemaString = reader.ReadToEnd();
                //        return SchemaString;
                //    }
                //}
            }
            catch
            {
                return "";
            }
        }

        //private static XmlSchemaSimpleTypeRestriction EnumRestrictions(IReadOnlyCollection<string> Values, XmlQualifiedName Name)
        //{
        //    var RestrictionsRoReturn = new XmlSchemaSimpleTypeRestriction();
        //    RestrictionsRoReturn.BaseTypeName = Name;
        //    if (Values.Count > 0)
        //    {
        //        foreach (var GivenValue in Values)
        //        {
        //            RestrictionsRoReturn.Facets.Add(new XmlSchemaEnumerationFacet { Value = GivenValue });
        //        }
        //    }
        //    return RestrictionsRoReturn;
        //}

        //private XmlSchemaSimpleType TopicTypeSchemas()
        //{
        //    var TopicTypesSchemaElement = new XmlSchemaSimpleType();
        //    TopicTypesSchemaElement.Name = "TopicType";
        //    TopicTypesSchemaElement.Content = EnumRestrictions(TopicType, TopicTypesSchemaElement.QualifiedName);
        //    return TopicTypesSchemaElement;
        //}

        //private XmlSchemaSimpleType TopicStatusSchemas()
        //{
        //    var TopicStatusSchemaElement = new XmlSchemaSimpleType();
        //    TopicStatusSchemaElement.Name = "TopicStatus";
        //    TopicStatusSchemaElement.Content = EnumRestrictions(TopicStatus, TopicStatusSchemaElement.QualifiedName);
        //    return TopicStatusSchemaElement;
        //}

        //private XmlSchemaSimpleType TopicLabelSchemas()
        //{
        //    var TopicLabelSchemaElement = new XmlSchemaSimpleType();
        //    TopicLabelSchemaElement.Name = "TopicLabel";
        //    TopicLabelSchemaElement.Content = EnumRestrictions(TopicLabel, TopicLabelSchemaElement.QualifiedName);
        //    return TopicLabelSchemaElement;
        //}

        //private XmlSchemaSimpleType SnippetTypeSchemas()
        //{
        //    var SnippetTypeSchemaElement = new XmlSchemaSimpleType();
        //    SnippetTypeSchemaElement.Name = "SnippetType";
        //    SnippetTypeSchemaElement.Content = EnumRestrictions(SnippetType, SnippetTypeSchemaElement.QualifiedName);
        //    return SnippetTypeSchemaElement;
        //}

        //private XmlSchemaSimpleType PrioritySchemas()
        //{
        //    var PrioritySchemaElement = new XmlSchemaSimpleType();
        //    PrioritySchemaElement.Name = "Priority";
        //    PrioritySchemaElement.Content = EnumRestrictions(Priority, PrioritySchemaElement.QualifiedName);
        //    return PrioritySchemaElement;
        //}

        //private XmlSchemaSimpleType UserIdSchemas()
        //{
        //    var UserIdTypeSchemaElement = new XmlSchemaSimpleType();
        //    UserIdTypeSchemaElement.Name = "UserIdType";
        //    UserIdTypeSchemaElement.Content = EnumRestrictions(UserIdType, UserIdTypeSchemaElement.QualifiedName);
        //    return UserIdTypeSchemaElement;
        //}

        //private XmlSchemaSimpleType StageSchemas()
        //{
        //    var UserIdTypeSchemaElement = new XmlSchemaSimpleType();
        //    UserIdTypeSchemaElement.Name = "Stage";
        //    UserIdTypeSchemaElement.Content = EnumRestrictions(UserIdType, UserIdTypeSchemaElement.QualifiedName);
        //    return UserIdTypeSchemaElement;
        //}

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
