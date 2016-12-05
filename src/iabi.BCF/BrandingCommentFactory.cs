using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace iabi.BCF
{
    public class BrandingCommentFactory
    {
        public const string IABI_BRANDING_URL = "http://iabi.eu";
        public static string GetBrandingComment()
        {
            var bcfToolsVersion = typeof(BrandingCommentFactory).GetTypeInfo().Assembly.GetName().Version;
            return $"Created with the iabi.BCF library, V{bcfToolsVersion.Major}.{bcfToolsVersion.Minor}.{bcfToolsVersion.Revision} at {DateTime.Now:dd.MM.yyyy HH:mm}. Visit {IABI_BRANDING_URL} to find out more.";
        }

        public static string AppendBrandingCommentToTopLevelXml(string xmlInput)
        {
            var inputDocument = XDocument.Parse(xmlInput);
            inputDocument.AddFirst(new XComment(GetBrandingComment()));
            using (var memStream = new MemoryStream())
            {
                using (var streamReader = new StreamReader(memStream))
                {
                    inputDocument.Save(memStream);
                    memStream.Position = 0;
                    var xmlString = streamReader.ReadToEnd();
                    return xmlString;
                }
            }
        }
    }
}
