using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Dangl.BCF.BCFv2;
using Dangl.BCF.BCFv2.Schemas;
using Xunit;

namespace Dangl.BCF.Tests.BCFTestCases.v2
{
    public static class CompareTool
    {
        public static void CompareFiles(byte[] fileToImport, byte[] readAndWrittenFile)
        {
            // Check that archive file contents are present
            ArchiveFilesCompareTool.CompareZipArchiveFileEntries(fileToImport, readAndWrittenFile);
            var expectedFile = BCFv2Container.ReadStream(new MemoryStream(fileToImport));
            var actualFile = BCFv2Container.ReadStream(new MemoryStream(readAndWrittenFile));
            CompareContainers(expectedFile, actualFile, new ZipArchive(new MemoryStream(fileToImport)), new ZipArchive(new MemoryStream(readAndWrittenFile)));
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
                            var expectedStart = $"Created with the Dangl.BCF library, Version {FileVersionProvider.NuGetVersion} at ";
                            var expectedEnd = $". Visit {BrandingCommentFactory.BRANDING_URL} to find out more.";
                            Assert.Contains("<!--" + expectedStart, readXml);
                            Assert.Contains(expectedEnd + "-->", readXml);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="expectedContainer"></param>
        /// <param name="actualContainer"></param>
        /// <param name="expectedArchive"></param>
        /// <param name="actualArchive"></param>
        /// <param name="originatesFromApiConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareContainers(BCFv2Container expectedContainer, BCFv2Container actualContainer, ZipArchive expectedArchive = null, ZipArchive actualArchive = null, bool originatesFromApiConversion = false)
        {
            CompareProjectAndVersion(expectedContainer, actualContainer);
            CompareFileAttachments(expectedContainer, actualContainer);
            CompareProjectExtensions(expectedContainer, actualContainer);

            if (expectedArchive == null && actualArchive == null)
            {
                using (var memStream01 = new MemoryStream())
                {
                    expectedContainer.WriteStream(memStream01);
                    expectedArchive = new ZipArchive(memStream01);
                    using (var memStream02 = new MemoryStream())
                    {
                        expectedContainer.WriteStream(memStream02);
                        actualArchive = new ZipArchive(memStream02);
                        TopicsCompareTool.CompareAllTopics(expectedContainer, actualContainer, expectedArchive, actualArchive, originatesFromApiConversion);
                        return;
                    }
                }
            }

            TopicsCompareTool.CompareAllTopics(expectedContainer, actualContainer, expectedArchive, actualArchive, originatesFromApiConversion);
        }

        public static void CompareProjectExtensions(BCFv2Container expectedContainer, BCFv2Container actualContainer)
        {
            if (TestCompareUtilities.BothNotNull(expectedContainer.ProjectExtensions, actualContainer.ProjectExtensions, "ProjectExtensions"))
            {
                CompareStringList(expectedContainer.ProjectExtensions.SnippetType, actualContainer.ProjectExtensions.SnippetType, "SnippetType");
                CompareStringList(expectedContainer.ProjectExtensions.Priority, actualContainer.ProjectExtensions.Priority, "Priority");
                CompareStringList(expectedContainer.ProjectExtensions.TopicStatus, actualContainer.ProjectExtensions.TopicStatus, "TopicStatus");
                CompareStringList(expectedContainer.ProjectExtensions.TopicType, actualContainer.ProjectExtensions.TopicType, "TopicType");
                CompareStringList(expectedContainer.ProjectExtensions.UserIdType, actualContainer.ProjectExtensions.UserIdType, "UserIdType");
                CompareStringList(expectedContainer.ProjectExtensions.TopicLabel, actualContainer.ProjectExtensions.TopicLabel, "TopicLabel");
            }
        }

        public static void CompareStringList(List<string> expected, List<string> actual, string listName)
        {
            // Check that all expected values are present
            foreach (var currentValue in expected)
            {
                Assert.True(actual.Contains(currentValue), "Value: " + currentValue + " is missing in actual list of name: " + listName);
            }
            // Check that no additional values are present
            foreach (var currentValue in actual)
            {
                Assert.True(expected.Contains(currentValue), "Value: " + currentValue + " is missing in expected list of name: " + listName + " but present in actual list.");
            }
        }

        public static void CompareFileAttachments(BCFv2Container expectedContainer, BCFv2Container actualContainer)
        {
            // Check that all files from the expected container are present
            foreach (var currentFile in expectedContainer.FileAttachments)
            {
                Assert.True(actualContainer.FileAttachments.ContainsKey(currentFile.Key), "Missing file: " + currentFile.Key + " in actual container.");
                Assert.True(expectedContainer.FileAttachments[currentFile.Key].SequenceEqual(actualContainer.FileAttachments[currentFile.Key]), "File: " + currentFile.Key + " binary different in actual container");
            }

            // Check that no file attachments were added
            foreach (var currentFile in actualContainer.FileAttachments)
            {
                Assert.True(expectedContainer.FileAttachments.ContainsKey(currentFile.Key));
            }
        }

        /// <summary>
        ///     Will compare the project and version descriptions within the file
        /// </summary>
        /// <param name="expectedContainer"></param>
        /// <param name="actualContainer"></param>
        public static void CompareProjectAndVersion(BCFv2Container expectedContainer, BCFv2Container actualContainer)
        {
            // Compare project
            if (TestCompareUtilities.BothNotNull(expectedContainer.BcfProject, actualContainer.BcfProject, "BCFProject"))
            {
                if (TestCompareUtilities.BothNotNull(expectedContainer.BcfProject.Project, actualContainer.BcfProject.Project, "BCFProject.Project"))
                {
                    Assert.Equal(expectedContainer.BcfProject.Project.Name, actualContainer.BcfProject.Project.Name);
                    Assert.Equal(expectedContainer.BcfProject.Project.ProjectId, actualContainer.BcfProject.Project.ProjectId);
                }
                Assert.Equal(expectedContainer.BcfProject.ExtensionSchema, actualContainer.BcfProject.ExtensionSchema);
            }

            // Compare version
            if (TestCompareUtilities.BothNotNull(expectedContainer.BcfVersionInfo, actualContainer.BcfVersionInfo, "BCFVersionInfo"))
            {
                if (expectedContainer.BcfVersionInfo.VersionId.Contains("2.0"))
                {
                    Assert.Contains("2.0", actualContainer.BcfVersionInfo.VersionId);
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
        /// <param name="expectedObject"></param>
        /// <param name="actualObject"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static bool BothNotNull(object expectedObject, object actualObject, string parameterName)
        {
            if (expectedObject == null && actualObject != null)
            {
                Assert.True(false, "Parameter: " + parameterName + "; Expected is null but actual is present.");
            }
            else if (expectedObject != null && actualObject == null)
            {
                Assert.True(false, "Parameter: " + parameterName + "; Actual is null but Expected is present.");
            }
            return expectedObject != null;
        }
    }

    public static class ArchiveFilesCompareTool
    {
        public static void CompareZipArchiveFileEntries(byte[] fileToImport, byte[] readAndWrittenFile)
        {
            var expectedZipArchive = new ZipArchive(new MemoryStream(fileToImport));
            var actualZipArchive = new ZipArchive(new MemoryStream(readAndWrittenFile));

            // Check that all expected files are present
            foreach (var currentExpectedFile in expectedZipArchive.Entries)
            {
                var filePresent = actualZipArchive.Entries.Any(curr => curr.FullName == currentExpectedFile.FullName);
                if (!filePresent && (
                    currentExpectedFile.FullName.ToUpperInvariant().EndsWith(".png".ToUpperInvariant())
                    || currentExpectedFile.FullName.ToUpperInvariant().EndsWith(".jpg".ToUpperInvariant())
                    || currentExpectedFile.FullName.ToUpperInvariant().EndsWith(".jpeg".ToUpperInvariant())))
                {
                    // It may be a snapshot or a bitmap that was just renamed (for example, from snapshot.png to {Guid}.png
                    // Will search for a file in the same directory with the same size
                    filePresent = FileIsPresentInSameFolderWithDifferentName(expectedZipArchive, actualZipArchive, currentExpectedFile.FullName);
                }
                if (!filePresent && currentExpectedFile.FullName.ToUpperInvariant().EndsWith(".bcfv".ToUpperInvariant()))
                {
                    // Viewpoints may be renamed
                    var folder = currentExpectedFile.FullName.Substring(0, currentExpectedFile.FullName.Length - currentExpectedFile.FullName.Split('/').Last().Length);
                    var expectedMarkupXml = XmlUtilities.GetElementFromZipFile(expectedZipArchive, folder + "markup.bcf");
                    var viewpointGuid = expectedMarkupXml.Descendants("Viewpoints").Where(curr => curr.Descendants("Viewpoint").FirstOrDefault().Value == currentExpectedFile.FullName.Split('/').Last()).FirstOrDefault().Attributes().FirstOrDefault(curr => curr.Name == "Guid").Value;

                    // Get viewpoint reference for this viewpoint in the created archive
                    var actualMarkupXml = XmlUtilities.GetElementFromZipFile(actualZipArchive, folder + "markup.bcf");
                    var actualViewpointReference = actualMarkupXml.Descendants("Viewpoints").Where(curr => curr.Attributes().Where(attr => attr.Name == "Guid" && attr.Value == viewpointGuid).Any()).FirstOrDefault().Descendants("Viewpoint").FirstOrDefault().Value;

                    filePresent = actualZipArchive.Entries.Any(curr => curr.FullName == folder + actualViewpointReference);
                }
                if (!filePresent && currentExpectedFile.FullName.ToUpperInvariant().EndsWith("project.bcfp".ToUpperInvariant()))
                {
                    // Originating file has maybe an empty project.bcfp, check this and do not enforce file availability then.
                    var expectedProjectXml = XmlUtilities.GetElementFromZipFile(expectedZipArchive, currentExpectedFile.FullName);

                    var projectShouldBeTreatedAsEmpty =
                        (!expectedProjectXml.Descendants("ExtensionSchema").Any() || expectedProjectXml.Descendants("ExtensionSchema").All(curr => string.IsNullOrWhiteSpace(curr.Value))) // ExtensionSchema not present or empty
                        && (!expectedProjectXml.Descendants("Project").Any() || expectedProjectXml.Descendants("Project").All(curr => curr.Attribute("ProjectId") == null || string.IsNullOrWhiteSpace(curr.Attribute("ProjectId").Value))) // Project Id not present or empty
                        && (!expectedProjectXml.Descendants("Name").Where(curr => curr.Parent.Name.LocalName == "Project").Any() || expectedProjectXml.Descendants("Name").Where(curr => curr.Parent.Name.LocalName == "Project").All(curr => string.IsNullOrWhiteSpace(curr.Value))); // Project Name not present or empty
                    if (projectShouldBeTreatedAsEmpty)
                    {
                        filePresent = true;
                    }
                }

                if (!filePresent)
                {
                    filePresent = currentExpectedFile.Length == 0; // Empty entry, then dont look for it
                }

                if (!filePresent)
                {
                    Assert.True(false, "File: \"" + currentExpectedFile.FullName + "\" not present in created archive.");
                }
            }
        }

        public static bool FileIsPresentInSameFolderWithDifferentName(ZipArchive expectedArchive, ZipArchive actualArchive, string expectedEntryFullName)
        {
            var expectedEntry = expectedArchive.Entries.FirstOrDefault(curr => curr.FullName == expectedEntryFullName);
            var folder = expectedEntryFullName.Substring(0, expectedEntryFullName.Length - expectedEntryFullName.Split('/').Last().Length);

            var sameFileInFolder = actualArchive.Entries
                .Where(curr => curr.FullName.StartsWith(folder))
                .Where(curr => !curr.FullName.Substring(folder.Length).Contains('/'))
                .Where(curr => curr.Length == expectedEntry.Length)
                .Any();

            return sameFileInFolder;
        }
    }

    public static class TopicsCompareTool
    {
        /// <summary>
        /// </summary>
        /// <param name="expectedContainer"></param>
        /// <param name="actualContainer"></param>
        /// <param name="expectedArchive"></param>
        /// <param name="actualArchive"></param>
        /// <param name="originatesFromApiConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareAllTopics(BCFv2Container expectedContainer, BCFv2Container actualContainer, ZipArchive expectedArchive, ZipArchive actualArchive, bool originatesFromApiConversion)
        {
            foreach (var expectedTopic in expectedContainer.Topics)
            {
                // Make sure topic is present only one
                Assert.Single(actualContainer.Topics.Where(curr => curr.Markup.Topic.Guid == expectedTopic.Markup.Topic.Guid));
                var actualTopic = actualContainer.Topics.FirstOrDefault(curr => curr.Markup.Topic.Guid == expectedTopic.Markup.Topic.Guid);
                CompareSingleTopic(expectedTopic, actualTopic, expectedArchive, actualArchive, originatesFromApiConversion);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="expectedTopic"></param>
        /// <param name="actualTopic"></param>
        /// <param name="expectedArchive"></param>
        /// <param name="actualArchive"></param>
        /// <param name="originatesFromApiConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareSingleTopic(BCFTopic expectedTopic, BCFTopic actualTopic, ZipArchive expectedArchive, ZipArchive actualArchive, bool originatesFromApiConversion)
        {
            // Compare Markup
            CompareMarkup(expectedTopic.Markup, actualTopic.Markup, expectedArchive, actualArchive);

            // Compare SnippetData
            if (expectedTopic.SnippetData != null)
            {
                Assert.True(expectedTopic.SnippetData.SequenceEqual(actualTopic.SnippetData));
            }
            else
            {
                Assert.Null(actualTopic.SnippetData);
            }

            // Compare Viewpoints
            CompareViewpoints(expectedTopic.Viewpoints, actualTopic.Viewpoints, originatesFromApiConversion);

            // Compare ViewpointBitmaps
            if (!originatesFromApiConversion)
            {
                CompareViewpointBitmaps(expectedTopic.ViewpointBitmaps, actualTopic.ViewpointBitmaps);
            }

            // Compare ViewpointSnapshots
            CompareViewpointSnapshots(expectedTopic.ViewpointSnapshots, actualTopic.ViewpointSnapshots);
        }

        public static void CompareMarkup(Markup expectedMarkup, Markup actualMarkup, ZipArchive expectedArchive, ZipArchive actualArchive)
        {
            // Compare Header Section
            if (TestCompareUtilities.BothNotNull(expectedMarkup.Header, actualMarkup.Header, "Markup.Header"))
            {
                foreach (var currentHeaderEntry in expectedMarkup.Header)
                {
                    var foundExactlyOneMatchignEntryInActual = actualMarkup.Header
                        .Where(curr => curr.Date == currentHeaderEntry.Date)
                        .Where(curr => curr.Filename == currentHeaderEntry.Filename)
                        .Where(curr => curr.IfcProject == currentHeaderEntry.IfcProject)
                        .Where(curr => curr.IfcSpatialStructureElement == currentHeaderEntry.IfcSpatialStructureElement)
                        .Where(curr => curr.isExternal == currentHeaderEntry.isExternal)
                        .Where(curr => curr.Reference == currentHeaderEntry.Reference)
                        .Count() == 1;

                    Assert.True(foundExactlyOneMatchignEntryInActual, "Found not matching header entry in actual file.");
                }

                // Check that the both sections contain the same number of entries
                Assert.Equal(expectedMarkup.Header.Count, actualMarkup.Header.Count);
            }

            CompareComments(expectedMarkup.Comment, actualMarkup.Comment);

            if (TestCompareUtilities.BothNotNull(expectedMarkup.Viewpoints, actualMarkup.Viewpoints, "Markup.Viewpoints"))
            {
                CompareViewpoints(expectedMarkup.Viewpoints, actualMarkup.Viewpoints, expectedArchive, actualArchive, expectedMarkup.Topic.Guid);
            }

            if (TestCompareUtilities.BothNotNull(expectedMarkup.Topic, actualMarkup.Topic, "Markup.Topic"))
            {
                CompareTopic(expectedMarkup.Topic, actualMarkup.Topic);
            }
        }

        public static void CompareViewpoints(List<ViewPoint> expectedViewpoints, List<ViewPoint> actualViewpoints, ZipArchive expectedArchive, ZipArchive actualArchive, string topicGuid)
        {
            // Count Matches
            Assert.Equal(expectedViewpoints.Count, actualViewpoints.Count);

            foreach (var expectedViewpoint in expectedViewpoints)
            {
                var actualViewpoint = actualViewpoints.FirstOrDefault(curr => curr.Guid == expectedViewpoint.Guid);

                Assert.Equal(expectedViewpoint.Guid, actualViewpoint.Guid);
                if (expectedViewpoint.Snapshot != actualViewpoint.Snapshot)
                {
                    // This means the file reference to the snapshot is different; need to check it then in binary format
                    var expectedViewpointSnapshotStream = new MemoryStream();
                    expectedArchive.Entries.FirstOrDefault(curr => curr.FullName == topicGuid + "/" + expectedViewpoint.Snapshot).Open().CopyTo(expectedViewpointSnapshotStream);
                    var expectedViewpointSnapshotBinary = expectedViewpointSnapshotStream.ToArray();

                    Assert.False(string.IsNullOrWhiteSpace(actualViewpoint.Snapshot));
                    var actualViewpointEntry = actualArchive.Entries.FirstOrDefault(curr => curr.FullName == topicGuid + "/" + actualViewpoint.Snapshot);
                    Assert.NotNull(actualViewpointEntry);
                    var actualViewpointSnapshotStream = new MemoryStream();
                    actualViewpointEntry.Open().CopyTo(actualViewpointSnapshotStream);
                    var actualViewpointSnapshotBinary = actualViewpointSnapshotStream.ToArray();

                    Assert.True(expectedViewpointSnapshotBinary.SequenceEqual(actualViewpointSnapshotBinary));
                }
                Assert.Equal(expectedViewpoint.Viewpoint, actualViewpoint.Viewpoint);
            }
        }

        public static void CompareBimSnippet(BimSnippet expected, BimSnippet actual)
        {
            Assert.Equal(expected.isExternal, actual.isExternal);
            Assert.Equal(expected.Reference, actual.Reference);
            Assert.Equal(expected.ReferenceSchema, actual.ReferenceSchema);
            Assert.Equal(expected.SnippetType, actual.SnippetType);
        }

        public static void CompareTopic(Topic expectedTopic, Topic actualTopic)
        {
            Assert.Equal(expectedTopic.AssignedTo, actualTopic.AssignedTo);

            // Compare Snippet
            if (TestCompareUtilities.BothNotNull(expectedTopic.BimSnippet, actualTopic.BimSnippet, "Markup.BimSnippet"))
            {
                CompareBimSnippet(expectedTopic.BimSnippet, actualTopic.BimSnippet);
            }

            // Compare document references
            if (TestCompareUtilities.BothNotNull(expectedTopic.DocumentReferences, actualTopic.DocumentReferences, "Markup.DocumentReferences"))
            {
                Assert.Equal(expectedTopic.DocumentReferences.Count, actualTopic.DocumentReferences.Count);

                foreach (var expectedDocumentReference in expectedTopic.DocumentReferences)
                {
                    // Find the matching document reference in the actual topic
                    var actualDocumentReference = actualTopic.DocumentReferences
                        .Where(curr => curr.Description == expectedDocumentReference.Description)
                        .Where(curr => curr.Guid == expectedDocumentReference.Guid)
                        .Where(curr => curr.isExternal == expectedDocumentReference.isExternal)
                        .Where(curr => curr.ReferencedDocument == expectedDocumentReference.ReferencedDocument)
                        .FirstOrDefault();
                    Assert.NotNull(actualDocumentReference);
                }
            }

            // Check labels
            if (TestCompareUtilities.BothNotNull(expectedTopic.Labels, actualTopic.Labels, "Markup.Labels"))
            {
                Assert.Equal(expectedTopic.Labels.Count, actualTopic.Labels.Count);
                foreach (var expectedLabel in expectedTopic.Labels)
                {
                    Assert.Contains(expectedLabel, actualTopic.Labels);
                }
            }

            // Check related topics
            if (TestCompareUtilities.BothNotNull(expectedTopic.RelatedTopics, actualTopic.RelatedTopics, "Markup.RelatedTopics"))
            {
                Assert.Equal(expectedTopic.RelatedTopics.Count, actualTopic.RelatedTopics.Count);
                foreach (var expectedRelatedTopic in expectedTopic.RelatedTopics)
                {
                    var actualRelatedTopic = actualTopic.RelatedTopics
                        .Where(curr => curr.Guid == expectedRelatedTopic.Guid)
                        .FirstOrDefault();
                    Assert.NotNull(actualRelatedTopic);
                }
            }

            Assert.Equal(expectedTopic.CreationAuthor, actualTopic.CreationAuthor);
            Assert.True((int) (expectedTopic.CreationDate - actualTopic.CreationDate).TotalSeconds < 5);
            Assert.Equal(expectedTopic.CreationDateSpecified, actualTopic.CreationDateSpecified);

            if (!(string.IsNullOrWhiteSpace(expectedTopic.Description) && string.IsNullOrWhiteSpace(actualTopic.Description)))
            {
                Assert.Equal(expectedTopic.Description, actualTopic.Description);
            }
            Assert.Equal(expectedTopic.Guid, actualTopic.Guid);

            if (expectedTopic.Index != actualTopic.Index)
            {
                if (!(expectedTopic.Index != null && expectedTopic.Index.Trim() == "0" && string.IsNullOrWhiteSpace(actualTopic.Index)
                      || actualTopic.Index != null && actualTopic.Index.Trim() == "0" && string.IsNullOrWhiteSpace(expectedTopic.Index)))
                {
                    Assert.True(false, "Index does not match");
                }
            }

            Assert.Equal(expectedTopic.ModifiedAuthor, actualTopic.ModifiedAuthor);
            Assert.Equal(expectedTopic.ModifiedDate, actualTopic.ModifiedDate);
            Assert.Equal(expectedTopic.ModifiedDateSpecified, actualTopic.ModifiedDateSpecified);
            Assert.Equal(expectedTopic.Priority, actualTopic.Priority);
            Assert.Equal(expectedTopic.ReferenceLink, actualTopic.ReferenceLink);
            Assert.Equal(expectedTopic.Title, actualTopic.Title);
            Assert.Equal(expectedTopic.TopicStatus, actualTopic.TopicStatus);
            Assert.Equal(expectedTopic.TopicType, actualTopic.TopicType);
        }

        public static void CompareComments(List<Comment> expectedComments, List<Comment> actualComments)
        {
            // Compare count
            Assert.Equal(expectedComments.Count, actualComments.Count);

            foreach (var expectedComment in expectedComments)
            {
                // Get actual comment
                var actualComment = actualComments.FirstOrDefault(curr => curr.Guid == expectedComment.Guid);
                Assert.NotNull(actualComment);
                CompareSingleComment(expectedComment, actualComment);
            }
        }

        public static void CompareSingleComment(Comment expectedComment, Comment actualComment)
        {
            Assert.Equal(expectedComment.Author, actualComment.Author);

            var commentTextMatches = expectedComment.Comment1 == actualComment.Comment1;
            if (!commentTextMatches)
            {
                // It's possible that there is no actual comment text but the XML file does have the <Comment> element with an empty value
                commentTextMatches = string.IsNullOrWhiteSpace(expectedComment.Comment1) && string.IsNullOrWhiteSpace(actualComment.Comment1);
            }
            Assert.True(commentTextMatches, "No match in comment: Comment1");

            Assert.Equal(expectedComment.Date, actualComment.Date);
            Assert.Equal(expectedComment.Guid, actualComment.Guid);
            Assert.Equal(expectedComment.ModifiedAuthor, actualComment.ModifiedAuthor);
            Assert.Equal(expectedComment.ModifiedDate, actualComment.ModifiedDate);
            Assert.Equal(expectedComment.ModifiedDateSpecified, actualComment.ModifiedDateSpecified);

            if (expectedComment.ShouldSerializeReplyToComment())
            {
                Assert.True(actualComment.ShouldSerializeReplyToComment());
                Assert.Equal(expectedComment.ReplyToComment.Guid, actualComment.ReplyToComment.Guid);
            }
            else
            {
                Assert.False(actualComment.ShouldSerializeReplyToComment());
            }

            Assert.Equal(expectedComment.Status, actualComment.Status);
            Assert.Equal(expectedComment.VerbalStatus, actualComment.VerbalStatus);

            if (expectedComment.ShouldSerializeViewpoint())
            {
                Assert.True(actualComment.ShouldSerializeViewpoint(), "No match in comment: Viewpoint");
                Assert.Equal(expectedComment.Viewpoint.Guid, actualComment.Viewpoint.Guid);
            }
            else
            {
                Assert.False(actualComment.ShouldSerializeViewpoint(), "No match in comment: Viewpoint");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="expectedViewpoints"></param>
        /// <param name="actualViewpoints"></param>
        /// <param name="originatesFromApiConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareViewpoints(IEnumerable<VisualizationInfo> expectedViewpoints, IEnumerable<VisualizationInfo> actualViewpoints, bool originatesFromApiConversion)
        {
            foreach (var expectedViewpoint in expectedViewpoints)
            {
                var actualViewpoint = actualViewpoints.FirstOrDefault(curr => curr.GUID == expectedViewpoint.GUID);
                Assert.NotNull(actualViewpoint);
                CompareSingleViewpoints(expectedViewpoint, actualViewpoint, originatesFromApiConversion);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="expectedViewpoint"></param>
        /// <param name="actualViewpoint"></param>
        /// <param name="originatesFromApiConversion">If true, Bitmaps are not compared since the API does not support them</param>
        public static void CompareSingleViewpoints(VisualizationInfo expectedViewpoint, VisualizationInfo actualViewpoint, bool originatesFromApiConversion)
        {
            Assert.Equal(expectedViewpoint.GUID, actualViewpoint.GUID);

            // Compare Bitmaps
            if (!originatesFromApiConversion && TestCompareUtilities.BothNotNull(expectedViewpoint.Bitmaps, actualViewpoint.Bitmaps, "Viewpoint.Bitmaps"))
            {
                Assert.Equal(expectedViewpoint.Bitmaps.Count, actualViewpoint.Bitmaps.Count);
                foreach (var expectedBitmap in expectedViewpoint.Bitmaps)
                {
                    var actualBitmap = actualViewpoint.Bitmaps
                        .Where(curr => curr.Bitmap == expectedBitmap.Bitmap)
                        .Where(curr => curr.Height == expectedBitmap.Height)
                        .Where(curr => curr.Location.X == expectedBitmap.Location.X)
                        .Where(curr => curr.Location.Y == expectedBitmap.Location.Y)
                        .Where(curr => curr.Location.Z == expectedBitmap.Location.Z)
                        .Where(curr => curr.Normal.X == expectedBitmap.Normal.X)
                        .Where(curr => curr.Normal.Y == expectedBitmap.Normal.Y)
                        .Where(curr => curr.Normal.Z == expectedBitmap.Normal.Z)
                        .Where(curr => curr.Reference == expectedBitmap.Reference)
                        .Where(curr => curr.Up.X == expectedBitmap.Up.X)
                        .Where(curr => curr.Up.Y == expectedBitmap.Up.Y)
                        .Where(curr => curr.Up.Z == expectedBitmap.Up.Z)
                        .FirstOrDefault();
                    Assert.NotNull(actualViewpoint);
                }
            }

            // Compare Components
            if (TestCompareUtilities.BothNotNull(expectedViewpoint.Components, actualViewpoint.Components, "Viewpoint.Components"))
            {
                if (expectedViewpoint.Components.Count == 1 && actualViewpoint.Components.Count == 1)
                {
                    var expectedComponent = expectedViewpoint.Components.First();
                    var actualComponent = actualViewpoint.Components.First();

                    Assert.Equal(expectedComponent.AuthoringToolId, actualComponent.AuthoringToolId);
                    Assert.True((expectedComponent.Color == null && actualComponent.Color == null) || expectedComponent.Color.SequenceEqual(actualComponent.Color), "Color");
                    Assert.Equal(expectedComponent.IfcGuid, actualComponent.IfcGuid);
                    Assert.Equal(expectedComponent.OriginatingSystem, actualComponent.OriginatingSystem);
                    Assert.Equal(expectedComponent.Selected, actualComponent.Selected);
                    Assert.Equal(expectedComponent.Visible, actualComponent.Visible);
                }
                else
                {
                    foreach (var currentComponent in expectedViewpoint.Components)
                    {
                        var actualComponent = actualViewpoint.Components
                            .Where(curr => curr.AuthoringToolId == currentComponent.AuthoringToolId)
                            .Where(curr => (curr.Color == null && currentComponent.Color == null) || curr.Color.SequenceEqual(currentComponent.Color))
                            .Where(curr => curr.IfcGuid == currentComponent.IfcGuid)
                            .Where(curr => curr.OriginatingSystem == currentComponent.OriginatingSystem)
                            .Where(curr => curr.Selected == currentComponent.Selected)
                            .Where(curr => curr.Visible == currentComponent.Visible)
                            .FirstOrDefault();

                        Assert.NotNull(actualComponent);
                    }
                }
            }

            // Compare Lines
            if (TestCompareUtilities.BothNotNull(expectedViewpoint.Lines, actualViewpoint.Lines, "Viewpoint.Lines"))
            {
                foreach (var expectedLine in expectedViewpoint.Lines)
                {
                    var actualLine = actualViewpoint.Lines
                        .Where(curr => curr.EndPoint.X == expectedLine.EndPoint.X)
                        .Where(curr => curr.EndPoint.Y == expectedLine.EndPoint.Y)
                        .Where(curr => curr.EndPoint.Z == expectedLine.EndPoint.Z)
                        .Where(curr => curr.StartPoint.X == expectedLine.StartPoint.X)
                        .Where(curr => curr.StartPoint.Y == expectedLine.StartPoint.Y)
                        .Where(curr => curr.StartPoint.Z == expectedLine.StartPoint.Z)
                        .FirstOrDefault();
                    Assert.NotNull(actualLine);
                }
            }

            // Compare Clipping planes
            if (TestCompareUtilities.BothNotNull(expectedViewpoint.ClippingPlanes, actualViewpoint.ClippingPlanes, "Viewpoint.ClippingPlanes"))
            {
                foreach (var expectedPlane in expectedViewpoint.ClippingPlanes)
                {
                    var actualPlane = actualViewpoint.ClippingPlanes
                        .Where(curr => curr.Direction.X == expectedPlane.Direction.X)
                        .Where(curr => curr.Direction.Y == expectedPlane.Direction.Y)
                        .Where(curr => curr.Direction.Z == expectedPlane.Direction.Z)
                        .Where(curr => curr.Location.X == expectedPlane.Location.X)
                        .Where(curr => curr.Location.Y == expectedPlane.Location.Y)
                        .Where(curr => curr.Location.Z == expectedPlane.Location.Z)
                        .FirstOrDefault();
                    Assert.NotNull(actualPlane);
                }
            }

            // Compare OrthogonalCamera
            if (TestCompareUtilities.BothNotNull(expectedViewpoint.OrthogonalCamera, actualViewpoint.OrthogonalCamera, "Viewpoint.OrthogonalCamera"))
            {
                CompareOrthogonalCameras(expectedViewpoint.OrthogonalCamera, actualViewpoint.OrthogonalCamera);
            }
            // Compare PerspectiveCamera
            if (TestCompareUtilities.BothNotNull(expectedViewpoint.PerspectiveCamera, actualViewpoint.PerspectiveCamera, "Viewpoint.PerspectiveCamera"))
            {
                ComparePerspectiveCameras(expectedViewpoint.PerspectiveCamera, actualViewpoint.PerspectiveCamera);
            }
        }

        public static void CompareOrthogonalCameras(OrthogonalCamera expected, OrthogonalCamera actual)
        {
            Assert.Equal(expected.ViewToWorldScale, actual.ViewToWorldScale);
            Assert.Equal(expected.CameraDirection.X, actual.CameraDirection.X);
            Assert.Equal(expected.CameraDirection.Y, actual.CameraDirection.Y);
            Assert.Equal(expected.CameraDirection.Z, actual.CameraDirection.Z);
            Assert.Equal(expected.CameraUpVector.X, actual.CameraUpVector.X);
            Assert.Equal(expected.CameraUpVector.Y, actual.CameraUpVector.Y);
            Assert.Equal(expected.CameraUpVector.Z, actual.CameraUpVector.Z);
            Assert.Equal(expected.CameraViewPoint.X, actual.CameraViewPoint.X);
            Assert.Equal(expected.CameraViewPoint.Y, actual.CameraViewPoint.Y);
            Assert.Equal(expected.CameraViewPoint.Z, actual.CameraViewPoint.Z);
        }

        public static void ComparePerspectiveCameras(PerspectiveCamera expected, PerspectiveCamera actual)
        {
            Assert.Equal(expected.FieldOfView, actual.FieldOfView);
            Assert.Equal(expected.CameraDirection.X, actual.CameraDirection.X);
            Assert.Equal(expected.CameraDirection.Y, actual.CameraDirection.Y);
            Assert.Equal(expected.CameraDirection.Z, actual.CameraDirection.Z);
            Assert.Equal(expected.CameraUpVector.X, actual.CameraUpVector.X);
            Assert.Equal(expected.CameraUpVector.Y, actual.CameraUpVector.Y);
            Assert.Equal(expected.CameraUpVector.Z, actual.CameraUpVector.Z);
            Assert.Equal(expected.CameraViewPoint.X, actual.CameraViewPoint.X);
            Assert.Equal(expected.CameraViewPoint.Y, actual.CameraViewPoint.Y);
            Assert.Equal(expected.CameraViewPoint.Z, actual.CameraViewPoint.Z);
        }

        public static void CompareViewpointBitmaps(Dictionary<VisualizationInfo, List<byte[]>> expectedBitmaps, Dictionary<VisualizationInfo, List<byte[]>> actualBitmaps)
        {
            foreach (var expectedBitmap in expectedBitmaps)
            {
                var actualBitmap = actualBitmaps[actualBitmaps.Keys.FirstOrDefault(curr => curr.GUID == expectedBitmap.Key.GUID)];
                foreach (var expectedBitmapSingleByte in expectedBitmap.Value)
                {
                    Assert.True(actualBitmap.Any(curr => curr.SequenceEqual(expectedBitmapSingleByte)), "Did not find matching bitmap binary data");
                }
            }
        }

        public static void CompareViewpointSnapshots(ReadOnlyDictionary<string, byte[]> expectedSnapshots, ReadOnlyDictionary<string, byte[]> actualSnapshots)
        {
            foreach (var expectedSnapshot in expectedSnapshots)
            {
                Assert.True(expectedSnapshot.Value.SequenceEqual(actualSnapshots[expectedSnapshot.Key]), "Did not find matching snapshot binary data");
            }
        }
    }
}