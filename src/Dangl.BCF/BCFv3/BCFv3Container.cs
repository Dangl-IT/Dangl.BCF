using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Dangl.BCF.BCFv3.Schemas;
using Version = Dangl.BCF.BCFv3.Schemas.Version;

namespace Dangl.BCF.BCFv3
{
    /// <summary>
    ///     Container class for a BCFv2.1 physical file
    /// </summary>
    public class BCFv3Container : BindableBase
    {
        private Project _bcfProject;
        private Version _bcfVersionInfo;
        private Dictionary<string, byte[]> _fileAttachments;
        private Extensions _projectExtensions;
        private ObservableCollection<BCFTopic> _topics;

        /// <summary>
        ///     Version information for the BCFv2. Read-Only
        /// </summary>
        public Version BcfVersionInfo => _bcfVersionInfo ?? (_bcfVersionInfo = new Version
        {
            VersionId = "3.0"
        });

        /// <summary>
        ///     BCF Project and project extensions information
        /// </summary>
        public Project BcfProject
        {
            get { return _bcfProject; }
            set { SetProperty(ref _bcfProject, value); }
        }

        /// <summary>
        /// Contains allowed property values for this project, e.g. a list of which topic types are allowed
        /// </summary>
        public Extensions ProjectExtensions
        {
            get { return _projectExtensions; }
            set { SetProperty(ref _projectExtensions, value); }
        }

        /// <summary>
        ///     Contains the BCFv2's single topics
        /// </summary>
        public ObservableCollection<BCFTopic> Topics
        {
            get
            {
                if (_topics == null)
                {
                    _topics = new ObservableCollection<BCFTopic>();
                }
                return _topics;
            }
        }

        /// <summary>
        /// Holds all raw byte arrays of the attachment files
        /// </summary>
        public Dictionary<string, byte[]> FileAttachments
        {
            get
            {
                if (_fileAttachments == null)
                {
                    _fileAttachments = new Dictionary<string, byte[]>();
                }
                return _fileAttachments;
            }
        }

        /// <summary>
        /// Returns the raw byte array of the attachment
        /// </summary>
        /// <param name="documentReference"></param>
        /// <returns></returns>
        public byte[] GetAttachmentForDocumentReference(DocumentReference documentReference)
        {
            if (documentReference.ItemElementName == ItemChoiceType.Url)
            {
                throw new ArgumentException("Reference is external");
            }

            return FileAttachments[GetFilenameFromReference(documentReference.Item)];
        }

