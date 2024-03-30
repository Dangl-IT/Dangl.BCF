using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Dangl.BCF.BCFv3;
using Dangl.BCF.BCFv3.Schemas;
using Xunit;
using System.Collections;
using System.Reflection;
using System.Xml.Linq;

namespace Dangl.BCF.Tests.BCFTestCases.v3
{
    public static class CompareTool
    {
        public static void CompareFiles(byte[] fileToImport,
            byte[] readAndWrittenFile,
            string[] filesToIgnore = null)
        {
            // Check that archive file contents are present
            ArchiveFilesCompareTool.CompareZipArchiveFileEntries(fileToImport, readAndWrittenFile, filesToIgnore);
            var expectedFile = BCFv3Container.ReadStream(new MemoryStream(fileToImport));
            var actualFile = BCFv3Container.ReadStream(new MemoryStream(readAndWrittenFile));
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
        public static void CompareContainers(BCFv3Container expectedContainer, BCFv3Container actualContainer, ZipArchive expectedArchive = null, ZipArchive actualArchive = null, bool originatesFromApiConversion = false)
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

        public static void CompareProjectExtensions(BCFv3Container expectedContainer, BCFv3Container actualContainer)
        {
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedContainer.ProjectExtensions, actualContainer.ProjectExtensions, "ProjectExtensions"))
            {
                CompareStringList(expectedContainer.ProjectExtensions.SnippetTypes, actualContainer.ProjectExtensions.SnippetTypes, "SnippetType");
                CompareStringList(expectedContainer.ProjectExtensions.Priorities, actualContainer.ProjectExtensions.Priorities, "Priority");
                CompareStringList(expectedContainer.ProjectExtensions.TopicStatuses, actualContainer.ProjectExtensions.TopicStatuses, "TopicStatus");
                CompareStringList(expectedContainer.ProjectExtensions.TopicTypes, actualContainer.ProjectExtensions.TopicTypes, "TopicType");
                CompareStringList(expectedContainer.ProjectExtensions.Users, actualContainer.ProjectExtensions.Users, "UserIdType");
                CompareStringList(expectedContainer.ProjectExtensions.TopicLabels, actualContainer.ProjectExtensions.TopicLabels, "TopicLabel");
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

        public static void CompareFileAttachments(BCFv3Container expectedContainer, BCFv3Container actualContainer)
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
        public static void CompareProjectAndVersion(BCFv3Container expectedContainer, BCFv3Container actualContainer)
        {
            // Compare project
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedContainer.BcfProject, actualContainer.BcfProject, "BCFProject"))
            {
                if (TestCompareUtilities.BothNotNullAndEmpty(expectedContainer.BcfProject, actualContainer.BcfProject, "BCFProject.Project"))
                {
                    Assert.Equal(expectedContainer.BcfProject.Name, actualContainer.BcfProject.Name);
                    Assert.Equal(expectedContainer.BcfProject.ProjectId, actualContainer.BcfProject.ProjectId);
                }
                Assert.Equal(expectedContainer.ProjectExtensions.Serialize(), actualContainer.ProjectExtensions.Serialize());
            }

