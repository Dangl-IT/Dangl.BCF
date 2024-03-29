using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Dangl.BCF
{
    public class BrandingCommentFactory
    {
        public const string BRANDING_URL = "https://www.dangl-it.com";

        public static string GetBrandingComment()
        {
            return $"Created with the Dangl.BCF library, Version {FileVersionProvider.NuGetVersion} at {DateTime.UtcNow:dd.MM.yyyy HH:mm}. Visit {BRANDING_URL} to find out more.";
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
