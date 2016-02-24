using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace iabi.BCF.Tests
{
    public static class XmlUtilities
    {
        public static bool ElementNameInXml(Stream XmlStream, string ElementName)
        {
            using (StreamReader Rdr = new StreamReader(XmlStream))
            {
                return ElementNameInXml(Rdr.ReadToEnd(), ElementName);
            }
        }

        public static bool ElementNameInXml(string XmlString, string ElementName)
        {
            var Xml = XElement.Parse(XmlString);
            return ElementNameInXml(Xml, ElementName);
        }

        public static bool ElementNameInXml(XElement Xml, string ElementName)
        {
            var Elements = Xml.DescendantNodesAndSelf();
            return Elements.OfType<XElement>().Any(Curr => Curr.Name.LocalName == ElementName);
        }

        public static XElement GetElementFromZipFile(ZipArchive Archive, string FullFileName)
        {
            var Entry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == FullFileName);
            using (StreamReader Rdr = new StreamReader(Entry.Open()))
            {
                return XElement.Parse(Rdr.ReadToEnd());
            }
        }
    }
}