            // Compare version
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedContainer.BcfVersionInfo, actualContainer.BcfVersionInfo, "BCFVersionInfo"))
            {
                if (expectedContainer.BcfVersionInfo.VersionId.Contains("3.0"))
                {
                    Assert.Contains("3.0", actualContainer.BcfVersionInfo.VersionId);
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
        public static bool BothNotNullAndEmpty(object expectedObject, object actualObject, string parameterName)
        {
            if (expectedObject == null && actualObject != null)
            {
                Assert.True(false, "Parameter: " + parameterName + "; Expected is null but actual is present.");
            }
            else if (expectedObject != null && actualObject == null)
            {
                Assert.True(false, "Parameter: " + parameterName + "; Actual is null but Expected is present.");
            }
            if (expectedObject != null && expectedObject is IEnumerable && actualObject != null && actualObject is IEnumerable)
            {
                if (((IEnumerable<object>) expectedObject).Count() != ((IEnumerable<object>) actualObject).Count())
                {
                    Assert.True(false, "Parameter: " + parameterName + "; Actual and Expected have different count of values");
                }
                return ((IEnumerable<object>) expectedObject).Count() != 0;
            }
            return expectedObject != null;
        }
    }

    public static class ArchiveFilesCompareTool
    {
        public static void CompareZipArchiveFileEntries(byte[] fileToImport,
            byte[] readAndWrittenFile,
            string[] filesToIgnore = null)
        {
            var expectedZipArchive = new ZipArchive(new MemoryStream(fileToImport));
            var actualZipArchive = new ZipArchive(new MemoryStream(readAndWrittenFile));

            // Check that all expected files are present
            foreach (var currentExpectedFile in expectedZipArchive.Entries)
            {
                var filePresent = actualZipArchive.Entries.Any(e => e.FullName == currentExpectedFile.FullName);
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


                    var viewpointParents = expectedMarkupXml.Descendants("Viewpoints");
                    var viewpoints = viewpointParents.SelectMany(v => v.Descendants())
                        .Where(d => d.Name.LocalName.Equals("Viewpoint", System.StringComparison.InvariantCultureIgnoreCase));
                    var viewpointsWithGuidAttributes = viewpoints.Where(e => e.Attribute("Guid") != null);
                    var viewpointGuid = viewpointsWithGuidAttributes.FirstOrDefault().Attribute("Guid").Value;

                    // Get viewpoint reference for this viewpoint in the created archive
                    var actualMarkupXml = XmlUtilities.GetElementFromZipFile(actualZipArchive, folder + "markup.bcf");
                    var actualViewpointReference = actualMarkupXml
                        .Descendants()
                        .Where(d => d.Name.LocalName.Equals("Viewpoint", System.StringComparison.InvariantCultureIgnoreCase)
                            && d.Attributes().Any(a => a.Name.LocalName == "Guid")
                            && d.Attribute("Guid").Value == viewpointGuid)
                        .Elements()
                        .FirstOrDefault(e => e.Name.LocalName == "Viewpoint")
                        .Value;

                    filePresent = actualZipArchive.Entries.Any(curr => curr.FullName == folder + actualViewpointReference);
                }
                if (!filePresent && currentExpectedFile.FullName.ToUpperInvariant().EndsWith("project.bcfp".ToUpperInvariant()))
                {
                    // Originating file has maybe an empty project.bcfp, check this and do not enforce file availability then.
                    var expectedProjectXml = XmlUtilities.GetElementFromZipFile(expectedZipArchive, currentExpectedFile.FullName);

                    var projectShouldBeTreatedAsEmpty =
                        (!expectedProjectXml.Descendants("ExtensionSchema").Any() || expectedProjectXml.Descendants("ExtensionSchema").All(curr => string.IsNullOrWhiteSpace(curr.Value))) // ExtensionSchema not present or empty
                        && (!expectedProjectXml.Descendants("Project").Any() || expectedProjectXml.Descendants("Project").All(d => d.Attribute("ProjectId") == null || string.IsNullOrWhiteSpace(d.Attribute("ProjectId").Value))) // Project Id not present or empty
                        && (!expectedProjectXml.Descendants("Name").Where(d => d.Parent.Name.LocalName == "Project").Any() || expectedProjectXml.Descendants("Name").Where(d => d.Parent.Name.LocalName == "Project").All(d => string.IsNullOrWhiteSpace(d.Value))); // Project Name not present or empty
                    if (projectShouldBeTreatedAsEmpty)
                    {
                        filePresent = true;
                    }
                }

                if (!filePresent)
                {
                    filePresent = currentExpectedFile.Length == 0; // Empty entry, then dont look for it
                }

                if (!filePresent && !(filesToIgnore?.Contains(currentExpectedFile.Name) ?? false))
                {
                    Assert.False(true, "File: \"" + currentExpectedFile.FullName + "\" not present in created archive.");
                }
            }
        }

        public static bool FileIsPresentInSameFolderWithDifferentName(ZipArchive expectedArchive, ZipArchive actualArchive, string expectedEntryFullName)
        {
            var expectedEntry = expectedArchive.Entries.FirstOrDefault(e => e.FullName == expectedEntryFullName);
            var folder = expectedEntryFullName.Substring(0, expectedEntryFullName.Length - expectedEntryFullName.Split('/').Last().Length);

            var sameFileInFolder = actualArchive.Entries
                .Where(e => e.FullName.StartsWith(folder))
                .Where(e => !e.FullName.Substring(folder.Length).Contains('/'))
                .Where(e => e.Length == expectedEntry.Length)
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
        public static void CompareAllTopics(BCFv3Container expectedContainer, BCFv3Container actualContainer, ZipArchive expectedArchive, ZipArchive actualArchive, bool originatesFromApiConversion)
        {
            foreach (var expectedTopic in expectedContainer.Topics)
            {
                // Make sure topic is present only one
                Assert.Single(actualContainer.Topics.Where(t => t.Markup.Topic.Guid == expectedTopic.Markup.Topic.Guid));
                var actualTopic = actualContainer.Topics.FirstOrDefault(t => t.Markup.Topic.Guid == expectedTopic.Markup.Topic.Guid);
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
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedMarkup.Header, actualMarkup.Header, "Markup.Header"))
            {
                foreach (var currentHeaderEntry in expectedMarkup.Header.Files)
                {
                    var foundExactlyOneMatchignEntryInActual = actualMarkup.Header
                        .Files
                        .Where(h => h.Date == currentHeaderEntry.Date)
                        .Where(h => h.Filename == currentHeaderEntry.Filename)
                        .Where(h => h.IfcProject == currentHeaderEntry.IfcProject)
                        .Where(h => h.IfcSpatialStructureElement == currentHeaderEntry.IfcSpatialStructureElement)
                        .Where(h => h.IsExternal == currentHeaderEntry.IsExternal)
                        .Where(h => h.Reference == currentHeaderEntry.Reference)
                        .Count() == 1;

                    Assert.True(foundExactlyOneMatchignEntryInActual, "Found not matching header entry in actual file.");
                }

                // Check that the both sections contain the same number of entries
                Assert.Equal(expectedMarkup.Header.Files.Count, actualMarkup.Header.Files.Count);
            }

            CompareComments(expectedMarkup.Topic.Comments, actualMarkup.Topic.Comments);

            if (TestCompareUtilities.BothNotNullAndEmpty(expectedMarkup.Topic.Viewpoints, actualMarkup.Topic.Viewpoints, "Markup.Viewpoints"))
            {
                CompareViewpoints(expectedMarkup.Topic.Viewpoints, actualMarkup.Topic.Viewpoints, expectedArchive, actualArchive, expectedMarkup.Topic.Guid);
            }

            if (TestCompareUtilities.BothNotNullAndEmpty(expectedMarkup.Topic, actualMarkup.Topic, "Markup.Topic"))
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
                var actualViewpoint = actualViewpoints.FirstOrDefault(v => v.Guid == expectedViewpoint.Guid);

                Assert.Equal(expectedViewpoint.Guid, actualViewpoint.Guid);
                if (expectedViewpoint.Snapshot != actualViewpoint.Snapshot)
                {
                    // This means the file reference to the snapshot is different; need to check it then in binary format
                    var expectedViewpointSnapshotStream = new MemoryStream();
                    expectedArchive.Entries.FirstOrDefault(e => e.FullName == topicGuid + "/" + expectedViewpoint.Snapshot).Open().CopyTo(expectedViewpointSnapshotStream);
                    var expectedViewpointSnapshotBinary = expectedViewpointSnapshotStream.ToArray();

                    Assert.False(string.IsNullOrWhiteSpace(actualViewpoint.Snapshot));
                    var actualViewpointEntry = actualArchive.Entries.FirstOrDefault(e => e.FullName == topicGuid + "/" + actualViewpoint.Snapshot);
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
            Assert.Equal(expected.IsExternal, actual.IsExternal);
            Assert.Equal(expected.Reference, actual.Reference);
            Assert.Equal(expected.ReferenceSchema, actual.ReferenceSchema);
            Assert.Equal(expected.SnippetType, actual.SnippetType);
        }

        public static void CompareTopic(Topic expectedTopic, Topic actualTopic)
        {
            Assert.Equal(expectedTopic.AssignedTo, actualTopic.AssignedTo);

            // Compare Snippet
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedTopic.BimSnippet, actualTopic.BimSnippet, "Markup.BimSnippet"))
            {
                CompareBimSnippet(expectedTopic.BimSnippet, actualTopic.BimSnippet);
            }

            // Compare document references
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedTopic.DocumentReferences, actualTopic.DocumentReferences, "Markup.DocumentReferences"))
            {
                Assert.Equal(expectedTopic.DocumentReferences.Count, actualTopic.DocumentReferences.Count);

                foreach (var expectedDocumentReference in expectedTopic.DocumentReferences)
                {
                    // Find the matching document reference in the actual topic
                    var actualDocumentReference = actualTopic.DocumentReferences
                        .Where(d => d.Description == expectedDocumentReference.Description)
                        .Where(d => d.Guid == expectedDocumentReference.Guid)
                        .Where(d => d.Item == expectedDocumentReference.Item)
                        .Where(d => d.ItemElementName == expectedDocumentReference.ItemElementName)
                        .FirstOrDefault();
                    Assert.NotNull(actualDocumentReference);
                }
            }

            // Check labels
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedTopic.Labels, actualTopic.Labels, "Markup.Labels"))
            {
                Assert.Equal(expectedTopic.Labels.Count, actualTopic.Labels.Count);
                foreach (var expectedLabel in expectedTopic.Labels)
                {
                    Assert.Contains(expectedLabel, actualTopic.Labels);
                }
            }

            // Check related topics
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedTopic.RelatedTopics, actualTopic.RelatedTopics, "Markup.RelatedTopics"))
            {
                Assert.Equal(expectedTopic.RelatedTopics.Count, actualTopic.RelatedTopics.Count);
                foreach (var expectedRelatedTopic in expectedTopic.RelatedTopics)
                {
                    var actualRelatedTopic = actualTopic.RelatedTopics
                        .Where(r => r.Guid == expectedRelatedTopic.Guid)
                        .FirstOrDefault();
                    Assert.NotNull(actualRelatedTopic);
                }
            }

            Assert.Equal(expectedTopic.CreationAuthor, actualTopic.CreationAuthor);
            Assert.True((int) (expectedTopic.CreationDate - actualTopic.CreationDate).TotalSeconds < 5);
            Assert.True(expectedTopic.ShouldSerializeCreationDate());
            Assert.True(actualTopic.ShouldSerializeCreationDate());

            if (!(string.IsNullOrWhiteSpace(expectedTopic.Description) && string.IsNullOrWhiteSpace(actualTopic.Description)))
            {
                Assert.Equal(expectedTopic.Description, actualTopic.Description);
            }
            Assert.Equal(expectedTopic.Guid, actualTopic.Guid);

            if (expectedTopic.Index != actualTopic.Index)
            {
                Assert.True(false, "Index does not match");
            }

            Assert.Equal(expectedTopic.ModifiedAuthor, actualTopic.ModifiedAuthor);
            Assert.Equal(expectedTopic.ModifiedDate, actualTopic.ModifiedDate);
            Assert.Equal(expectedTopic.ModifiedDateSpecified, actualTopic.ModifiedDateSpecified);
            Assert.Equal(expectedTopic.Priority, actualTopic.Priority);
            Assert.Equal(expectedTopic.ReferenceLinks, actualTopic.ReferenceLinks);
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
                var actualComment = actualComments.FirstOrDefault(c => c.Guid == expectedComment.Guid);
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

            if (expectedComment.ShouldSerializeViewpoint() && expectedComment.Viewpoint.Guid != null)
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
                var actualViewpoint = actualViewpoints.FirstOrDefault(v => v.Guid == expectedViewpoint.Guid);
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
            Assert.Equal(expectedViewpoint.Guid, actualViewpoint.Guid);

            // Compare Bitmaps
            if (!originatesFromApiConversion && TestCompareUtilities.BothNotNullAndEmpty(expectedViewpoint.Bitmaps, actualViewpoint.Bitmaps, "Viewpoint.Bitmaps"))
            {
                Assert.Equal(expectedViewpoint.Bitmaps.Count, actualViewpoint.Bitmaps.Count);
                foreach (var expectedBitmap in expectedViewpoint.Bitmaps)
                {
                    var actualBitmap = actualViewpoint.Bitmaps
                        .Where(b => b.Format == expectedBitmap.Format)
                        .Where(b => b.Height == expectedBitmap.Height)
                        .Where(b => b.Location.X == expectedBitmap.Location.X)
                        .Where(b => b.Location.Y == expectedBitmap.Location.Y)
                        .Where(b => b.Location.Z == expectedBitmap.Location.Z)
                        .Where(b => b.Normal.X == expectedBitmap.Normal.X)
                        .Where(b => b.Normal.Y == expectedBitmap.Normal.Y)
                        .Where(b => b.Normal.Z == expectedBitmap.Normal.Z)
                        .Where(b => b.Reference == expectedBitmap.Reference)
                        .Where(b => b.Up.X == expectedBitmap.Up.X)
                        .Where(b => b.Up.Y == expectedBitmap.Up.Y)
                        .Where(b => b.Up.Z == expectedBitmap.Up.Z)
                        .FirstOrDefault();
                    Assert.NotNull(actualViewpoint);
                }
            }

            // Compare Components
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedViewpoint.Components, actualViewpoint.Components, "Viewpoint.Components"))
            {
                CompareComponents(expectedViewpoint.Components, actualViewpoint.Components);
            }

            // Compare Lines
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedViewpoint.Lines, actualViewpoint.Lines, "Viewpoint.Lines"))
            {
                foreach (var expectedLine in expectedViewpoint.Lines)
                {
                    var actualLine = actualViewpoint.Lines
                        .Where(l => l.EndPoint.X == expectedLine.EndPoint.X)
                        .Where(l => l.EndPoint.Y == expectedLine.EndPoint.Y)
                        .Where(l => l.EndPoint.Z == expectedLine.EndPoint.Z)
                        .Where(l => l.StartPoint.X == expectedLine.StartPoint.X)
                        .Where(l => l.StartPoint.Y == expectedLine.StartPoint.Y)
                        .Where(l => l.StartPoint.Z == expectedLine.StartPoint.Z)
                        .FirstOrDefault();
                    Assert.NotNull(actualLine);
                }
            }

            // Compare Clipping planes
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedViewpoint.ClippingPlanes, actualViewpoint.ClippingPlanes, "Viewpoint.ClippingPlanes"))
            {
                foreach (var expectedPlane in expectedViewpoint.ClippingPlanes)
                {
                    var actualPlane = actualViewpoint.ClippingPlanes
                        .Where(c => c.Direction.X == expectedPlane.Direction.X)
                        .Where(c => c.Direction.Y == expectedPlane.Direction.Y)
                        .Where(c => c.Direction.Z == expectedPlane.Direction.Z)
                        .Where(c => c.Location.X == expectedPlane.Location.X)
                        .Where(c => c.Location.Y == expectedPlane.Location.Y)
                        .Where(c => c.Location.Z == expectedPlane.Location.Z)
                        .FirstOrDefault();
                    Assert.NotNull(actualPlane);
                }
            }

            // Compare OrthogonalCamera
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedViewpoint.Item as OrthogonalCamera, actualViewpoint.Item as OrthogonalCamera, "Viewpoint.OrthogonalCamera"))
            {
                CompareOrthogonalCameras(expectedViewpoint.Item as OrthogonalCamera, actualViewpoint.Item as OrthogonalCamera);
            }
            // Compare PerspectiveCamera
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedViewpoint.Item as PerspectiveCamera, actualViewpoint.Item as PerspectiveCamera, "Viewpoint.PerspectiveCamera"))
            {
                ComparePerspectiveCameras(expectedViewpoint.Item as PerspectiveCamera, actualViewpoint.Item as PerspectiveCamera);
            }
        }

        private static void CompareComponents(Components expectedComponents, Components actualComponents)
        {
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedComponents.Coloring, actualComponents.Coloring, "Viewpoint.Components.Coloring"))
            {
                Assert.Equal(expectedComponents.Coloring.Count, actualComponents.Coloring.Count);
                foreach (var expectedColoring in expectedComponents.Coloring)
                {
                    var actualColoring = actualComponents.Coloring.FirstOrDefault(c => c.Color == expectedColoring.Color);
                    Assert.NotNull(actualColoring);
                    CompareComponentsList(expectedColoring.Components, actualColoring.Components);
                }
                var allActualColorsAreExpected = actualComponents.Coloring.All(c => expectedComponents.Coloring.Any(ec => ec.Color == c.Color));
                Assert.True(allActualColorsAreExpected);
            }
            CompareComponentsList(expectedComponents.Selection, actualComponents.Selection);
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedComponents.Visibility?.ViewSetupHints, actualComponents.Visibility?.ViewSetupHints, "Viewpoint.Components.ViewSetupHints"))
            {
                var expectedViewSetupHints = expectedComponents.Visibility.ViewSetupHints;
                var actualViewSetupHints = actualComponents.Visibility.ViewSetupHints;
                Assert.Equal(expectedViewSetupHints.OpeningsVisible, actualViewSetupHints.OpeningsVisible);
                Assert.Equal(expectedViewSetupHints.SpaceBoundariesVisible, actualViewSetupHints.SpaceBoundariesVisible);
                Assert.Equal(expectedViewSetupHints.SpacesVisible, actualViewSetupHints.SpacesVisible);
            }
            if (TestCompareUtilities.BothNotNullAndEmpty(expectedComponents.Visibility, actualComponents.Visibility, "Viewpoint.Components.Visibility"))
            {
                var expectedVisibility = expectedComponents.Visibility;
                var actualVisibility = actualComponents.Visibility;
                Assert.Equal(expectedVisibility.DefaultVisibility, actualVisibility.DefaultVisibility);
                CompareComponentsList(expectedVisibility.Exceptions, actualVisibility.Exceptions);
            }
        }

        private static void CompareComponentsList(List<Component> expectedComponents, List<Component> actualComponents)
        {
            if (expectedComponents == null)
            {
                Assert.Null(actualComponents);
                return;
            }
            Assert.Equal(expectedComponents.Count, actualComponents.Count);
            foreach (var expectedComponent in expectedComponents)
            {
                var actualComponent = actualComponents.FirstOrDefault(c => c.IfcGuid == expectedComponent.IfcGuid);
                Assert.NotNull(actualComponent);
                Assert.Equal(expectedComponent.AuthoringToolId, actualComponent.AuthoringToolId);
                Assert.Equal(expectedComponent.IfcGuid, actualComponent.IfcGuid);
                Assert.Equal(expectedComponent.OriginatingSystem, actualComponent.OriginatingSystem);
            }
            var allActualComponentsAreExpected = actualComponents.All(c => expectedComponents.Any(ec => ec.IfcGuid == c.IfcGuid));
            Assert.True(allActualComponentsAreExpected);
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
                var actualBitmap = actualBitmaps[actualBitmaps.Keys.FirstOrDefault(b => b.Guid == expectedBitmap.Key.Guid)];
                foreach (var expectedBitmapSingleByte in expectedBitmap.Value)
                {
                    Assert.True(actualBitmap.Any(b => b.SequenceEqual(expectedBitmapSingleByte)), "Did not find matching bitmap binary data");
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