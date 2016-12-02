using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using iabi.BCF.BCFv21;
using iabi.BCF.BCFv21.Schemas;
using Xunit;
using System.Collections;
using System.Reflection;

namespace iabi.BCF.Tests.BCFTestCases.v21
{
    public static class CompareTool
    {
        public static void CompareFiles(byte[] FileToImport, byte[] ReadAndWrittenFile)
        {
            // Check that archive file contents are present
            ArchiveFilesCompareTool.CompareZipArchiveFileEntries(FileToImport, ReadAndWrittenFile);
            var ExpectedFile = BCFv21Container.ReadStream(new MemoryStream(FileToImport));
            var ActualFile = BCFv21Container.ReadStream(new MemoryStream(ReadAndWrittenFile));
            CompareContainers(ExpectedFile, ActualFile, new ZipArchive(new MemoryStream(FileToImport)), new ZipArchive(new MemoryStream(ReadAndWrittenFile)));
        }

        public static void CheckBrandingCommentPresenceInEveryFile(byte[] createdZipArchive)
        {
            using (var memStream = new MemoryStream(createdZipArchive))
            {
                using (var zipArchive = new ZipArchive(memStream))
                {
                    var createdXmlEntries = zipArchive.Entries
                        .Where(entry => entry.Name.EndsWith("bcf.version", System.StringComparison.OrdinalIgnoreCase)
                                        || entry.Name.EndsWith("extensions.xsd", System.StringComparison.OrdinalIgnoreCase)
                                        || entry.Name.EndsWith("project.bcfp", System.StringComparison.OrdinalIgnoreCase)
                                        || entry.Name.EndsWith(".bcf", System.StringComparison.OrdinalIgnoreCase)
                                        || entry.Name.EndsWith(".bcfv", System.StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                    Assert.True(createdXmlEntries.Any()); // Might be an ivalid archive otherwise
                    foreach (var currentEntry in createdXmlEntries)
                    {
                        using (var streamReader = new StreamReader(currentEntry.Open()))
                        {
                            var readXml = streamReader.ReadToEnd();
                            var bcfToolsVersion = typeof(BrandingCommentFactory).GetTypeInfo().Assembly.GetName().Version;
                            var expectedStart = $"Created with the iabi.BCF library, V{bcfToolsVersion.Major}.{bcfToolsVersion.Minor}.{bcfToolsVersion.Revision} at ";
                            var expectedEnd = $". Visit {BrandingCommentFactory.IABI_BRANDING_URL} to find out more.";
                            Assert.True(readXml.Contains("<!--" + expectedStart));
                            Assert.True(readXml.Contains(expectedEnd + "-->"));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ExpectedContainer"></param>
        /// <param name="ActualContainer"></param>
        /// <param name="ExpectedArchive"></param>
        /// <param name="ActualArchive"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareContainers(BCFv21Container ExpectedContainer, BCFv21Container ActualContainer, ZipArchive ExpectedArchive = null, ZipArchive ActualArchive = null, bool OriginatesFromAPIConversion = false)
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

        public static void CompareProjectExtensions(BCFv21Container ExpectedContainer, BCFv21Container ActualContainer)
        {
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedContainer.ProjectExtensions, ActualContainer.ProjectExtensions, "ProjectExtensions"))
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
                Assert.True(Actual.Contains(CurrentValue), "Value: " + CurrentValue + " is missing in actual list of name: " + ListName);
            }
            // Check that no additional values are present
            foreach (var CurrentValue in Actual)
            {
                Assert.True(Expected.Contains(CurrentValue), "Value: " + CurrentValue + " is missing in expected list of name: " + ListName + " but present in actual list.");
            }
        }

        public static void CompareFileAttachments(BCFv21Container ExpectedContainer, BCFv21Container ActualContainer)
        {
            // Check that all files from the expected container are present
            foreach (var CurrentFile in ExpectedContainer.FileAttachments)
            {
                Assert.True(ActualContainer.FileAttachments.ContainsKey(CurrentFile.Key), "Missing file: " + CurrentFile.Key + " in actual container.");
                Assert.True(ExpectedContainer.FileAttachments[CurrentFile.Key].SequenceEqual(ActualContainer.FileAttachments[CurrentFile.Key]), "File: " + CurrentFile.Key + " binary different in actual container");
            }

            // Check that no file attachments were added
            foreach (var CurrentFile in ActualContainer.FileAttachments)
            {
                Assert.True(ExpectedContainer.FileAttachments.ContainsKey(CurrentFile.Key));
            }
        }

        /// <summary>
        ///     Will compare the project and version descriptions within the file
        /// </summary>
        /// <param name="ExpectedContainer"></param>
        /// <param name="ActualContainer"></param>
        public static void CompareProjectAndVersion(BCFv21Container ExpectedContainer, BCFv21Container ActualContainer)
        {
            // Compare project
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedContainer.BCFProject, ActualContainer.BCFProject, "BCFProject"))
            {
                if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedContainer.BCFProject.Project, ActualContainer.BCFProject.Project, "BCFProject.Project"))
                {
                    Assert.Equal(ExpectedContainer.BCFProject.Project.Name, ActualContainer.BCFProject.Project.Name);
                    Assert.Equal(ExpectedContainer.BCFProject.Project.ProjectId, ActualContainer.BCFProject.Project.ProjectId);
                }
                Assert.Equal(ExpectedContainer.BCFProject.ExtensionSchema, ActualContainer.BCFProject.ExtensionSchema);
            }

            // Compare version
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedContainer.BCFVersionInfo, ActualContainer.BCFVersionInfo, "BCFVersionInfo"))
            {
                if (ExpectedContainer.BCFVersionInfo.VersionId.Contains("2.1"))
                {
                    Assert.True(ActualContainer.BCFVersionInfo.VersionId.Contains("2.1"));
                }
                else
                {
                    Assert.True(false, "Unrecognized VersionId");
                }
            }
        }
    }

    public static class TestCompareUtilities
    {
        /// <summary>
        ///     Raises <see cref="Assert.Fail" /> if one is null and the other one is not
        /// </summary>
        /// <param name="ExpectedObject"></param>
        /// <param name="ActualObject"></param>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        public static bool BothNotNullAndEmpty(object ExpectedObject, object ActualObject, string ParameterName)
        {
            if (ExpectedObject == null && ActualObject != null)
            {
                Assert.True(false, "Parameter: " + ParameterName + "; Expected is null but actual is present.");
            }
            else if (ExpectedObject != null && ActualObject == null)
            {
                Assert.True(false, "Parameter: " + ParameterName + "; Actual is null but Expected is present.");
            }
            if (ExpectedObject != null && ExpectedObject is IEnumerable && ActualObject != null && ActualObject is IEnumerable)
            {
                if (((IEnumerable<object>) ExpectedObject).Count() != ((IEnumerable<object>) ActualObject).Count())
                {
                    Assert.True(false, "Parameter: " + ParameterName + "; Actual and Expected have different count of values");
                }
                return ((IEnumerable<object>) ExpectedObject).Count() != 0;
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

                    FilePresent = ActualZipArchive.Entries.Any(Curr => Curr.FullName == Folder + ActualViewpointReference);
                }
                if (!FilePresent && CurrentExpectedFile.FullName.ToUpperInvariant().EndsWith("project.bcfp".ToUpperInvariant()))
                {
                    // Originating file has maybe an empty project.bcfp, check this and do not enforce file availability then.
                    var ExpectedProjectXml = XmlUtilities.GetElementFromZipFile(ExpectedZipArchive, CurrentExpectedFile.FullName);

                    var ProjectShouldBeTreatedAsEmpty =
                        (!ExpectedProjectXml.Descendants("ExtensionSchema").Any() || ExpectedProjectXml.Descendants("ExtensionSchema").All(Curr => string.IsNullOrWhiteSpace(Curr.Value))) // ExtensionSchema not present or empty
                        && (!ExpectedProjectXml.Descendants("Project").Any() || ExpectedProjectXml.Descendants("Project").All(Curr => Curr.Attribute("ProjectId") == null || string.IsNullOrWhiteSpace(Curr.Attribute("ProjectId").Value))) // Project Id not present or empty
                        && (!ExpectedProjectXml.Descendants("Name").Where(Curr => Curr.Parent.Name.LocalName == "Project").Any() || ExpectedProjectXml.Descendants("Name").Where(Curr => Curr.Parent.Name.LocalName == "Project").All(Curr => string.IsNullOrWhiteSpace(Curr.Value))); // Project Name not present or empty
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
                    Assert.True(false, "File: \"" + CurrentExpectedFile.FullName + "\" not present in created archive.");
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
        /// </summary>
        /// <param name="ExpectedContainer"></param>
        /// <param name="ActualContainer"></param>
        /// <param name="ExpectedArchive"></param>
        /// <param name="ActualArchive"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareAllTopics(BCFv21Container ExpectedContainer, BCFv21Container ActualContainer, ZipArchive ExpectedArchive, ZipArchive ActualArchive, bool OriginatesFromAPIConversion)
        {
            foreach (var ExpectedTopic in ExpectedContainer.Topics)
            {
                // Make sure topic is present only one
                Assert.Equal(1, ActualContainer.Topics.Where(Curr => Curr.Markup.Topic.Guid == ExpectedTopic.Markup.Topic.Guid).Count());
                var ActualTopic = ActualContainer.Topics.FirstOrDefault(Curr => Curr.Markup.Topic.Guid == ExpectedTopic.Markup.Topic.Guid);
                CompareSingleTopic(ExpectedTopic, ActualTopic, ExpectedArchive, ActualArchive, OriginatesFromAPIConversion);
            }
        }

        /// <summary>
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
                Assert.True(ExpectedTopic.SnippetData.SequenceEqual(ActualTopic.SnippetData));
            }
            else
            {
                Assert.Null(ActualTopic.SnippetData);
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
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedMarkup.Header, ActualMarkup.Header, "Markup.Header"))
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

                    Assert.True(FoundExactlyOneMatchignEntryInActual, "Found not matching header entry in actual file.");
                }

                // Check that the both sections contain the same number of entries
                Assert.Equal(ExpectedMarkup.Header.Count, ActualMarkup.Header.Count);
            }

            CompareComments(ExpectedMarkup.Comment, ActualMarkup.Comment);

            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedMarkup.Viewpoints, ActualMarkup.Viewpoints, "Markup.Viewpoints"))
            {
                CompareViewpoints(ExpectedMarkup.Viewpoints, ActualMarkup.Viewpoints, ExpectedArchive, ActualArchive, ExpectedMarkup.Topic.Guid);
            }

            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedMarkup.Topic, ActualMarkup.Topic, "Markup.Topic"))
            {
                CompareTopic(ExpectedMarkup.Topic, ActualMarkup.Topic);
            }
        }

        public static void CompareViewpoints(List<ViewPoint> ExpectedViewpoints, List<ViewPoint> ActualViewpoints, ZipArchive ExpectedArchive, ZipArchive ActualArchive, string TopicGuid)
        {
            // Count Matches
            Assert.Equal(ExpectedViewpoints.Count, ActualViewpoints.Count);

            foreach (var ExpectedViewpoint in ExpectedViewpoints)
            {
                var ActualViewpoint = ActualViewpoints.FirstOrDefault(Curr => Curr.Guid == ExpectedViewpoint.Guid);

                Assert.Equal(ExpectedViewpoint.Guid, ActualViewpoint.Guid);
                if (ExpectedViewpoint.Snapshot != ActualViewpoint.Snapshot)
                {
                    // This means the file reference to the snapshot is different; need to check it then in binary format
                    var ExpectedViewpointSnapshotStream = new MemoryStream();
                    ExpectedArchive.Entries.FirstOrDefault(Curr => Curr.FullName == TopicGuid + "/" + ExpectedViewpoint.Snapshot).Open().CopyTo(ExpectedViewpointSnapshotStream);
                    var ExpectedViewpointSnapshotBinary = ExpectedViewpointSnapshotStream.ToArray();

                    Assert.False(string.IsNullOrWhiteSpace(ActualViewpoint.Snapshot));
                    var ActualViewpointEntry = ActualArchive.Entries.FirstOrDefault(Curr => Curr.FullName == TopicGuid + "/" + ActualViewpoint.Snapshot);
                    Assert.NotNull(ActualViewpointEntry);
                    var ActualViewpointSnapshotStream = new MemoryStream();
                    ActualViewpointEntry.Open().CopyTo(ActualViewpointSnapshotStream);
                    var ActualViewpointSnapshotBinary = ActualViewpointSnapshotStream.ToArray();

                    Assert.True(ExpectedViewpointSnapshotBinary.SequenceEqual(ActualViewpointSnapshotBinary));
                }
                Assert.Equal(ExpectedViewpoint.Viewpoint, ActualViewpoint.Viewpoint);
            }
        }

        public static void CompareBimSnippet(BimSnippet Expected, BimSnippet Actual)
        {
            Assert.Equal(Expected.isExternal, Actual.isExternal);
            Assert.Equal(Expected.Reference, Actual.Reference);
            Assert.Equal(Expected.ReferenceSchema, Actual.ReferenceSchema);
            Assert.Equal(Expected.SnippetType, Actual.SnippetType);
        }

        public static void CompareTopic(Topic ExpectedTopic, Topic ActualTopic)
        {
            Assert.Equal(ExpectedTopic.AssignedTo, ActualTopic.AssignedTo);

            // Compare Snippet
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedTopic.BimSnippet, ActualTopic.BimSnippet, "Markup.BimSnippet"))
            {
                CompareBimSnippet(ExpectedTopic.BimSnippet, ActualTopic.BimSnippet);
            }

            // Compare document references
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedTopic.DocumentReference, ActualTopic.DocumentReference, "Markup.DocumentReferences"))
            {
                Assert.Equal(ExpectedTopic.DocumentReference.Count, ActualTopic.DocumentReference.Count);

                foreach (var ExpectedDocumentReference in ExpectedTopic.DocumentReference)
                {
                    // Find the matching document reference in the actual topic
                    var ActualDocumentReference = ActualTopic.DocumentReference
                        .Where(Curr => Curr.Description == ExpectedDocumentReference.Description)
                        .Where(Curr => Curr.Guid == ExpectedDocumentReference.Guid)
                        .Where(Curr => Curr.isExternal == ExpectedDocumentReference.isExternal)
                        .Where(Curr => Curr.ReferencedDocument == ExpectedDocumentReference.ReferencedDocument)
                        .FirstOrDefault();
                    Assert.NotNull(ActualDocumentReference);
                }
            }

            // Check labels
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedTopic.Labels, ActualTopic.Labels, "Markup.Labels"))
            {
                Assert.Equal(ExpectedTopic.Labels.Count, ActualTopic.Labels.Count);
                foreach (var ExpectedLabel in ExpectedTopic.Labels)
                {
                    Assert.True(ActualTopic.Labels.Contains(ExpectedLabel));
                }
            }

            // Check related topics
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedTopic.RelatedTopic, ActualTopic.RelatedTopic, "Markup.RelatedTopics"))
            {
                Assert.Equal(ExpectedTopic.RelatedTopic.Count, ActualTopic.RelatedTopic.Count);
                foreach (var ExpectedRelatedTopic in ExpectedTopic.RelatedTopic)
                {
                    var ActualRelatedTopic = ActualTopic.RelatedTopic
                        .Where(Curr => Curr.Guid == ExpectedRelatedTopic.Guid)
                        .FirstOrDefault();
                    Assert.NotNull(ActualRelatedTopic);
                }
            }

            Assert.Equal(ExpectedTopic.CreationAuthor, ActualTopic.CreationAuthor);
            Assert.True((int) (ExpectedTopic.CreationDate - ActualTopic.CreationDate).TotalSeconds < 5);
            Assert.True(ExpectedTopic.ShouldSerializeCreationDate());
            Assert.True(ActualTopic.ShouldSerializeCreationDate());

            if (!(string.IsNullOrWhiteSpace(ExpectedTopic.Description) && string.IsNullOrWhiteSpace(ActualTopic.Description)))
            {
                Assert.Equal(ExpectedTopic.Description, ActualTopic.Description);
            }
            Assert.Equal(ExpectedTopic.Guid, ActualTopic.Guid);

            if (ExpectedTopic.Index != ActualTopic.Index)
            {
                Assert.True(false, "Index does not match");
            }

            Assert.Equal(ExpectedTopic.ModifiedAuthor, ActualTopic.ModifiedAuthor);
            Assert.Equal(ExpectedTopic.ModifiedDate, ActualTopic.ModifiedDate);
            Assert.Equal(ExpectedTopic.ModifiedDateSpecified, ActualTopic.ModifiedDateSpecified);
            Assert.Equal(ExpectedTopic.Priority, ActualTopic.Priority);
            Assert.Equal(ExpectedTopic.ReferenceLink, ActualTopic.ReferenceLink);
            Assert.Equal(ExpectedTopic.Title, ActualTopic.Title);
            Assert.Equal(ExpectedTopic.TopicStatus, ActualTopic.TopicStatus);
            Assert.Equal(ExpectedTopic.TopicType, ActualTopic.TopicType);
        }

        public static void CompareComments(List<Comment> ExpectedComments, List<Comment> ActualComments)
        {
            // Compare count
            Assert.Equal(ExpectedComments.Count, ActualComments.Count);

            foreach (var ExpectedComment in ExpectedComments)
            {
                // Get actual comment
                var ActualComment = ActualComments.FirstOrDefault(Curr => Curr.Guid == ExpectedComment.Guid);
                Assert.NotNull(ActualComment);
                CompareSingleComment(ExpectedComment, ActualComment);
            }
        }

        public static void CompareSingleComment(Comment ExpectedComment, Comment ActualComment)
        {
            Assert.Equal(ExpectedComment.Author, ActualComment.Author);

            var CommentTextMatches = ExpectedComment.Comment1 == ActualComment.Comment1;
            if (!CommentTextMatches)
            {
                // It's possible that there is no actual comment text but the XML file does have the <Comment> element with an empty value
                CommentTextMatches = string.IsNullOrWhiteSpace(ExpectedComment.Comment1) && string.IsNullOrWhiteSpace(ActualComment.Comment1);
            }
            Assert.True(CommentTextMatches, "No match in comment: Comment1");

            Assert.Equal(ExpectedComment.Date, ActualComment.Date);
            Assert.Equal(ExpectedComment.Guid, ActualComment.Guid);
            Assert.Equal(ExpectedComment.ModifiedAuthor, ActualComment.ModifiedAuthor);
            Assert.Equal(ExpectedComment.ModifiedDate, ActualComment.ModifiedDate);
            Assert.Equal(ExpectedComment.ModifiedDateSpecified, ActualComment.ModifiedDateSpecified);

            if (ExpectedComment.ShouldSerializeViewpoint())
            {
                Assert.True(ActualComment.ShouldSerializeViewpoint(), "No match in comment: Viewpoint");
                Assert.Equal(ExpectedComment.Viewpoint.Guid, ActualComment.Viewpoint.Guid);
            }
            else
            {
                Assert.False(ActualComment.ShouldSerializeViewpoint(), "No match in comment: Viewpoint");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ExpectedViewpoints"></param>
        /// <param name="ActualViewpoints"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareViewpoints(IEnumerable<VisualizationInfo> ExpectedViewpoints, IEnumerable<VisualizationInfo> ActualViewpoints, bool OriginatesFromAPIConversion)
        {
            foreach (var ExpectedViewpoint in ExpectedViewpoints)
            {
                var ActualViewpoint = ActualViewpoints.FirstOrDefault(Curr => Curr.Guid == ExpectedViewpoint.Guid);
                Assert.NotNull(ActualViewpoint);
                CompareSingleViewpoints(ExpectedViewpoint, ActualViewpoint, OriginatesFromAPIConversion);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ExpectedViewpoint"></param>
        /// <param name="ActualViewpoint"></param>
        /// <param name="OriginatesFromAPIConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareSingleViewpoints(VisualizationInfo ExpectedViewpoint, VisualizationInfo ActualViewpoint, bool OriginatesFromAPIConversion)
        {
            Assert.Equal(ExpectedViewpoint.Guid, ActualViewpoint.Guid);

            // Compare Bitmaps
            if (!OriginatesFromAPIConversion && TestCompareUtilities.BothNotNullAndEmpty(ExpectedViewpoint.Bitmap, ActualViewpoint.Bitmap, "Viewpoint.Bitmaps"))
            {
                Assert.Equal(ExpectedViewpoint.Bitmap.Count, ActualViewpoint.Bitmap.Count);
                foreach (var ExpectedBitmap in ExpectedViewpoint.Bitmap)
                {
                    var ActualBitmap = ActualViewpoint.Bitmap
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
                    Assert.NotNull(ActualViewpoint);
                }
            }

            // Compare Components
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedViewpoint.Components, ActualViewpoint.Components, "Viewpoint.Components"))
            {
                Assert.Equal(ExpectedViewpoint.Components.DefaultVisibilityComponents, ActualViewpoint.Components.DefaultVisibilityComponents);
                Assert.Equal(ExpectedViewpoint.Components.DefaultVisibilityOpenings, ActualViewpoint.Components.DefaultVisibilityOpenings);
                Assert.Equal(ExpectedViewpoint.Components.DefaultVisibilitySpaceBoundaries, ActualViewpoint.Components.DefaultVisibilitySpaceBoundaries);
                Assert.Equal(ExpectedViewpoint.Components.DefaultVisibilitySpaces, ActualViewpoint.Components.DefaultVisibilitySpaces);

                if (ExpectedViewpoint.Components.Component.Count == 1 && ActualViewpoint.Components.Component.Count == 1)
                {
                    var ExpectedComponent = ExpectedViewpoint.Components.Component.First();
                    var ActualComponent = ActualViewpoint.Components.Component.First();

                    Assert.Equal(ExpectedComponent.AuthoringToolId, ActualComponent.AuthoringToolId);
                    Assert.True((ExpectedComponent.Color == null && ActualComponent.Color == null) || ExpectedComponent.Color.SequenceEqual(ActualComponent.Color), "Color");
                    Assert.Equal(ExpectedComponent.IfcGuid, ActualComponent.IfcGuid);
                    Assert.Equal(ExpectedComponent.OriginatingSystem, ActualComponent.OriginatingSystem);
                    Assert.Equal(ExpectedComponent.Selected, ActualComponent.Selected);
                    Assert.Equal(ExpectedComponent.Visible, ActualComponent.Visible);
                }
                else
                {
                    foreach (var CurrentComponent in ExpectedViewpoint.Components.Component)
                    {
                        var ActualComponent = ActualViewpoint.Components.Component
                            .Where(Curr => Curr.AuthoringToolId == CurrentComponent.AuthoringToolId)
                            .Where(Curr => (Curr.Color == null && CurrentComponent.Color == null) || Curr.Color.SequenceEqual(CurrentComponent.Color))
                            .Where(Curr => Curr.IfcGuid == CurrentComponent.IfcGuid)
                            .Where(Curr => Curr.OriginatingSystem == CurrentComponent.OriginatingSystem)
                            .Where(Curr => Curr.Selected == CurrentComponent.Selected)
                            .Where(Curr => Curr.Visible == CurrentComponent.Visible)
                            .FirstOrDefault();

                        Assert.NotNull(ActualComponent);
                    }
                }
            }

            // Compare Lines
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedViewpoint.Lines, ActualViewpoint.Lines, "Viewpoint.Lines"))
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
                    Assert.NotNull(ActualLine);
                }
            }

            // Compare Clipping planes
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedViewpoint.ClippingPlanes, ActualViewpoint.ClippingPlanes, "Viewpoint.ClippingPlanes"))
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
                    Assert.NotNull(ActualPlane);
                }
            }

            // Compare OrthogonalCamera
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedViewpoint.OrthogonalCamera, ActualViewpoint.OrthogonalCamera, "Viewpoint.OrthogonalCamera"))
            {
                CompareOrthogonalCameras(ExpectedViewpoint.OrthogonalCamera, ActualViewpoint.OrthogonalCamera);
            }
            // Compare PerspectiveCamera
            if (TestCompareUtilities.BothNotNullAndEmpty(ExpectedViewpoint.PerspectiveCamera, ActualViewpoint.PerspectiveCamera, "Viewpoint.PerspectiveCamera"))
            {
                ComparePerspectiveCameras(ExpectedViewpoint.PerspectiveCamera, ActualViewpoint.PerspectiveCamera);
            }
        }

        public static void CompareOrthogonalCameras(OrthogonalCamera Expected, OrthogonalCamera Actual)
        {
            Assert.Equal(Expected.ViewToWorldScale, Actual.ViewToWorldScale);
            Assert.Equal(Expected.CameraDirection.X, Actual.CameraDirection.X);
            Assert.Equal(Expected.CameraDirection.Y, Actual.CameraDirection.Y);
            Assert.Equal(Expected.CameraDirection.Z, Actual.CameraDirection.Z);
            Assert.Equal(Expected.CameraUpVector.X, Actual.CameraUpVector.X);
            Assert.Equal(Expected.CameraUpVector.Y, Actual.CameraUpVector.Y);
            Assert.Equal(Expected.CameraUpVector.Z, Actual.CameraUpVector.Z);
            Assert.Equal(Expected.CameraViewPoint.X, Actual.CameraViewPoint.X);
            Assert.Equal(Expected.CameraViewPoint.Y, Actual.CameraViewPoint.Y);
            Assert.Equal(Expected.CameraViewPoint.Z, Actual.CameraViewPoint.Z);
        }

        public static void ComparePerspectiveCameras(PerspectiveCamera Expected, PerspectiveCamera Actual)
        {
            Assert.Equal(Expected.FieldOfView, Actual.FieldOfView);
            Assert.Equal(Expected.CameraDirection.X, Actual.CameraDirection.X);
            Assert.Equal(Expected.CameraDirection.Y, Actual.CameraDirection.Y);
            Assert.Equal(Expected.CameraDirection.Z, Actual.CameraDirection.Z);
            Assert.Equal(Expected.CameraUpVector.X, Actual.CameraUpVector.X);
            Assert.Equal(Expected.CameraUpVector.Y, Actual.CameraUpVector.Y);
            Assert.Equal(Expected.CameraUpVector.Z, Actual.CameraUpVector.Z);
            Assert.Equal(Expected.CameraViewPoint.X, Actual.CameraViewPoint.X);
            Assert.Equal(Expected.CameraViewPoint.Y, Actual.CameraViewPoint.Y);
            Assert.Equal(Expected.CameraViewPoint.Z, Actual.CameraViewPoint.Z);
        }

        public static void CompareViewpointBitmaps(Dictionary<VisualizationInfo, List<byte[]>> ExpectedBitmaps, Dictionary<VisualizationInfo, List<byte[]>> ActualBitmaps)
        {
            foreach (var ExpectedBitmap in ExpectedBitmaps)
            {
                var ActualBitmap = ActualBitmaps[ActualBitmaps.Keys.FirstOrDefault(Curr => Curr.Guid == ExpectedBitmap.Key.Guid)];
                foreach (var ExpectedBitmapSingleByte in ExpectedBitmap.Value)
                {
                    Assert.True(ActualBitmap.Any(Curr => Curr.SequenceEqual(ExpectedBitmapSingleByte)), "Did not find matching bitmap binary data");
                }
            }
        }

        public static void CompareViewpointSnapshots(ReadOnlyDictionary<string, byte[]> ExpectedSnapshots, ReadOnlyDictionary<string, byte[]> ActualSnapshots)
        {
            foreach (var ExpectedSnapshot in ExpectedSnapshots)
            {
                Assert.True(ExpectedSnapshot.Value.SequenceEqual(ActualSnapshots[ExpectedSnapshot.Key]), "Did not find matching snapshot binary data");
            }
        }
    }
}