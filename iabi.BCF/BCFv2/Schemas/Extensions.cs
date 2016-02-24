using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace iabi.BCF.BCFv2.Schemas
{
    public class Extensions_XSD
    {
        private List<string> _TopicType;

        public List<string> TopicType
        {
            get
            {
                return _TopicType ?? (_TopicType = new List<string>());
            }
            internal set { _TopicType = value; }
        }

        private List<string> _TopicStatus;

        public List<string> TopicStatus
        {
            get
            {
                return _TopicStatus ?? (_TopicStatus = new List<string>());
            }
            internal set { _TopicStatus = value; }
        }

        private List<string> _TopicLabel;

        public List<string> TopicLabel
        {
            get
            {
                return _TopicLabel ?? (_TopicLabel = new List<string>());
            }
            internal set { _TopicLabel = value; }
        }

        private List<string> _SnippetType;

        public List<string> SnippetType
        {
            get
            {
                return _SnippetType ?? (_SnippetType = new List<string>());
            }
            internal set { _SnippetType = value; }
        }

        private List<string> _Priority;

        public List<string> Priority
        {
            get
            {
                return _Priority ?? (_Priority = new List<string>());
            }
            internal set { _Priority = value; }
        }

        private List<string> _UserIdType;

        public List<string> UserIdType
        {
            get
            {
                return _UserIdType ?? (_UserIdType = new List<string>());
            }
            internal set { _UserIdType = value; }
        }

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

        public string WriteExtension()
        {
            try
            {
                XmlSchema ExtensionsSchemaToWrite = new XmlSchema();
                // Add the redfines tag
                XmlSchemaRedefine SchemaRedefine = new XmlSchemaRedefine();
                SchemaRedefine.SchemaLocation = "markup.xsd";
                ExtensionsSchemaToWrite.Includes.Add(SchemaRedefine);
                // Add the items
                SchemaRedefine.Items.Add(TopicTypeSchemas());
                SchemaRedefine.Items.Add(TopicStatusSchemas());
                SchemaRedefine.Items.Add(TopicLabelSchemas());
                SchemaRedefine.Items.Add(SnippetTypeSchemas());
                SchemaRedefine.Items.Add(PrioritySchemas());
                SchemaRedefine.Items.Add(UserIdSchemas());
                using (MemoryStream CurrentMemoryStream = new MemoryStream())
                {
                    ExtensionsSchemaToWrite.Write(CurrentMemoryStream);

                    using (StreamReader reader = new StreamReader(CurrentMemoryStream))
                    {
                        CurrentMemoryStream.Position = 0;
                        string SchemaString = reader.ReadToEnd();
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
            List<string> ListToReturn = new List<string>();
            foreach (XmlSchemaEnumerationFacet CurrentEnum in GivenSchemaType.Facets)
            {
                ListToReturn.Add(CurrentEnum.Value);
            }
            return ListToReturn;
        }

        private XmlSchemaSimpleTypeRestriction EnumRestrictions(List<string> Values, XmlQualifiedName Name)
        {
            XmlSchemaSimpleTypeRestriction RestrictionsRoReturn = new XmlSchemaSimpleTypeRestriction();
            RestrictionsRoReturn.BaseTypeName = Name;
            if (Values.Count > 0)
            {
                foreach (string GivenValue in Values)
                {
                    XmlSchemaEnumerationFacet CurrentEnumItem = new XmlSchemaEnumerationFacet();
                    CurrentEnumItem.Value = GivenValue;
                    RestrictionsRoReturn.Facets.Add(CurrentEnumItem);
                }
            }
            return RestrictionsRoReturn;
        }

        private XmlSchemaSimpleType TopicTypeSchemas()
        {
            XmlSchemaSimpleType TopicTypesSchemaElement = new XmlSchemaSimpleType();
            TopicTypesSchemaElement.Name = "TopicType";
            TopicTypesSchemaElement.Content = EnumRestrictions(TopicType, TopicTypesSchemaElement.QualifiedName);
            return TopicTypesSchemaElement;
        }

        private XmlSchemaSimpleType TopicStatusSchemas()
        {
            XmlSchemaSimpleType TopicStatusSchemaElement = new XmlSchemaSimpleType();
            TopicStatusSchemaElement.Name = "TopicStatus";
            TopicStatusSchemaElement.Content = EnumRestrictions(TopicStatus, TopicStatusSchemaElement.QualifiedName);
            return TopicStatusSchemaElement;
        }

        private XmlSchemaSimpleType TopicLabelSchemas()
        {
            XmlSchemaSimpleType TopicLabelSchemaElement = new XmlSchemaSimpleType();
            TopicLabelSchemaElement.Name = "TopicLabel";
            TopicLabelSchemaElement.Content = EnumRestrictions(TopicLabel, TopicLabelSchemaElement.QualifiedName);
            return TopicLabelSchemaElement;
        }

        private XmlSchemaSimpleType SnippetTypeSchemas()
        {
            XmlSchemaSimpleType SnippetTypeSchemaElement = new XmlSchemaSimpleType();
            SnippetTypeSchemaElement.Name = "SnippetType";
            SnippetTypeSchemaElement.Content = EnumRestrictions(SnippetType, SnippetTypeSchemaElement.QualifiedName);
            return SnippetTypeSchemaElement;
        }

        private XmlSchemaSimpleType PrioritySchemas()
        {
            XmlSchemaSimpleType PrioritySchemaElement = new XmlSchemaSimpleType();
            PrioritySchemaElement.Name = "Priority";
            PrioritySchemaElement.Content = EnumRestrictions(Priority, PrioritySchemaElement.QualifiedName);
            return PrioritySchemaElement;
        }

        private XmlSchemaSimpleType UserIdSchemas()
        {
            XmlSchemaSimpleType UserIdTypeSchemaElement = new XmlSchemaSimpleType();
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