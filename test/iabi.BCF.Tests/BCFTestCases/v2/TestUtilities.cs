using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.v2
{
    public static class TestUtilities
    {
        public static BimSnippet GetBimSnippetFromXml(XElement snippetXml)
        {
            var snippet = new BimSnippet();
            snippet.isExternal = snippetXml.Attribute("isExternal") == null ? false : bool.Parse(snippetXml.Attribute("isExternal").Value);
            snippet.Reference = snippetXml.Descendants("Reference").FirstOrDefault().Value;
            snippet.ReferenceSchema = snippetXml.Descendants("ReferenceSchema").FirstOrDefault().Value;
            snippet.SnippetType = snippetXml.Attribute("SnippetType").Value;
            return snippet;
        }

        public static Comment GetCommentFromXml(XElement commentXml)
        {
            var comment = new Comment();
            comment.Author = commentXml.Descendants("Author").FirstOrDefault().Value;
            comment.Comment1 = commentXml.Descendants("Comment").FirstOrDefault().Value;
            comment.Date = (DateTime) commentXml.Descendants("Date").FirstOrDefault();
            comment.Guid = commentXml.Attribute("Guid").Value;
            comment.ModifiedAuthor = commentXml.Descendants("ModifiedAuthor").Any() ? commentXml.Descendants("ModifiedAuthor").FirstOrDefault().Value : null;
            if (commentXml.Descendants("ModifiedDate").Any())
            {
                comment.ModifiedDate = (DateTime) commentXml.Descendants("ModifiedDate").FirstOrDefault();
            }
            comment.ReplyToComment = commentXml.Descendants("ReplyToComment").Any() ? new CommentReplyToComment {Guid = commentXml.Descendants("ReplyToComment").FirstOrDefault().Attribute("Guid").Value} : null;
            comment.Status = commentXml.Descendants("Status").FirstOrDefault().Value;
            comment.VerbalStatus = commentXml.Descendants("VerbalStatus").FirstOrDefault().Value;
            comment.Viewpoint = commentXml.Descendants("Viewpoint").Any() ? new CommentViewpoint {Guid = commentXml.Descendants("Viewpoint").FirstOrDefault().Attribute("Guid").Value} : null;

            return comment;
        }

        public static PerspectiveCamera GetPerspectiveCameraObjectFromXml(XElement cameraXml)
        {
            return new PerspectiveCamera
            {
                FieldOfView = double.Parse(cameraXml.DescendantNodes().OfType<XElement>().FirstOrDefault(curr => curr.Name.LocalName == "FieldOfView").Value, CultureInfo.InvariantCulture),
                CameraDirection = new Direction
                {
                    X = double.Parse(cameraXml.Descendants("CameraDirection").FirstOrDefault().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    Y = double.Parse(cameraXml.Descendants("CameraDirection").FirstOrDefault().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    Z = double.Parse(cameraXml.Descendants("CameraDirection").FirstOrDefault().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                CameraUpVector = new Direction
                {
                    X = double.Parse(cameraXml.Descendants("CameraUpVector").FirstOrDefault().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    Y = double.Parse(cameraXml.Descendants("CameraUpVector").FirstOrDefault().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    Z = double.Parse(cameraXml.Descendants("CameraUpVector").FirstOrDefault().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                CameraViewPoint = new Point
                {
                    X = double.Parse(cameraXml.Descendants("CameraViewPoint").FirstOrDefault().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    Y = double.Parse(cameraXml.Descendants("CameraViewPoint").FirstOrDefault().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    Z = double.Parse(cameraXml.Descendants("CameraViewPoint").FirstOrDefault().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                }
            };
        }

        public static byte[] GetBinaryData(this byte[] zipArchive, string absolutePath)
        {
            using (var archive = new ZipArchive(new MemoryStream(zipArchive), ZipArchiveMode.Read))
            {
                var entry = archive.Entries.FirstOrDefault(curr => curr.FullName == absolutePath);
                var memStream = new MemoryStream();
                entry.Open().CopyTo(memStream);
                return memStream.ToArray();
            }
        }
    }
}