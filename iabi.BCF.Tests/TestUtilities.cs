using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace iabi.BCF.Test
{




    public static class TestUtilities
    {
        //public static void CompareBCFv2Container(BCFv2Container Expected, BCFv2Container Actual)
        //{
        //    var Config = new KellermanSoftware.CompareNetObjects.ComparisonConfig();
        //    Config.TreatStringEmptyAndNullTheSame = true;
        //    Config.MembersToIgnore.Add("Index");
        //    Config.MaxMillisecondsDateDifference = 2000;
        //    var Comparator = new KellermanSoftware.CompareNetObjects.CompareLogic(Config);

        //    var Comparison = Comparator.Compare(Expected, Actual);

        //    Assert.IsTrue(Comparison.AreEqual, Comparison.DifferencesString);
        //}

        public static BimSnippet GetBimSnippetFromXml(XElement SnippetXml)
        {
            var Snippet = new BimSnippet();

            Snippet.isExternal = SnippetXml.Attribute("isExternal") == null ? false : bool.Parse(SnippetXml.Attribute("isExternal").Value);
            Snippet.Reference = SnippetXml.Descendants("Reference").FirstOrDefault().Value;
            Snippet.ReferenceSchema = SnippetXml.Descendants("ReferenceSchema").FirstOrDefault().Value;
            Snippet.SnippetType = SnippetXml.Attribute("SnippetType").Value;

            return Snippet;
        }

        public static Comment GetCommentFromXml(XElement CommentXml)
        {
            var Comment = new Comment();

            Comment.Author = CommentXml.Descendants("Author").FirstOrDefault().Value;
            Comment.Comment1 = CommentXml.Descendants("Comment").FirstOrDefault().Value;
            Comment.Date = (DateTime)(CommentXml.Descendants("Date").FirstOrDefault() as XElement);
            Comment.Guid = CommentXml.Attribute("Guid").Value;
            Comment.ModifiedAuthor = CommentXml.Descendants("ModifiedAuthor").Any() ? CommentXml.Descendants("ModifiedAuthor").FirstOrDefault().Value : null;
            if (CommentXml.Descendants("ModifiedDate").Any())
            {
                Comment.ModifiedDate = (DateTime)(CommentXml.Descendants("ModifiedDate").FirstOrDefault() as XElement);
            }
            Comment.ReplyToComment = CommentXml.Descendants("ReplyToComment").Any() ? new CommentReplyToComment { Guid = CommentXml.Descendants("ReplyToComment").FirstOrDefault().Attribute("Guid").Value } : null;
            Comment.Status = CommentXml.Descendants("Status").FirstOrDefault().Value;
            Comment.VerbalStatus = CommentXml.Descendants("VerbalStatus").FirstOrDefault().Value;
            Comment.Viewpoint = CommentXml.Descendants("Viewpoint").Any() ? new CommentViewpoint { Guid = CommentXml.Descendants("Viewpoint").FirstOrDefault().Attribute("Guid").Value } : null;

            return Comment;
        }

        public static PerspectiveCamera GetPerspectiveCameraObjectFromXml(XElement CameraXml)
        {
            return new PerspectiveCamera
            {
                FieldOfView = double.Parse(CameraXml.DescendantNodes().OfType<XElement>().FirstOrDefault(Curr => Curr.Name.LocalName == "FieldOfView").Value, CultureInfo.InvariantCulture),
                CameraDirection = new Direction
                {
                    X = double.Parse(CameraXml.Descendants("CameraDirection").FirstOrDefault().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    Y = double.Parse(CameraXml.Descendants("CameraDirection").FirstOrDefault().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    Z = double.Parse(CameraXml.Descendants("CameraDirection").FirstOrDefault().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                CameraUpVector = new Direction
                {
                    X = double.Parse(CameraXml.Descendants("CameraUpVector").FirstOrDefault().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    Y = double.Parse(CameraXml.Descendants("CameraUpVector").FirstOrDefault().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    Z = double.Parse(CameraXml.Descendants("CameraUpVector").FirstOrDefault().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                },
                CameraViewPoint = new iabi.BCF.BCFv2.Schemas.Point
                {
                    X = double.Parse(CameraXml.Descendants("CameraViewPoint").FirstOrDefault().Descendants("X").First().Value, CultureInfo.InvariantCulture),
                    Y = double.Parse(CameraXml.Descendants("CameraViewPoint").FirstOrDefault().Descendants("Y").First().Value, CultureInfo.InvariantCulture),
                    Z = double.Parse(CameraXml.Descendants("CameraViewPoint").FirstOrDefault().Descendants("Z").First().Value, CultureInfo.InvariantCulture)
                }
            };
        }

        public static byte[] GetBinaryData(this byte[] ZipArchive, string AbsolutePath)
        {
            using (var Archive = new ZipArchive(new MemoryStream(ZipArchive), ZipArchiveMode.Read))
            {
                var Entry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == AbsolutePath);
                var MemStream = new MemoryStream();
                Entry.Open().CopyTo(MemStream);
                return MemStream.ToArray();
            }
        }
    }
}