        /// <summary>
        ///     Creates a BCFv2 zip archive
        /// </summary>
        /// <param name="streamToWrite"></param>
        public void WriteStream(Stream streamToWrite)
        {
            using (var bcfZip = new ZipArchive(streamToWrite, ZipArchiveMode.Create, true))
            {
                // Write the version information
                var versionInformation = bcfZip.CreateEntry("bcf.version");
                using (var versionWriter = new StreamWriter(versionInformation.Open()))
                {
                    var serializedVersionInfo = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(BcfVersionInfo.Serialize());
                    versionWriter.Write(serializedVersionInfo);
                }

                if (ProjectExtensions != null)
                {
                    var projectInformation = bcfZip.CreateEntry("extensions.xml");
                    using (var projectInfoWriter = new StreamWriter(projectInformation.Open()))
                    {
                        var serializedExtensions = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(ProjectExtensions.Serialize());
                        projectInfoWriter.Write(serializedExtensions);
                    }
                }
                // Write the project info if it is present
                if (BcfProject != null)
                {
                    var projectEntry = bcfZip.CreateEntry("project.bcfp");
                    using (var projectInfoWriter = new StreamWriter(projectEntry.Open()))
                    {
                        var serializedProjectInfo = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(BcfProject.Serialize());
                        projectInfoWriter.Write(serializedProjectInfo);
                    }
                }
                // Write file attachments
                if (FileAttachments.Any())
                {
                    foreach (var attachment in FileAttachments)
                    {
                        var attachmentEntry = bcfZip.CreateEntry(attachment.Key);
                        using (var attachmentWriter = new BinaryWriter(attachmentEntry.Open()))
                        {
                            attachmentWriter.Write(attachment.Value);
                        }
                    }
                }
                // Write an entry for each topic
                foreach (var topic in Topics)
                {
                    if (topic.Markup.Topic.CreationDate == default(DateTime))
                    {
                        topic.Markup.Topic.CreationDate = DateTime.UtcNow;
                    }
                    if (string.IsNullOrWhiteSpace(topic.Markup.Topic.Guid))
                    {
                        topic.Markup.Topic.Guid = Guid.NewGuid().ToString();
                    }
                    if (topic.Markup.Topic.ShouldSerializeBimSnippet())
                    {
                        // Write BIM Snippet if present in the file and internal
                        if (!topic.Markup.Topic.BimSnippet.IsExternal)
                        {
                            topic.Markup.Topic.BimSnippet.IsExternal = false;
                            var bimSnippetBinaryEntry = bcfZip.CreateEntry(topic.Markup.Topic.Guid + "/" + topic.Markup.Topic.BimSnippet.Reference);
                            using (var snippetWriter = new BinaryWriter(bimSnippetBinaryEntry.Open()))
                            {
                                if (topic.SnippetData != null)
                                {
                                    snippetWriter.Write(topic.SnippetData);
                                }
                            }
                        }
                    }
                    var topicEntry = bcfZip.CreateEntry(topic.Markup.Topic.Guid + "/" + "markup.bcf");
                    for (var i = 0; i < topic.Viewpoints.Count; i++)
                    {
                        if (topic.ViewpointSnapshots.ContainsKey(topic.Viewpoints[i].Guid))
                        {
                            topic.Markup.Topic.Viewpoints[i].Snapshot = "Snapshot_" + topic.Viewpoints[i].Guid + ".png";
                        }
                    }
                    using (var topicWriter = new StreamWriter(topicEntry.Open()))
                    {
                        var serializedTopic = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(topic.Markup.Serialize());
                        topicWriter.Write(serializedTopic);
                    }
                    // Write viewpoints if present
                    for (var i = 0; i < topic.Viewpoints.Count; i++)
                    {
                        var entryName = topic.Markup.Topic.Guid + "/" + topic.Markup.Topic.Viewpoints[i].Viewpoint;
                        var viewpoint = bcfZip.CreateEntry(entryName);

                        using (var viewpointWriter = new StreamWriter(viewpoint.Open()))
                        {
                            if (topic.Viewpoints[i].Bitmaps.Count > 0)
                            {
                                foreach (var bitmap in topic.Viewpoints[i].Bitmaps)
                                {
                                    bitmap.Reference = "Bitmap_" + Guid.NewGuid() + "." + (bitmap.Format == Schemas.BitmapFormat.jpg? "jpg" : "png");
                                }
                            }
                            var serializedViewpoint = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(topic.Viewpoints[i].Serialize());
                            viewpointWriter.Write(serializedViewpoint);
                        }
                        // Write snapshot if present
                        if (topic.ViewpointSnapshots.ContainsKey(topic.Viewpoints[i].Guid))
                        {
                            var snapshotEntryName = topic.Markup.Topic.Guid + "/" + topic.Markup.Topic.Viewpoints[i].Snapshot;
                            var viewpointSnapshot = bcfZip.CreateEntry(snapshotEntryName);
                            using (var viewpointSnapshotWriter = new BinaryWriter(viewpointSnapshot.Open()))
                            {
                                viewpointSnapshotWriter.Write(topic.ViewpointSnapshots[topic.Markup.Topic.Viewpoints[i].Guid]);
                            }
                        }
                        // Write bitmaps if present
                        if (topic.ViewpointBitmaps.ContainsKey(topic.Viewpoints[i]))
                        {
                            for (var j = 0; j < topic.ViewpointBitmaps[topic.Viewpoints[i]].Count; j++)
                            {
                                // It's a little bit hacky still....
                                var bitmapEntryName = topic.Markup.Topic.Guid + "/" + topic.Viewpoints[i].Bitmaps[j].Reference;
                                var viewpointBitmapEntry = bcfZip.CreateEntry(bitmapEntryName);
                                using (var viewpointBitmapWriter = new BinaryWriter(viewpointBitmapEntry.Open()))
                                {
                                    viewpointBitmapWriter.Write(topic.ViewpointBitmaps[topic.Viewpoints[i]][j]);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Reads a BCFv2 zip archive
        /// </summary>
        /// <param name="zipFileStream">The zip archive of the physical file</param>
        /// <returns></returns>
        public static BCFv3Container ReadStream(Stream zipFileStream)
        {
            var container = new BCFv3Container();
            var bcfZipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
            // Check if version info is compliant with this implementation (2.1)
            var versionEntry = bcfZipArchive.Entries.FirstOrDefault(e => string.Equals(e.FullName, "bcf.version", StringComparison.OrdinalIgnoreCase));
            if (versionEntry != null)
            {
                var fileVersionInfo = Version.Deserialize(versionEntry.Open());
                if (fileVersionInfo.VersionId != "3.0")
                {
                    throw new NotSupportedException("BCFzip version");
                }
            }
            // Get project info if present
            if (bcfZipArchive.Entries.Any(e => e.FullName == "project.bcfp"))
            {
                var deserializedProject = Project.Deserialize(bcfZipArchive.Entries.First(Entry => Entry.FullName == "project.bcfp").Open());
                if (!(string.IsNullOrWhiteSpace(deserializedProject.Name) && string.IsNullOrWhiteSpace(deserializedProject.ProjectId)))
                {
                    container.BcfProject = deserializedProject;
                }
            }
            // Get extensions if present
            if (bcfZipArchive.Entries.Any(e => e.FullName == "extensions.xml"))
            {
                var deserializedExtensions = Extensions.Deserialize(bcfZipArchive.Entries.First(Entry => Entry.FullName == "extensions.xml").Open());
                container.ProjectExtensions = deserializedExtensions;
            }
            // Get each topic's GUID and read the topic
            var topicIds = new List<string>();
            foreach (var entry in bcfZipArchive.Entries)
            {
                var topicId = entry.FullName;
                if (Regex.IsMatch(topicId, @"^\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}/markup.bcf\b"))
                {
                    if (!topicIds.Contains(Regex.Match(topicId, @"^\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b").Value))
                    {
                        container.Topics.Add(ReadSingleTopic(bcfZipArchive, Regex.Match(topicId, @"^\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b").Value, container));
                    }
                    topicIds.Add(Regex.Match(topicId, @"^\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b").Value);
                }
            }

            return container;
        }

        /// <summary>
        ///     Will take the current location within a <see cref="ZipArchive" /> and a relative location to that to output
        ///     the absolute location for the ZipArchive Entry.
        /// </summary>
        /// <param name="currentPath">Position within the archive from which to start.</param>
        /// <param name="relativeReference">Relative to the given position.</param>
        /// <returns></returns>
        public static string GetAbsolutePath(string currentPath, string relativeReference)
        {
            if (string.IsNullOrWhiteSpace(currentPath + relativeReference))
            {
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(currentPath))
            {
                return relativeReference;
            }
            if (string.IsNullOrWhiteSpace(relativeReference))
            {
                return currentPath;
            }
            var pathSegments = currentPath.Split('/');
            var referenceSegments = relativeReference.Split('/');
            var fullPathSegments = pathSegments.Concat(referenceSegments).Where(s => s != "/" && !string.IsNullOrWhiteSpace(s)).ToList();
            for (var i = 0; i < fullPathSegments.Count; i++)
            {
                // Need to get one higher
                if (fullPathSegments[i].Trim() == "..")
                {
                    fullPathSegments.RemoveAt(i);
                    fullPathSegments.RemoveAt(i - 1);
                    i = i - 2;
                }
            }
            return string.Join("/", fullPathSegments);
        }

        /// <summary>
        /// Returns just the filename portion of a file reference, e.g. "picture.jpg" for "C:\Pictures\picuture.jpg"
        /// </summary>
        /// <param name="fileReference"></param>
        /// <returns></returns>
        public static string GetFilenameFromReference(string fileReference)
        {
            return string.IsNullOrWhiteSpace(fileReference) ? string.Empty : fileReference.Split('/').Last();
        }

        /// <summary>
        ///     Transforms an absolute path to a relative path from a given location
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="currentLocation">The location from which to make the relative path</param>
        /// <returns></returns>
        public static string TransformToRelativePath(string absolutePath, string currentLocation)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(currentLocation))
            {
                return absolutePath;
            }
            // Nested within the current location
            if (absolutePath.StartsWith(currentLocation))
            {
                return absolutePath.Substring(currentLocation.Length + 1);
            }
            var absolutePathComponents = absolutePath.Split('/');
            var currentLocComponents = currentLocation.Split('/');

            int offset;
            for (offset = 0; offset < currentLocComponents.Length; offset++)
            {
                if (absolutePathComponents[offset] != currentLocComponents[offset])
                {
                    break;
                }
            }
            var result = string.Empty;
            for (var i = 0; i < currentLocComponents.Length - offset; i++)
            {
                result += "../";
            }
            for (var i = offset; i < absolutePathComponents.Length; i++)
            {
                result += absolutePathComponents[i] + "/";
            }
            return result.TrimEnd('/');
        }

        private static BCFTopic ReadSingleTopic(ZipArchive archive, string topicId, BCFv3Container container)
        {
            var topic = new BCFTopic();
            // Get the markup
            topic.Markup = Markup.Deserialize(archive.Entries.First(e => e.FullName == topicId + "/" + "markup.bcf").Open());
            // Check if any comments have a Viewpoint object without any value, then set it to null
            foreach (var comment in topic.Markup.Topic.Comments.Where(c => c.ShouldSerializeViewpoint() && string.IsNullOrWhiteSpace(c.Viewpoint.Guid)))
            {
                comment.Viewpoint = null;
            }
            if (topic.Markup.Topic.ShouldSerializeBimSnippet()  && !topic.Markup.Topic.BimSnippet.IsExternal)
            {
                // Read the snippet
                var snippetPathInArchive = GetAbsolutePath(topicId, topic.Markup.Topic.BimSnippet.Reference);
                var entry = archive.Entries.FirstOrDefault(Curr => Curr.FullName == snippetPathInArchive);
                if (entry == null)
                {
                    topic.Markup.Topic.BimSnippet.IsExternal = true;
                }
                else
                {
                    using (var memStream = new MemoryStream())
                    {
                        entry.Open().CopyTo(memStream);
                        topic.SnippetData = memStream.ToArray();
                    }
                }
            }
            // See if any internal header files are referenced
            if (topic.Markup.ShouldSerializeHeader() && topic.Markup.Header.Files.Any(h => !h.IsExternal))
            {
                foreach (var internalFile in topic.Markup.Header.Files.Where(h => !h.IsExternal))
                {
                    if (internalFile.Reference == null)
                    {
                        continue;
                    }
                    var filePathInArchive = GetAbsolutePath(topicId, internalFile.Reference);
                    var entry = archive.Entries.FirstOrDefault(e => e.FullName == filePathInArchive);
                    // Only append if not known already and the file is actually present
                    if (entry != null && !container.FileAttachments.ContainsKey(entry.Name))
                    {
                        using (var memStream = new MemoryStream())
                        {
                            entry.Open().CopyTo(memStream);
                            container.FileAttachments.Add(entry.Name, memStream.ToArray());
                        }
                    }
                }
            }
            // Get referenced documents
            if (topic.Markup.Topic.ShouldSerializeDocumentReferences() && topic.Markup.Topic.DocumentReferences.Any(d => d.ItemElementName != ItemChoiceType.Url))
            {
                foreach (var internalDocument in topic.Markup.Topic.DocumentReferences.Where(d => d.ItemElementName != ItemChoiceType.Url))
                {
                    var filePathInArchive = GetAbsolutePath(topicId, internalDocument.Item);
                    var entry = archive.Entries.FirstOrDefault(e => e.FullName == filePathInArchive);
                    // Only append if not known already and the file is actually present
                    if (entry != null && !container.FileAttachments.ContainsKey(entry.Name))
                    {
                        using (var memStream = new MemoryStream())
                        {
                            entry.Open().CopyTo(memStream);
                            container.FileAttachments.Add(entry.Name, memStream.ToArray());
                        }
                    }
                }
            }
            // Get viewpoints
            for (var i = 0; i < topic.Markup.Topic.Viewpoints.Count; i++)
            {
                var deserializedViewpoint = VisualizationInfo.Deserialize(archive.Entries.First(e => e.FullName == topicId + "/" + topic.Markup.Topic.Viewpoints[i].Viewpoint).Open());
                deserializedViewpoint.Guid = topic.Markup.Topic.Viewpoints[i].Guid;
                topic.Viewpoints.Add(deserializedViewpoint);
                // Get viewpoint bitmaps if present
                if (topic.Viewpoints[i].Bitmaps.Count > 0)
                {
                    foreach (var viewpointBitmap in topic.Viewpoints[i].Bitmaps)
                    {
                        using (var bytesMemoryStream = new MemoryStream())
                        {
                            var bitmapPathInArchive = GetAbsolutePath(topicId, viewpointBitmap.Reference);
                            var bitmapFileEntry = archive.Entries.FirstOrDefault(e => e.FullName == bitmapPathInArchive);
                            if (bitmapFileEntry == null)
                            {
                                // File entry was not found, possible because it was referenced with an absolute path
                                bitmapPathInArchive = GetAbsolutePath(topicId, TransformToRelativePath(viewpointBitmap.Reference, topicId));
                                bitmapFileEntry = archive.Entries.FirstOrDefault(e => e.FullName == bitmapPathInArchive);
                                if (bitmapFileEntry != null)
                                {
                                    viewpointBitmap.Reference = TransformToRelativePath(viewpointBitmap.Reference, topicId);
                                }
                                else
                                {
                                    throw new ArgumentNullException(nameof(bitmapFileEntry), "Could not locate bitmap file in archive");
                                }
                            }
                            bitmapFileEntry.Open().CopyTo(bytesMemoryStream);
                            if (!topic.ViewpointBitmaps.ContainsKey(topic.Viewpoints[i]))
                            {
                                topic.ViewpointBitmaps.Add(topic.Viewpoints[i], new List<byte[]>());
                            }
                            topic.ViewpointBitmaps[topic.Viewpoints[i]].Add(bytesMemoryStream.ToArray());
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(topic.Markup.Topic.Viewpoints[i].Snapshot))
                {
                    using (var bytesMemoryStream = new MemoryStream())
                    {
                        archive.Entries.First(e => e.FullName == topicId + "/" + topic.Markup.Topic.Viewpoints[i].Snapshot).Open().CopyTo(bytesMemoryStream);
                        topic.AddOrUpdateSnapshot(topic.Viewpoints[i].Guid, bytesMemoryStream.ToArray());
                    }
                }
            }
            return topic;
        }
    }
}