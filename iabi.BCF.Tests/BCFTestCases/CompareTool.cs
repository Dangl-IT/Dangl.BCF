using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iabi.BCF.Test.BCFTestCases
{
    public static class CompareTool
    {
        public static void CompareFiles(byte[] FileToImport, byte[] ReadAndWrittenFile)
        {
            // Check that archive file contents are present
            ArchiveFilesCompareTool.CompareZipArchiveFileEntries(FileToImport, ReadAndWrittenFile);
            var ExpectedFile = BCFv2Container.ReadStream(new MemoryStream(FileToImport));
            var ActualFile = BCFv2Container.ReadStream(new MemoryStream(ReadAndWrittenFile));
            CompareContainers(ExpectedFile, ActualFile, new ZipArchive(new MemoryStream(FileToImport)), new ZipArchive(new MemoryStream(ReadAndWrittenFile)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ExpectedContainer"></param>
        /// <param name="ActualContainer"></param>
        /// <param name="ExpectedArchive"></param>
        /// <param name="ActualArchive"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareContainers(BCFv2Container ExpectedContainer, BCFv2Container ActualContainer, ZipArchive ExpectedArchive = null, ZipArchive ActualArchive = null, bool OriginatesFromAPIConversion = false)
        {
            CompareProjectAndVersion(ExpectedContainer, ActualContainer);
            CompareFileAttachments(ExpectedContainer, ActualContainer);
            CompareProjectExtensions(ExpectedContainer, ActualContainer);

            if (ExpectedArchive == null && ActualArchive == null)
            {
                using (var MemStream_01 = new MemoryStream())
                {
                    ExpectedContainer.WriteStream(MemStream_01);
                    ExpectedArchive = new ZipArchive(MemStream_01);
                    using (var MemStream_02 = new MemoryStream())
                    {
                        ExpectedContainer.WriteStream(MemStream_02);
                        ActualArchive = new ZipArchive(MemStream_02);
                        TopicsCompareTool.CompareAllTopics(ExpectedContainer, ActualContainer, ExpectedArchive, ActualArchive, OriginatesFromAPIConversion);
                        return;
                    }
                }
            }

            TopicsCompareTool.CompareAllTopics(ExpectedContainer, ActualContainer, ExpectedArchive, ActualArchive, OriginatesFromAPIConversion);
        }

        public static void CompareProjectExtensions(BCFv2Container ExpectedContainer, BCFv2Container ActualContainer)
        {
            if (TestCompareUtilities.BothNotNull(ExpectedContainer.ProjectExtensions, ActualContainer.ProjectExtensions, "ProjectExtensions"))
            {
                CompareStringList(ExpectedContainer.ProjectExtensions.SnippetType, ActualContainer.ProjectExtensions.SnippetType, "SnippetType");
                CompareStringList(ExpectedContainer.ProjectExtensions.Priority, ActualContainer.ProjectExtensions.Priority, "Priority");
                CompareStringList(ExpectedContainer.ProjectExtensions.TopicStatus, ActualContainer.ProjectExtensions.TopicStatus, "TopicStatus");
                CompareStringList(ExpectedContainer.ProjectExtensions.TopicType, ActualContainer.ProjectExtensions.TopicType, "TopicType");
                CompareStringList(ExpectedContainer.ProjectExtensions.UserIdType, ActualContainer.ProjectExtensions.UserIdType, "UserIdType");
                CompareStringList(ExpectedContainer.ProjectExtensions.TopicLabel, ActualContainer.ProjectExtensions.TopicLabel, "TopicLabel");
            }
        }

        public static void CompareStringList(List<string> Expected, List<string> Actual, string ListName)
        {
            // Check that all expected values are present
            foreach (var CurrentValue in Expected)
            {
                Assert.IsTrue(Actual.Contains(CurrentValue), "Value: " + CurrentValue + " is missing in actual list of name: " + ListName);
            }
            // Check that no additional values are present
            foreach (var CurrentValue in Actual)
            {
                Assert.IsTrue(Expected.Contains(CurrentValue), "Value: " + CurrentValue + " is missing in expected list of name: " + ListName + " but present in actual list.");
            }
        }

        public static void CompareFileAttachments(BCFv2Container ExpectedContainer, BCFv2Container ActualContainer)
        {
            // Check that all files from the expected container are present
            foreach (var CurrentFile in ExpectedContainer.FileAttachments)
            {
                Assert.IsTrue(ActualContainer.FileAttachments.ContainsKey(CurrentFile.Key), "Missing file: " + CurrentFile.Key + " in actual container.");
                Assert.IsTrue(ExpectedContainer.FileAttachments[CurrentFile.Key].SequenceEqual(ActualContainer.FileAttachments[CurrentFile.Key]), "File: " + CurrentFile.Key + " binary different in actual container");
            }

            // Check that no file attachments were added
            foreach (var CurrentFile in ActualContainer.FileAttachments)
            {
                Assert.IsTrue(ExpectedContainer.FileAttachments.ContainsKey(CurrentFile.Key));
            }
        }

        /// <summary>
        /// Will compare the project and version descriptions within the file
        /// </summary>
        /// <param name="ExpectedContainer"></param>
        /// <param name="ActualContainer"></param>
        public static void CompareProjectAndVersion(BCFv2Container ExpectedContainer, BCFv2Container ActualContainer)
        {
            // Compare project
            if (TestCompareUtilities.BothNotNull(ExpectedContainer.BCFProject, ActualContainer.BCFProject, "BCFProject"))
            {
                if (TestCompareUtilities.BothNotNull(ExpectedContainer.BCFProject.Project, ActualContainer.BCFProject.Project, "BCFProject.Project"))
                {
                    Assert.AreEqual(ExpectedContainer.BCFProject.Project.Name, ActualContainer.BCFProject.Project.Name, "Project Name doesnt match");
                    Assert.AreEqual(ExpectedContainer.BCFProject.Project.ProjectId, ActualContainer.BCFProject.Project.ProjectId, "Project ProjectId doesnt match");
                }
                Assert.AreEqual(ExpectedContainer.BCFProject.ExtensionSchema, ActualContainer.BCFProject.ExtensionSchema);
            }

            // Compare version
            if (TestCompareUtilities.BothNotNull(ExpectedContainer.BCFVersionInfo, ActualContainer.BCFVersionInfo, "BCFVersionInfo"))
            {
                if (ExpectedContainer.BCFVersionInfo.VersionId.Contains("2.0"))
                {
                    Assert.IsTrue(ActualContainer.BCFVersionInfo.VersionId.Contains("2.0"));
                }
                else
                {
                    Assert.Fail("Unrecognized VersionId");
                }
            }
        }
    }

    public static class TestCompareUtilities
    {
        /// <summary>
        /// Raises <see cref="Assert.Fail"/> if one is null and the other one is not
        /// </summary>
        /// <param name="ExpectedObject"></param>
        /// <param name="ActualObject"></param>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        public static bool BothNotNull(object ExpectedObject, object ActualObject, string ParameterName)
        {
            if (ExpectedObject == null && ActualObject != null)
            {
                Assert.Fail("Parameter: " + ParameterName + "; Expected is null but actual is present.");
            }
            else if (ExpectedObject != null && ActualObject == null)
            {
                Assert.Fail("Parameter: " + ParameterName + "; Actual is null but Expected is present.");
            }
            return ExpectedObject != null;
        }
    }

    public static class ArchiveFilesCompareTool
    {
        public static void CompareZipArchiveFileEntries(byte[] FileToImport, byte[] ReadAndWrittenFile)
        {
            var ExpectedZipArchive = new ZipArchive(new MemoryStream(FileToImport));
            var ActualZipArchive = new ZipArchive(new MemoryStream(ReadAndWrittenFile));

            // Check that all expected files are present
            foreach (var CurrentExpectedFile in ExpectedZipArchive.Entries)
            {
                var FilePresent = ActualZipArchive.Entries.Any(Curr => Curr.FullName == CurrentExpectedFile.FullName);
                if (!FilePresent && (
                    CurrentExpectedFile.FullName.ToUpperInvariant().EndsWith(".png".ToUpperInvariant())
                    || CurrentExpectedFile.FullName.ToUpperInvariant().EndsWith(".jpg".ToUpperInvariant())
                    || CurrentExpectedFile.FullName.ToUpperInvariant().EndsWith(".jpeg".ToUpperInvariant())))
                {
                    // It may be a snapshot or a bitmap that was just renamed (for example, from snapshot.png to {Guid}.png
                    // Will search for a file in the same directory with the same size
                    FilePresent = FileIsPresentInSameFolderWithDifferentName(ExpectedZipArchive, ActualZipArchive, CurrentExpectedFile.FullName);
                }
                if (!FilePresent && CurrentExpectedFile.FullName.ToUpperInvariant().EndsWith(".bcfv".ToUpperInvariant()))
                {
                    // Viewpoints may be renamed
                    var Folder = CurrentExpectedFile.FullName.Substring(0, CurrentExpectedFile.FullName.Length - CurrentExpectedFile.FullName.Split('/').Last().Length);
                    var ExpectedMarkupXml = XmlUtilities.GetElementFromZipFile(ExpectedZipArchive, Folder + "markup.bcf");
                    var ViewpointGuid = ExpectedMarkupXml.Descendants("Viewpoints").Where(Curr => Curr.Descendants("Viewpoint").FirstOrDefault().Value == CurrentExpectedFile.FullName.Split('/').Last()).FirstOrDefault().Attributes().FirstOrDefault(Curr => Curr.Name == "Guid").Value;

                    // Get viewpoint reference for this viewpoint in the created archive
                    var ActualMarkupXml = XmlUtilities.GetElementFromZipFile(ActualZipArchive, Folder + "markup.bcf");
                    var ActualViewpointReference = ActualMarkupXml.Descendants("Viewpoints").Where(Curr => Curr.Attributes().Where(Attr => Attr.Name == "Guid" && Attr.Value == ViewpointGuid).Any()).FirstOrDefault().Descendants("Viewpoint").FirstOrDefault().Value;

                    FilePresent = ActualZipArchive.Entries.Any(Curr => Curr.FullName == (Folder + ActualViewpointReference));
                }
                if (!FilePresent && CurrentExpectedFile.FullName.ToUpperInvariant().EndsWith("project.bcfp".ToUpperInvariant()))
                {
                    // Originating file has maybe an empty project.bcfp, check this and do not enforce file availability then.
                    var ExpectedProjectXml = XmlUtilities.GetElementFromZipFile(ExpectedZipArchive, CurrentExpectedFile.FullName);

                    var ProjectShouldBeTreatedAsEmpty =
                        (!ExpectedProjectXml.Descendants("ExtensionSchema").Any() || ExpectedProjectXml.Descendants("ExtensionSchema").All(Curr => string.IsNullOrWhiteSpace(Curr.Value))) // ExtensionSchema not present or empty
                        && (!ExpectedProjectXml.Descendants("Project").Any() || ExpectedProjectXml.Descendants("Project").All(Curr => Curr.Attribute("ProjectId") == null || string.IsNullOrWhiteSpace(Curr.Attribute("ProjectId").Value))) // Project Id not present or empty
                        && ((!ExpectedProjectXml.Descendants("Name").Where(Curr => Curr.Parent.Name.LocalName == "Project").Any() || ExpectedProjectXml.Descendants("Name").Where(Curr => Curr.Parent.Name.LocalName == "Project").All(Curr => string.IsNullOrWhiteSpace(Curr.Value)))); // Project Name not present or empty
                    if (ProjectShouldBeTreatedAsEmpty)
                    {
                        FilePresent = true;
                    }
                }

                if (!FilePresent)
                {
                    FilePresent = CurrentExpectedFile.Length == 0; // Empty entry, then dont look for it
                }

                if (!FilePresent)
                {
                    Assert.Fail("File: \"" + CurrentExpectedFile.FullName + "\" not present in created archive.");
                }
            }
        }

        public static bool FileIsPresentInSameFolderWithDifferentName(ZipArchive ExpectedArchive, ZipArchive ActualArchive, string ExpectedEntryFullName)
        {
            var ExpectedEntry = ExpectedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == ExpectedEntryFullName);
            var Folder = ExpectedEntryFullName.Substring(0, ExpectedEntryFullName.Length - ExpectedEntryFullName.Split('/').Last().Length);

            var SameFileInFolder = ActualArchive.Entries
                .Where(Curr => Curr.FullName.StartsWith(Folder))
                .Where(Curr => !Curr.FullName.Substring(Folder.Length).Contains('/'))
                .Where(Curr => Curr.Length == ExpectedEntry.Length)
                .Any();

            return SameFileInFolder;
        }
    }

    public static class TopicsCompareTool
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="ExpectedContainer"></param>
        /// <param name="ActualContainer"></param>
        /// <param name="ExpectedArchive"></param>
        /// <param name="ActualArchive"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareAllTopics(BCFv2Container ExpectedContainer, BCFv2Container ActualContainer, ZipArchive ExpectedArchive, ZipArchive ActualArchive, bool OriginatesFromAPIConversion)
        {
            foreach (var ExpectedTopic in ExpectedContainer.Topics)
            {
                // Make sure topic is present only one
                Assert.AreEqual(1, ActualContainer.Topics.Where(Curr => Curr.Markup.Topic.Guid == ExpectedTopic.Markup.Topic.Guid).Count());
                var ActualTopic = ActualContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == ExpectedTopic.Markup.Topic.Guid);
                CompareSingleTopic(ExpectedTopic, ActualTopic, ExpectedArchive, ActualArchive, OriginatesFromAPIConversion);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ExpectedTopic"></param>
        /// <param name="ActualTopic"></param>
        /// <param name="ExpectedArchive"></param>
        /// <param name="ActualArchive"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareSingleTopic(BCFTopic ExpectedTopic, BCFTopic ActualTopic, ZipArchive ExpectedArchive, ZipArchive ActualArchive, bool OriginatesFromAPIConversion)
        {
            // Compare Markup
            CompareMarkup(ExpectedTopic.Markup, ActualTopic.Markup, ExpectedArchive, ActualArchive);

            // Compare SnippetData
            if (ExpectedTopic.SnippetData != null)
            {
                Assert.IsTrue(ExpectedTopic.SnippetData.SequenceEqual(ActualTopic.SnippetData));
            }
            else
            {
                Assert.IsNull(ActualTopic.SnippetData);
            }

            // Compare Viewpoints
            CompareViewpoints(ExpectedTopic.Viewpoints, ActualTopic.Viewpoints, OriginatesFromAPIConversion);

            // Compare ViewpointBitmaps
            if (!OriginatesFromAPIConversion)
            {
                CompareViewpointBitmaps(ExpectedTopic.ViewpointBitmaps, ActualTopic.ViewpointBitmaps);
            }

            // Compare ViewpointSnapshots
            CompareViewpointSnapshots(ExpectedTopic.ViewpointSnapshots, ActualTopic.ViewpointSnapshots);
        }

        public static void CompareMarkup(Markup ExpectedMarkup, Markup ActualMarkup, ZipArchive ExpectedArchive, ZipArchive ActualArchive)
        {
            // Compare Header Section
            if (TestCompareUtilities.BothNotNull(ExpectedMarkup.Header, ActualMarkup.Header, "Markup.Header"))
            {
                foreach (var CurrentHeaderEntry in ExpectedMarkup.Header)
                {
                    var FoundExactlyOneMatchignEntryInActual = ActualMarkup.Header
                        .Where(Curr => Curr.Date == CurrentHeaderEntry.Date)
                        .Where(Curr => Curr.Filename == CurrentHeaderEntry.Filename)
                        .Where(Curr => Curr.IfcProject == CurrentHeaderEntry.IfcProject)
                        .Where(Curr => Curr.IfcSpatialStructureElement == CurrentHeaderEntry.IfcSpatialStructureElement)
                        .Where(Curr => Curr.isExternal == CurrentHeaderEntry.isExternal)
                        .Where(Curr => Curr.Reference == CurrentHeaderEntry.Reference)
                        .Count() == 1;

                    Assert.IsTrue(FoundExactlyOneMatchignEntryInActual, "Found not matching header entry in actual file.");
                }

                // Check that the both sections contain the same number of entries
                Assert.AreEqual(ExpectedMarkup.Header.Count, ActualMarkup.Header.Count);
            }

            CompareComments(ExpectedMarkup.Comment, ActualMarkup.Comment);

            if (TestCompareUtilities.BothNotNull(ExpectedMarkup.Viewpoints, ActualMarkup.Viewpoints, "Markup.Viewpoints"))
            {
                CompareViewpoints(ExpectedMarkup.Viewpoints, ActualMarkup.Viewpoints, ExpectedArchive, ActualArchive, ExpectedMarkup.Topic.Guid);
            }

            if (TestCompareUtilities.BothNotNull(ExpectedMarkup.Topic, ActualMarkup.Topic, "Markup.Topic"))
            {
                CompareTopic(ExpectedMarkup.Topic, ActualMarkup.Topic);
            }
        }

        public static void CompareViewpoints(List<ViewPoint> ExpectedViewpoints, List<ViewPoint> ActualViewpoints, ZipArchive ExpectedArchive, ZipArchive ActualArchive, string TopicGuid)
        {
            // Count Matches
            Assert.AreEqual(ExpectedViewpoints.Count, ActualViewpoints.Count);

            foreach (var ExpectedViewpoint in ExpectedViewpoints)
            {
                var ActualViewpoint = ActualViewpoints.FirstOrDefault(Curr => Curr.Guid == ExpectedViewpoint.Guid);

                Assert.AreEqual(ExpectedViewpoint.Guid, ActualViewpoint.Guid);
                if (ExpectedViewpoint.Snapshot != ActualViewpoint.Snapshot)
                {
                    // This means the file reference to the snapshot is different; need to check it then in binary format
                    var ExpectedViewpointSnapshotStream = new MemoryStream();
                    ExpectedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == TopicGuid + "/" + ExpectedViewpoint.Snapshot).Open().CopyTo(ExpectedViewpointSnapshotStream);
                    var ExpectedViewpointSnapshotBinary = ExpectedViewpointSnapshotStream.ToArray();

                    Assert.IsFalse(string.IsNullOrWhiteSpace(ActualViewpoint.Snapshot), "Viewpoint has no reference");
                    var ActualViewpointEntry = ActualArchive.Entries.FirstOrDefault(Curr => Curr.FullName == TopicGuid + "/" + ActualViewpoint.Snapshot);
                    Assert.IsNotNull(ActualViewpointEntry, "Viewpoint entry not found");
                    var ActualViewpointSnapshotStream = new MemoryStream();
                    ActualViewpointEntry.Open().CopyTo(ActualViewpointSnapshotStream);
                    var ActualViewpointSnapshotBinary = ActualViewpointSnapshotStream.ToArray();

                    Assert.IsTrue(ExpectedViewpointSnapshotBinary.SequenceEqual(ActualViewpointSnapshotBinary));
                }
                Assert.AreEqual(ExpectedViewpoint.Viewpoint, ActualViewpoint.Viewpoint);
            }
        }

        public static void CompareBimSnippet(BimSnippet Expected, BimSnippet Actual)
        {
            Assert.AreEqual(Expected.isExternal, Actual.isExternal, "isExternal doesnt match");
            Assert.AreEqual(Expected.Reference, Actual.Reference, "reference doesnt match");
            Assert.AreEqual(Expected.ReferenceSchema, Actual.ReferenceSchema, "schema doesnt match");
            Assert.AreEqual(Expected.SnippetType, Actual.SnippetType, "type doesnt match");
        }

        public static void CompareTopic(Topic ExpectedTopic, Topic ActualTopic)
        {
            Assert.AreEqual(ExpectedTopic.AssignedTo, ActualTopic.AssignedTo);

            // Compare Snippet
            if (TestCompareUtilities.BothNotNull(ExpectedTopic.BimSnippet, ActualTopic.BimSnippet, "Markup.BimSnippet"))
            {
                CompareBimSnippet(ExpectedTopic.BimSnippet, ActualTopic.BimSnippet);
            }

            // Compare document references
            if (TestCompareUtilities.BothNotNull(ExpectedTopic.DocumentReferences, ActualTopic.DocumentReferences, "Markup.DocumentReferences"))
            {
                Assert.AreEqual(ExpectedTopic.DocumentReferences.Count, ActualTopic.DocumentReferences.Count);

                foreach (var ExpectedDocumentReference in ExpectedTopic.DocumentReferences)
                {
                    // Find the matching document reference in the actual topic
                    var ActualDocumentReference = ActualTopic.DocumentReferences
                        .Where(Curr => Curr.Description == ExpectedDocumentReference.Description)
                        .Where(Curr => Curr.Guid == ExpectedDocumentReference.Guid)
                        .Where(Curr => Curr.isExternal == ExpectedDocumentReference.isExternal)
                        .Where(Curr => Curr.ReferencedDocument == ExpectedDocumentReference.ReferencedDocument)
                        .FirstOrDefault();
                    Assert.IsNotNull(ActualDocumentReference, "Missing matching document reference");
                }
            }

            // Check labels
            if (TestCompareUtilities.BothNotNull(ExpectedTopic.Labels, ActualTopic.Labels, "Markup.Labels"))
            {
                Assert.AreEqual(ExpectedTopic.Labels.Count, ActualTopic.Labels.Count);
                foreach (var ExpectedLabel in ExpectedTopic.Labels)
                {
                    Assert.IsTrue(ActualTopic.Labels.Contains(ExpectedLabel));
                }
            }

            // Check related topics
            if (TestCompareUtilities.BothNotNull(ExpectedTopic.RelatedTopics, ActualTopic.RelatedTopics, "Markup.RelatedTopics"))
            {
                Assert.AreEqual(ExpectedTopic.RelatedTopics.Count, ActualTopic.RelatedTopics.Count);
                foreach (var ExpectedRelatedTopic in ExpectedTopic.RelatedTopics)
                {
                    var ActualRelatedTopic = ActualTopic.RelatedTopics
                        .Where(Curr => Curr.Guid == ExpectedRelatedTopic.Guid)
                        .FirstOrDefault();
                    Assert.IsNotNull(ActualRelatedTopic);
                }
            }

            Assert.AreEqual(ExpectedTopic.CreationAuthor, ActualTopic.CreationAuthor);
            Assert.AreEqual(0, (int)(ExpectedTopic.CreationDate - ActualTopic.CreationDate).TotalSeconds);
            Assert.AreEqual(ExpectedTopic.CreationDateSpecified, ActualTopic.CreationDateSpecified);

            if (!(string.IsNullOrWhiteSpace(ExpectedTopic.Description) && string.IsNullOrWhiteSpace(ActualTopic.Description)))
            {
                Assert.AreEqual(ExpectedTopic.Description, ActualTopic.Description);
            }
            Assert.AreEqual(ExpectedTopic.Guid, ActualTopic.Guid);

            if (ExpectedTopic.Index != ActualTopic.Index)
            {
                if (!(ExpectedTopic.Index != null && ExpectedTopic.Index.Trim() == "0" && string.IsNullOrWhiteSpace(ActualTopic.Index)
                    || ActualTopic.Index != null && ActualTopic.Index.Trim() == "0" && string.IsNullOrWhiteSpace(ExpectedTopic.Index)))
                {
                    Assert.Fail("Index does not match");
                }
            }

            Assert.AreEqual(ExpectedTopic.ModifiedAuthor, ActualTopic.ModifiedAuthor);
            Assert.AreEqual(ExpectedTopic.ModifiedDate, ActualTopic.ModifiedDate);
            Assert.AreEqual(ExpectedTopic.ModifiedDateSpecified, ActualTopic.ModifiedDateSpecified);
            Assert.AreEqual(ExpectedTopic.Priority, ActualTopic.Priority);
            Assert.AreEqual(ExpectedTopic.ReferenceLink, ActualTopic.ReferenceLink);
            Assert.AreEqual(ExpectedTopic.Title, ActualTopic.Title);
            Assert.AreEqual(ExpectedTopic.TopicStatus, ActualTopic.TopicStatus);
            Assert.AreEqual(ExpectedTopic.TopicType, ActualTopic.TopicType);
        }

        public static void CompareComments(List<Comment> ExpectedComments, List<Comment> ActualComments)
        {
            // Compare count
            Assert.AreEqual(ExpectedComments.Count, ActualComments.Count, "Comment count in markup differs.");

            foreach (var ExpectedComment in ExpectedComments)
            {
                // Get actual comment
                var ActualComment = ActualComments.FirstOrDefault(Curr => Curr.Guid == ExpectedComment.Guid);
                Assert.IsNotNull(ActualComment, "Did not find matching comment in actual file.");
                CompareSingleComment(ExpectedComment, ActualComment);
            }
        }

        public static void CompareSingleComment(Comment ExpectedComment, Comment ActualComment)
        {
            Assert.AreEqual(ExpectedComment.Author, ActualComment.Author, "No match in comment: Author");

            var CommentTextMatches = ExpectedComment.Comment1 == ActualComment.Comment1;
            if (!CommentTextMatches)
            {
                // It's possible that there is no actual comment text but the XML file does have the <Comment> element with an empty value
                CommentTextMatches = string.IsNullOrWhiteSpace(ExpectedComment.Comment1) && string.IsNullOrWhiteSpace(ActualComment.Comment1);
            }
            Assert.IsTrue(CommentTextMatches, "No match in comment: Comment1");

            Assert.AreEqual(ExpectedComment.Date, ActualComment.Date, "No match in comment: Date");
            Assert.AreEqual(ExpectedComment.Guid, ActualComment.Guid, "No match in comment: Guid");
            Assert.AreEqual(ExpectedComment.ModifiedAuthor, ActualComment.ModifiedAuthor, "No match in comment: ModifiedAuthor");
            Assert.AreEqual(ExpectedComment.ModifiedDate, ActualComment.ModifiedDate, "No match in comment: ModifiedDate");
            Assert.AreEqual(ExpectedComment.ModifiedDateSpecified, ActualComment.ModifiedDateSpecified, "No match in comment: ModifiedDateSpecified");

            if (ExpectedComment.ShouldSerializeReplyToComment())
            {
                Assert.IsTrue(ActualComment.ShouldSerializeReplyToComment());
                Assert.AreEqual(ExpectedComment.ReplyToComment.Guid, ActualComment.ReplyToComment.Guid, "No match in comment: ReplyToComment");
            }
            else
            {
                Assert.IsFalse(ActualComment.ShouldSerializeReplyToComment());
            }

            Assert.AreEqual(ExpectedComment.Status, ActualComment.Status, "No match in comment: Status");
            Assert.AreEqual(ExpectedComment.VerbalStatus, ActualComment.VerbalStatus, "No match in comment: VerbalStatus");

            if (ExpectedComment.ShouldSerializeViewpoint())
            {
                Assert.IsTrue(ActualComment.ShouldSerializeViewpoint(), "No match in comment: Viewpoint");
                Assert.AreEqual(ExpectedComment.Viewpoint.Guid, ActualComment.Viewpoint.Guid, "No match in comment: Viewpoint");
            }
            else
            {
                Assert.IsFalse(ActualComment.ShouldSerializeViewpoint(), "No match in comment: Viewpoint");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ExpectedViewpoints"></param>
        /// <param name="ActualViewpoints"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareViewpoints(IEnumerable<VisualizationInfo> ExpectedViewpoints, IEnumerable<VisualizationInfo> ActualViewpoints, bool OriginatesFromAPIConversion)
        {
            foreach (var ExpectedViewpoint in ExpectedViewpoints)
            {
                var ActualViewpoint = ActualViewpoints.FirstOrDefault(Curr => Curr.GUID == ExpectedViewpoint.GUID);
                Assert.IsNotNull(ActualViewpoint, "Found no matching viewpoint for expected viewpoint with guid " + ExpectedViewpoint.GUID);
                CompareSingleViewpoints(ExpectedViewpoint, ActualViewpoint, OriginatesFromAPIConversion);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ExpectedViewpoint"></param>
        /// <param name="ActualViewpoint"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareSingleViewpoints(VisualizationInfo ExpectedViewpoint, VisualizationInfo ActualViewpoint, bool OriginatesFromAPIConversion)
        {
            Assert.AreEqual(ExpectedViewpoint.GUID, ActualViewpoint.GUID, "Viewpoint Guids dont match");

            // Compare Bitmaps
            if (!OriginatesFromAPIConversion && TestCompareUtilities.BothNotNull(ExpectedViewpoint.Bitmaps, ActualViewpoint.Bitmaps, "Viewpoint.Bitmaps"))
            {
                Assert.AreEqual(ExpectedViewpoint.Bitmaps.Count, ActualViewpoint.Bitmaps.Count, "Bitmaps count doesnt match");
                foreach (var ExpectedBitmap in ExpectedViewpoint.Bitmaps)
                {
                    var ActualBitmap = ActualViewpoint.Bitmaps
                        .Where(Curr => Curr.Bitmap == ExpectedBitmap.Bitmap)
                        .Where(Curr => Curr.Height == ExpectedBitmap.Height)
                        .Where(Curr => Curr.Location.X == ExpectedBitmap.Location.X)
                        .Where(Curr => Curr.Location.Y == ExpectedBitmap.Location.Y)
                        .Where(Curr => Curr.Location.Z == ExpectedBitmap.Location.Z)
                        .Where(Curr => Curr.Normal.X == ExpectedBitmap.Normal.X)
                        .Where(Curr => Curr.Normal.Y == ExpectedBitmap.Normal.Y)
                        .Where(Curr => Curr.Normal.Z == ExpectedBitmap.Normal.Z)
                        .Where(Curr => Curr.Reference == ExpectedBitmap.Reference)
                        .Where(Curr => Curr.Up.X == ExpectedBitmap.Up.X)
                        .Where(Curr => Curr.Up.Y == ExpectedBitmap.Up.Y)
                        .Where(Curr => Curr.Up.Z == ExpectedBitmap.Up.Z)
                        .FirstOrDefault();
                    Assert.IsNotNull(ActualViewpoint, "Did not find a matching bitmap");
                }
            }

            // Compare Components
            if (TestCompareUtilities.BothNotNull(ExpectedViewpoint.Components, ActualViewpoint.Components, "Viewpoint.Components"))
            {
                if (ExpectedViewpoint.Components.Count == 1 && ActualViewpoint.Components.Count == 1)
                {
                    var ExpectedComponent = ExpectedViewpoint.Components.First();
                    var ActualComponent = ActualViewpoint.Components.First();

                    Assert.AreEqual(ExpectedComponent.AuthoringToolId, ActualComponent.AuthoringToolId, "AuthoringToolId");
                    Assert.IsTrue((ExpectedComponent.Color == null && ActualComponent.Color == null) || ExpectedComponent.Color.SequenceEqual(ActualComponent.Color), "Color");
                    Assert.AreEqual(ExpectedComponent.IfcGuid, ActualComponent.IfcGuid, "IfcGuid");
                    Assert.AreEqual(ExpectedComponent.OriginatingSystem, ActualComponent.OriginatingSystem, "OriginatingSystem");
                    Assert.AreEqual(ExpectedComponent.Selected, ActualComponent.Selected, "Selected");
                    Assert.AreEqual(ExpectedComponent.Visible, ActualComponent.Visible, "Visible");
                }
                else
                {
                    foreach (var CurrentComponent in ExpectedViewpoint.Components)
                    {
                        var ActualComponent = ActualViewpoint.Components
                            .Where(Curr => Curr.AuthoringToolId == CurrentComponent.AuthoringToolId)
                            .Where(Curr => (Curr.Color == null && CurrentComponent.Color == null) || Curr.Color.SequenceEqual(CurrentComponent.Color))
                            .Where(Curr => Curr.IfcGuid == CurrentComponent.IfcGuid)
                            .Where(Curr => Curr.OriginatingSystem == CurrentComponent.OriginatingSystem)
                            .Where(Curr => Curr.Selected == CurrentComponent.Selected)
                            .Where(Curr => Curr.Visible == CurrentComponent.Visible)
                            .FirstOrDefault();

                        Assert.IsNotNull(ActualComponent, "Did not find a matching component");
                    }
                }
            }

            // Compare Lines
            if (TestCompareUtilities.BothNotNull(ExpectedViewpoint.Lines, ActualViewpoint.Lines, "Viewpoint.Lines"))
            {
                foreach (var ExpectedLine in ExpectedViewpoint.Lines)
                {
                    var ActualLine = ActualViewpoint.Lines
                        .Where(Curr => Curr.EndPoint.X == ExpectedLine.EndPoint.X)
                        .Where(Curr => Curr.EndPoint.Y == ExpectedLine.EndPoint.Y)
                        .Where(Curr => Curr.EndPoint.Z == ExpectedLine.EndPoint.Z)
                        .Where(Curr => Curr.StartPoint.X == ExpectedLine.StartPoint.X)
                        .Where(Curr => Curr.StartPoint.Y == ExpectedLine.StartPoint.Y)
                        .Where(Curr => Curr.StartPoint.Z == ExpectedLine.StartPoint.Z)
                        .FirstOrDefault();
                    Assert.IsNotNull(ActualLine, "Did not find a matching line");
                }
            }

            // Compare Clipping planes
            if (TestCompareUtilities.BothNotNull(ExpectedViewpoint.ClippingPlanes, ActualViewpoint.ClippingPlanes, "Viewpoint.ClippingPlanes"))
            {
                foreach (var ExpectedPlane in ExpectedViewpoint.ClippingPlanes)
                {
                    var ActualPlane = ActualViewpoint.ClippingPlanes
                        .Where(Curr => Curr.Direction.X == ExpectedPlane.Direction.X)
                        .Where(Curr => Curr.Direction.Y == ExpectedPlane.Direction.Y)
                        .Where(Curr => Curr.Direction.Z == ExpectedPlane.Direction.Z)
                        .Where(Curr => Curr.Location.X == ExpectedPlane.Location.X)
                        .Where(Curr => Curr.Location.Y == ExpectedPlane.Location.Y)
                        .Where(Curr => Curr.Location.Z == ExpectedPlane.Location.Z)
                        .FirstOrDefault();
                    Assert.IsNotNull(ActualPlane);
                }
            }

            // Compare OrthogonalCamera
            if (TestCompareUtilities.BothNotNull(ExpectedViewpoint.OrthogonalCamera, ActualViewpoint.OrthogonalCamera, "Viewpoint.OrthogonalCamera"))
            {
                CompareOrthogonalCameras(ExpectedViewpoint.OrthogonalCamera, ActualViewpoint.OrthogonalCamera);
            }
            // Compare PerspectiveCamera
            if (TestCompareUtilities.BothNotNull(ExpectedViewpoint.PerspectiveCamera, ActualViewpoint.PerspectiveCamera, "Viewpoint.PerspectiveCamera"))
            {
                ComparePerspectiveCameras(ExpectedViewpoint.PerspectiveCamera, ActualViewpoint.PerspectiveCamera);
            }
        }

        public static void CompareOrthogonalCameras(OrthogonalCamera Expected, OrthogonalCamera Actual)
        {
            Assert.AreEqual(Expected.ViewToWorldScale, Actual.ViewToWorldScale, "ViewToWorldScale wrong");
            Assert.AreEqual(Expected.CameraDirection.X, Actual.CameraDirection.X, "Dir x wrong");
            Assert.AreEqual(Expected.CameraDirection.Y, Actual.CameraDirection.Y, "Dir y wrong");
            Assert.AreEqual(Expected.CameraDirection.Z, Actual.CameraDirection.Z, "Dir z wrong");
            Assert.AreEqual(Expected.CameraUpVector.X, Actual.CameraUpVector.X, "up x wrong");
            Assert.AreEqual(Expected.CameraUpVector.Y, Actual.CameraUpVector.Y, "up y wrong");
            Assert.AreEqual(Expected.CameraUpVector.Z, Actual.CameraUpVector.Z, "up z wrong");
            Assert.AreEqual(Expected.CameraViewPoint.X, Actual.CameraViewPoint.X, "viewpoint x wrong");
            Assert.AreEqual(Expected.CameraViewPoint.Y, Actual.CameraViewPoint.Y, "viewpoint y wrong");
            Assert.AreEqual(Expected.CameraViewPoint.Z, Actual.CameraViewPoint.Z, "viewpoint z wrong");
        }

        public static void ComparePerspectiveCameras(PerspectiveCamera Expected, PerspectiveCamera Actual)
        {
            Assert.AreEqual(Expected.FieldOfView, Actual.FieldOfView, "FoV wrong");
            Assert.AreEqual(Expected.CameraDirection.X, Actual.CameraDirection.X, "Dir x wrong");
            Assert.AreEqual(Expected.CameraDirection.Y, Actual.CameraDirection.Y, "Dir y wrong");
            Assert.AreEqual(Expected.CameraDirection.Z, Actual.CameraDirection.Z, "Dir z wrong");
            Assert.AreEqual(Expected.CameraUpVector.X, Actual.CameraUpVector.X, "up x wrong");
            Assert.AreEqual(Expected.CameraUpVector.Y, Actual.CameraUpVector.Y, "up y wrong");
            Assert.AreEqual(Expected.CameraUpVector.Z, Actual.CameraUpVector.Z, "up z wrong");
            Assert.AreEqual(Expected.CameraViewPoint.X, Actual.CameraViewPoint.X, "viewpoint x wrong");
            Assert.AreEqual(Expected.CameraViewPoint.Y, Actual.CameraViewPoint.Y, "viewpoint y wrong");
            Assert.AreEqual(Expected.CameraViewPoint.Z, Actual.CameraViewPoint.Z, "viewpoint z wrong");
        }

        public static void CompareViewpointBitmaps(Dictionary<VisualizationInfo, List<byte[]>> ExpectedBitmaps, Dictionary<VisualizationInfo, List<byte[]>> ActualBitmaps)
        {
            foreach (var ExpectedBitmap in ExpectedBitmaps)
            {
                var ActualBitmap = ActualBitmaps[ActualBitmaps.Keys.FirstOrDefault(Curr => Curr.GUID == ExpectedBitmap.Key.GUID)];
                foreach (var ExpectedBitmapSingleByte in ExpectedBitmap.Value)
                {
                    Assert.IsTrue(ActualBitmap.Any(Curr => Curr.SequenceEqual(ExpectedBitmapSingleByte)), "Did not find matching bitmap binary data");
                }
            }
        }

        public static void CompareViewpointSnapshots(ReadOnlyDictionary<string, byte[]> ExpectedSnapshots, ReadOnlyDictionary<string, byte[]> ActualSnapshots)
        {
            foreach (var ExpectedSnapshot in ExpectedSnapshots)
            {
                Assert.IsTrue(ExpectedSnapshot.Value.SequenceEqual(ActualSnapshots[ExpectedSnapshot.Key]), "Did not find matching snapshot binary data");
            }
        }
    }
}