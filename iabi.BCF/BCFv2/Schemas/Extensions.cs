using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace iabi.BCF.BCFv2.Schemas
{
    public class Extensions_XSD
    {
        private List<string> _Priority;

        private List<string> _SnippetType;

        private List<string> _TopicLabel;

        private List<string> _TopicStatus;
        private List<string> _TopicType;

        private List<string> _UserIdType;

        public Extensions_XSD(string SchemaString)
        {
            var SchemaXml = XElement.Parse(SchemaString);

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

            //try
            //{
            //    XmlSchema GivenSchema = new XmlSchema();
            //    using (MemoryStream CurrentMemoryStream = new MemoryStream())
            //    {
            //        using (StreamWriter CurrentStreamWriter = new StreamWriter(CurrentMemoryStream))
            //        {
            //            CurrentStreamWriter.Write(SchemaString);
            //            CurrentStreamWriter.Flush();
            //            CurrentMemoryStream.Position = 0;
            //            GivenSchema = XmlSchema.Read(CurrentMemoryStream, ValidationCallback);
            //        }
            //    }
            //    // Extract the Values
            //    foreach (XmlSchemaSimpleType CurrentType in GivenSchema.Items)
            //    {
            //        switch (CurrentType.Name)
            //        {
            //            case "TopicType":
            //                TopicType = ReadRestrictions((XmlSchemaSimpleTypeRestriction)CurrentType.Content);
            //                break;

            //            case "TopicStatus":
            //                TopicStatus = ReadRestrictions((XmlSchemaSimpleTypeRestriction)CurrentType.Content);
            //                break;

            //            case "TopicLabel":
            //                TopicLabel = ReadRestrictions((XmlSchemaSimpleTypeRestriction)CurrentType.Content);
            //                break;

            //            case "SnippetType":
            //                SnippetType = ReadRestrictions((XmlSchemaSimpleTypeRestriction)CurrentType.Content);
            //                break;

            //            case "Priority":
            //                Priority = ReadRestrictions((XmlSchemaSimpleTypeRestriction)CurrentType.Content);
            //                break;

            //            case "UserIdType":
            //                UserIdType = ReadRestrictions((XmlSchemaSimpleTypeRestriction)CurrentType.Content);
            //                break;

            //            default:
            //                // TODO GIVE ERROR WHEN READING
            //                break;
            //        }
            //    }
            //}
            //catch
            //{
            //}
        }

        public Extensions_XSD()
        {
        }

        public List<string> TopicType
        {
            get { return _TopicType ?? (_TopicType = new List<string>()); }
            internal set { _TopicType = value; }
        }

        public List<string> TopicStatus
        {
            get { return _TopicStatus ?? (_TopicStatus = new List<string>()); }
            internal set { _TopicStatus = value; }
        }

        public List<string> TopicLabel
        {
            get { return _TopicLabel ?? (_TopicLabel = new List<string>()); }
            internal set { _TopicLabel = value; }
        }

        public List<string> SnippetType
        {
            get { return _SnippetType ?? (_SnippetType = new List<string>()); }
            internal set { _SnippetType = value; }
        }

        public List<string> Priority
        {
            get { return _Priority ?? (_Priority = new List<string>()); }
            internal set { _Priority = value; }
        }

        public List<string> UserIdType
        {
            get { return _UserIdType ?? (_UserIdType = new List<string>()); }
            internal set { _UserIdType = value; }
        }

        public string WriteExtension()
        {
            try
            {
                var ExtensionsSchemaToWrite = new XmlSchema();
                // Add the redfines tag
                var SchemaRedefine = new XmlSchemaRedefine();
                SchemaRedefine.SchemaLocation = "markup.xsd";
                ExtensionsSchemaToWrite.Includes.Add(SchemaRedefine);
                // Add the items
                SchemaRedefine.Items.Add(TopicTypeSchemas());
                SchemaRedefine.Items.Add(TopicStatusSchemas());
                SchemaRedefine.Items.Add(TopicLabelSchemas());
                SchemaRedefine.Items.Add(SnippetTypeSchemas());
                SchemaRedefine.Items.Add(PrioritySchemas());
                SchemaRedefine.Items.Add(UserIdSchemas());
                using (var CurrentMemoryStream = new MemoryStream())
                {
                    ExtensionsSchemaToWrite.Write(CurrentMemoryStream);

                    using (var reader = new StreamReader(CurrentMemoryStream))
                    {
                        CurrentMemoryStream.Position = 0;
                        var SchemaString = reader.ReadToEnd();
                        return SchemaString;
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        private List<string> ReadRestrictions(XmlSchemaSimpleTypeRestriction GivenSchemaType)
        {
            var ListToReturn = new List<string>();
            foreach (XmlSchemaEnumerationFacet CurrentEnum in GivenSchemaType.Facets)
            {
                ListToReturn.Add(CurrentEnum.Value);
            }
            return ListToReturn;
        }

        private XmlSchemaSimpleTypeRestriction EnumRestrictions(List<string> Values, XmlQualifiedName Name)
        {
            var RestrictionsRoReturn = new XmlSchemaSimpleTypeRestriction();
            RestrictionsRoReturn.BaseTypeName = Name;
            if (Values.Count > 0)
            {
                foreach (var GivenValue in Values)
                {
                    var CurrentEnumItem = new XmlSchemaEnumerationFacet();
                    CurrentEnumItem.Value = GivenValue;
                    RestrictionsRoReturn.Facets.Add(CurrentEnumItem);
                }
            }
            return RestrictionsRoReturn;
        }

        private XmlSchemaSimpleType TopicTypeSchemas()
        {
            var TopicTypesSchemaElement = new XmlSchemaSimpleType();
            TopicTypesSchemaElement.Name = "TopicType";
            TopicTypesSchemaElement.Content = EnumRestrictions(TopicType, TopicTypesSchemaElement.QualifiedName);
            return TopicTypesSchemaElement;
        }

        private XmlSchemaSimpleType TopicStatusSchemas()
        {
            var TopicStatusSchemaElement = new XmlSchemaSimpleType();
            TopicStatusSchemaElement.Name = "TopicStatus";
            TopicStatusSchemaElement.Content = EnumRestrictions(TopicStatus, TopicStatusSchemaElement.QualifiedName);
            return TopicStatusSchemaElement;
        }

        private XmlSchemaSimpleType TopicLabelSchemas()
        {
            var TopicLabelSchemaElement = new XmlSchemaSimpleType();
            TopicLabelSchemaElement.Name = "TopicLabel";
            TopicLabelSchemaElement.Content = EnumRestrictions(TopicLabel, TopicLabelSchemaElement.QualifiedName);
            return TopicLabelSchemaElement;
        }

        private XmlSchemaSimpleType SnippetTypeSchemas()
        {
            var SnippetTypeSchemaElement = new XmlSchemaSimpleType();
            SnippetTypeSchemaElement.Name = "SnippetType";
            SnippetTypeSchemaElement.Content = EnumRestrictions(SnippetType, SnippetTypeSchemaElement.QualifiedName);
            return SnippetTypeSchemaElement;
        }

        private XmlSchemaSimpleType PrioritySchemas()
        {
            var PrioritySchemaElement = new XmlSchemaSimpleType();
            PrioritySchemaElement.Name = "Priority";
            PrioritySchemaElement.Content = EnumRestrictions(Priority, PrioritySchemaElement.QualifiedName);
            return PrioritySchemaElement;
        }

        private XmlSchemaSimpleType UserIdSchemas()
        {
            var UserIdTypeSchemaElement = new XmlSchemaSimpleType();
            UserIdTypeSchemaElement.Name = "UserIdType";
            UserIdTypeSchemaElement.Content = EnumRestrictions(UserIdType, UserIdTypeSchemaElement.QualifiedName);
            return UserIdTypeSchemaElement;
        }

        private static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            // DO NOTHING; JUST A STUB
        }

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