using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using iabi.BCF.BCFv2.Schemas;
using Version = iabi.BCF.BCFv2.Schemas.Version;

namespace iabi.BCF.BCFv2
{
    /// <summary>
    ///     Container class for a BCFv2 physical file
    /// </summary>
    public class BCFv2Container : BindableBase
    {
        private ProjectExtension _BCFProject;
        private Version _BCFVersionInfo;

        private Dictionary<string, byte[]> _FileAttachments;

        private Extensions_XSD _ProjectExtensions;

        private ObservableCollection<BCFTopic> _Topics;

        /// <summary>
        ///     Version information for the BCFv2. Read-Only
        /// </summary>
        public Version BCFVersionInfo
        {
            get
            {
                if (_BCFVersionInfo == null)
                {
                    _BCFVersionInfo = new Version();
                    // Trimming the "RC" (Release Candidate) from the documentation, this is final now=)
                    _BCFVersionInfo.DetailedVersion = "2.0";
                    _BCFVersionInfo.VersionId = "2.0";
                }
                return _BCFVersionInfo;
            }
        }

        /// <summary>
        ///     BCF Project and project extensions information
        /// </summary>
        public ProjectExtension BCFProject
        {
            get { return _BCFProject; }
            set { SetProperty(ref _BCFProject, value); }
        }

        public Extensions_XSD ProjectExtensions
        {
            get { return _ProjectExtensions; }
            set
            {
                if (SetProperty(ref _ProjectExtensions, value))
                {
                    if (value == null)
                    {
                        if (BCFProject == null)
                        {
                            BCFProject = new ProjectExtension();
                        }
                        BCFProject.ExtensionSchema = null;
                    }
                    else
                    {
                        if (BCFProject == null)
                        {
                            BCFProject = new ProjectExtension();
                        }
                        BCFProject.ExtensionSchema = "extensions.xsd";
                    }
                }
            }
        }

        /// <summary>
        ///     Contains the BCFv2's single topics
        /// </summary>
        public ObservableCollection<BCFTopic> Topics
        {
            get
            {
                if (_Topics == null)
                {
                    _Topics = new ObservableCollection<BCFTopic>();
                }
                return _Topics;
            }
        }

        public Dictionary<string, byte[]> FileAttachments
        {
            get
            {
                if (_FileAttachments == null)
                {
                    _FileAttachments = new Dictionary<string, byte[]>();
                }
                return _FileAttachments;
            }
        }

        public byte[] GetAttachmentForDocumentReference(TopicDocumentReferences Input)
        {
            if (Input.isExternal)
            {
                throw new ArgumentException("Reference is external");
            }
            return FileAttachments[GetFilenameFromReference(Input.ReferencedDocument)];
        }

        /// <summary>
        ///     Creates a BCFv2 zip archive
        /// </summary>
        /// <param name="StreamToWrite"></param>
        public void WriteStream(Stream StreamToWrite)
        {
            using (var BCFZip = new ZipArchive(StreamToWrite, ZipArchiveMode.Create, true))
            {
                // Write the version information
                var VersionInformation = BCFZip.CreateEntry("bcf.version");
                using (var VersionWriter = new StreamWriter(VersionInformation.Open()))
                {
                    VersionWriter.Write(BCFVersionInfo.Serialize());
                }

                if (ProjectExtensions != null && !ProjectExtensions.IsEmpty())
                {
                    var ProjectInformation = BCFZip.CreateEntry("extensions.xsd");
                    using (var ProjectInfoWriter = new StreamWriter(ProjectInformation.Open()))
                    {
                        ProjectInfoWriter.Write(ProjectExtensions.WriteExtension());
                    }
                    if (BCFProject == null)
                    {
                        BCFProject = new ProjectExtension();
                    }
                    BCFProject.ExtensionSchema = "extensions.xsd";
                }
                // Write the project info if it is present
                if (BCFProject != null)
                {
                    var ProjectEntry = BCFZip.CreateEntry("project.bcfp");
                    using (var ProjectInfoWriter = new StreamWriter(ProjectEntry.Open()))
                    {
                        ProjectInfoWriter.Write(BCFProject.Serialize());
                    }
                }
                // Write file attachments
                if (FileAttachments.Any())
                {
                    foreach (var CurrentAttachment in FileAttachments)
                    {
                        var AttachmentEntry = BCFZip.CreateEntry(CurrentAttachment.Key);
                        using (var AttachmentWriter = new BinaryWriter(AttachmentEntry.Open()))
                        {
                            AttachmentWriter.Write(CurrentAttachment.Value);
                        }
                    }
                }
                // Write an entry for each topic
                foreach (var CurrentTopic in Topics)
                {
                    if (!CurrentTopic.Markup.Topic.CreationDateSpecified)
                    {
                        CurrentTopic.Markup.Topic.CreationDate = DateTime.UtcNow;
                    }
                    if (string.IsNullOrWhiteSpace(CurrentTopic.Markup.Topic.Guid))
                    {
                        CurrentTopic.Markup.Topic.Guid = Guid.NewGuid().ToString();
                    }
                    if (CurrentTopic.Markup.Topic.ShouldSerializeBimSnippet())
                    {
                        // Write BIM Snippet if present in the file and internal
                        if (!CurrentTopic.Markup.Topic.BimSnippet.isExternal)
                        {
                            CurrentTopic.Markup.Topic.BimSnippet.isExternal = false;
                            var BIMSnippetBinaryEntry = BCFZip.CreateEntry(CurrentTopic.Markup.Topic.Guid + "/" + CurrentTopic.Markup.Topic.BimSnippet.Reference);
                            using (var CurrentSnippetWriter = new BinaryWriter(BIMSnippetBinaryEntry.Open()))
                            {
                                if (CurrentTopic.SnippetData != null)
                                {
                                    CurrentSnippetWriter.Write(CurrentTopic.SnippetData);
                                }
                            }
                        }
                    }
                    var CurrentTopicEntry = BCFZip.CreateEntry(CurrentTopic.Markup.Topic.Guid + "/" + "markup.bcf");
                    for (var i = 0; i < CurrentTopic.Viewpoints.Count; i++)
                    {
                        if (CurrentTopic.ViewpointSnapshots.ContainsKey(CurrentTopic.Viewpoints[i].GUID))
                        {
                            CurrentTopic.Markup.Viewpoints[i].Snapshot = "Snapshot_" + CurrentTopic.Viewpoints[i].GUID + ".png";
                        }
                    }
                    using (var TopicWriter = new StreamWriter(CurrentTopicEntry.Open()))
                    {
                        TopicWriter.Write(CurrentTopic.Markup.Serialize());
                    }
                    // Write viewpoints if present
                    for (var i = 0; i < CurrentTopic.Viewpoints.Count; i++)
                    {
                        var EntryName = CurrentTopic.Markup.Topic.Guid + "/" + CurrentTopic.Markup.Viewpoints[i].Viewpoint;
                        var CurrentViewpoint = BCFZip.CreateEntry(EntryName);

                        using (var CurrentViewpointWriter = new StreamWriter(CurrentViewpoint.Open()))
                        {
                            if (CurrentTopic.Viewpoints[i].Bitmaps.Count > 0)
                            {
                                foreach (var CurrentBitmap in CurrentTopic.Viewpoints[i].Bitmaps)
                                {
                                    CurrentBitmap.Reference = "Bitmap_" + Guid.NewGuid() + "." + (CurrentBitmap.Bitmap == BitmapFormat.JPG ? "jpg" : "png");
                                }
                            }
                            CurrentViewpointWriter.Write(CurrentTopic.Viewpoints[i].Serialize());
                        }
                        // Write snapshot if present
                        if (CurrentTopic.ViewpointSnapshots.ContainsKey(CurrentTopic.Viewpoints[i].GUID))
                        {
                            var SnapshotEntryName = CurrentTopic.Markup.Topic.Guid + "/" + CurrentTopic.Markup.Viewpoints[i].Snapshot;
                            var CurrentViewpointSnapshot = BCFZip.CreateEntry(SnapshotEntryName);
                            using (var CurrentViewpointSnapshotWriter = new BinaryWriter(CurrentViewpointSnapshot.Open()))
                            {
                                CurrentViewpointSnapshotWriter.Write(CurrentTopic.ViewpointSnapshots[CurrentTopic.Markup.Viewpoints[i].Guid]);
                            }
                        }
                        // Write bitmaps if present
                        if (CurrentTopic.ViewpointBitmaps.ContainsKey(CurrentTopic.Viewpoints[i]))
                        {
                            for (var j = 0; j < CurrentTopic.ViewpointBitmaps[CurrentTopic.Viewpoints[i]].Count; j++)
                            {
                                // It's a little bit hacky still....
                                var BitmapEntryName = CurrentTopic.Markup.Topic.Guid + "/" + CurrentTopic.Viewpoints[i].Bitmaps[j].Reference;
                                var CurrentViewpointSnapshot = BCFZip.CreateEntry(BitmapEntryName);
                                using (var CurrentViewpointSnapshotWriter = new BinaryWriter(CurrentViewpointSnapshot.Open()))
                                {
                                    CurrentViewpointSnapshotWriter.Write(CurrentTopic.ViewpointBitmaps[CurrentTopic.Viewpoints[i]][j]);
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
        /// <param name="ZIPFileStream">The zip archive of the physical file</param>
        /// <returns></returns>
        public static BCFv2Container ReadStream(Stream ZIPFileStream)
        {
            var ReturnObject = new BCFv2Container();
            var FileToOpen = new ZipArchive(ZIPFileStream, ZipArchiveMode.Read);
            // Check if version info is compliant with this implementation (2.0)
            var VersionEntry = FileToOpen.Entries.FirstOrDefault(Entry => string.Equals(Entry.FullName, "bcf.version", StringComparison.InvariantCultureIgnoreCase));
            if (VersionEntry != null)
            {
                var ReadFileVersionInfo = Version.Deserialize(VersionEntry.Open());
                if (ReadFileVersionInfo.VersionId != "2.0" || !ReadFileVersionInfo.DetailedVersion.Contains("2.0"))
                {
                    throw new NotSupportedException("BCFzip version");
                }
            }
            // Get project info if present
            if (FileToOpen.Entries.Any(Entry => Entry.FullName == "project.bcfp"))
            {
                var DeserializedProject = ProjectExtension.Deserialize(FileToOpen.Entries.First(Entry => Entry.FullName == "project.bcfp").Open());
                if (!(string.IsNullOrWhiteSpace(DeserializedProject.ExtensionSchema) && (DeserializedProject.Project == null || string.IsNullOrWhiteSpace(DeserializedProject.Project.Name) && string.IsNullOrWhiteSpace(DeserializedProject.Project.ProjectId))))
                {
                    if (string.IsNullOrWhiteSpace(DeserializedProject.ExtensionSchema))
                    {
                        DeserializedProject.ExtensionSchema = null;
                    }
                    ReturnObject.BCFProject = DeserializedProject;
                    if (!string.IsNullOrWhiteSpace(ReturnObject.BCFProject.ExtensionSchema) && FileToOpen.Entries.Any(Curr => Curr.FullName == ReturnObject.BCFProject.ExtensionSchema))
                    {
                        using (var Rdr = new StreamReader(FileToOpen.Entries.FirstOrDefault(Curr => Curr.FullName == ReturnObject.BCFProject.ExtensionSchema).Open()))
                        {
                            ReturnObject.ProjectExtensions = new Extensions_XSD(Rdr.ReadToEnd());
                        }
                    }
                }
            }
            // Get each topic's GUID and read the topic
            var TopicGUIDs = new List<string>();
            foreach (var CurrentEntry in FileToOpen.Entries)
            {
                var CurrentTopicGUID = CurrentEntry.FullName;
                if (Regex.IsMatch(CurrentTopicGUID, @"^\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}/markup.bcf\b"))
                {
                    if (!TopicGUIDs.Contains(Regex.Match(CurrentTopicGUID, @"^\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b").Value))
                    {
                        ReturnObject.Topics.Add(ReadSingleTopic(FileToOpen, Regex.Match(CurrentTopicGUID, @"^\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b").Value, ReturnObject));
                    }
                }
            }
            // Check if there is no extension but a reference to it; delete the reference then
            if (ReturnObject.ProjectExtensions == null && ReturnObject.BCFProject != null && !string.IsNullOrWhiteSpace(ReturnObject.BCFProject.ExtensionSchema))
            {
                ReturnObject.BCFProject.ExtensionSchema = null;
            }
            return ReturnObject;
        }

        /// <summary>
        ///     Will take the current location within a <see cref="ZipArchive" /> and a relative location to that to output
        ///     the absolute location for the ZipArchive Entry.
        /// </summary>
        /// <param name="CurrentPath">Position within the archive from which to start.</param>
        /// <param name="RelativeReference">Relative to the given position.</param>
        /// <returns></returns>
        public static string GetAbsolutePath(string CurrentPath, string RelativeReference)
        {
            if (string.IsNullOrWhiteSpace(CurrentPath + RelativeReference))
            {
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(CurrentPath))
            {
                return RelativeReference;
            }
            if (string.IsNullOrWhiteSpace(RelativeReference))
            {
                return CurrentPath;
            }
            var PathSegments = CurrentPath.Split('/');
            var ReferenceSegments = RelativeReference.Split('/');
            var FullPathSegments = PathSegments.Concat(ReferenceSegments).Where(Curr => Curr != "/" && !string.IsNullOrWhiteSpace(Curr)).ToList();
            for (var i = 0; i < FullPathSegments.Count; i++)
            {
                // Need to get one higher
                if (FullPathSegments[i].Trim() == "..")
                {
                    FullPathSegments.RemoveAt(i);
                    FullPathSegments.RemoveAt(i - 1);
                    i = i - 2;
                }
            }
            return string.Join("/", FullPathSegments);
        }

        public static string GetFilenameFromReference(string FileReference)
        {
            return string.IsNullOrWhiteSpace(FileReference) ? string.Empty : FileReference.Split('/').Last();
        }

        /// <summary>
        ///     Transforms an absolute path to a relative path from a given location
        /// </summary>
        /// <param name="AbsolutePath"></param>
        /// <param name="CurrentLocation">The location from which to make the relative path</param>
        /// <returns></returns>
        public static string TransformToRelativePath(string AbsolutePath, string CurrentLocation)
        {
            if (string.IsNullOrWhiteSpace(AbsolutePath))
            {
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(CurrentLocation))
            {
                return AbsolutePath;
            }
            // Nested within the current location
            if (AbsolutePath.StartsWith(CurrentLocation))
            {
                return AbsolutePath.Substring(CurrentLocation.Length + 1);
            }
            var AbsolutePathComponents = AbsolutePath.Split('/');
            var CurrentLocComponents = CurrentLocation.Split('/');

            int Offset;
            for (Offset = 0; Offset < CurrentLocComponents.Length; Offset++)
            {
                if (AbsolutePathComponents[Offset] != CurrentLocComponents[Offset])
                {
                    break;
                }
            }
            var Result = string.Empty;
            for (var i = 0; i < CurrentLocComponents.Length - Offset; i++)
            {
                Result += "../";
            }
            for (var i = Offset; i < AbsolutePathComponents.Length; i++)
            {
                Result += AbsolutePathComponents[i] + "/";
            }
            return Result.TrimEnd('/');
        }

        private static BCFTopic ReadSingleTopic(ZipArchive Archive, string TopicID, BCFv2Container Container)
        {
            var ReturnObject = new BCFTopic();
            // Get the markup
            ReturnObject.Markup = Markup.Deserialize(Archive.Entries.First(Entry => Entry.FullName == TopicID + "/" + "markup.bcf").Open());
            // Check if any comments have a Viewpoint object without any value, then set it to null
            foreach (var CurrentComment in ReturnObject.Markup.Comment.Where(Curr => Curr.ShouldSerializeViewpoint() && string.IsNullOrWhiteSpace(Curr.Viewpoint.Guid)))
            {
                CurrentComment.Viewpoint = null;
            }
            if (ReturnObject.Markup.Topic.ShouldSerializeBimSnippet() && !ReturnObject.Markup.Topic.BimSnippet.isExternal)
            {
                // Read the snippet
                var SnippetPathInArchive = GetAbsolutePath(TopicID, ReturnObject.Markup.Topic.BimSnippet.Reference);
                var Entry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == SnippetPathInArchive);
                if (Entry == null)
                {
                    ReturnObject.Markup.Topic.BimSnippet.isExternal = true;
                }
                else
                {
                    using (var MemStream = new MemoryStream())
                    {
                        Entry.Open().CopyTo(MemStream);
                        ReturnObject.SnippetData = MemStream.ToArray();
                    }
                }
            }
            // See if any internal header files are referenced
            if (ReturnObject.Markup.ShouldSerializeHeader() && ReturnObject.Markup.Header.Any(Curr => !Curr.isExternal))
            {
                foreach (var InternalFile in ReturnObject.Markup.Header.Where(Curr => !Curr.isExternal))
                {
                    var FilePathInArchive = GetAbsolutePath(TopicID, InternalFile.Reference);
                    var Entry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == FilePathInArchive);
                    // Only append if not known already
                    if (!Container.FileAttachments.ContainsKey(Entry.Name))
                    {
                        using (var MemStream = new MemoryStream())
                        {
                            Entry.Open().CopyTo(MemStream);
                            Container.FileAttachments.Add(Entry.Name, MemStream.ToArray());
                        }
                    }
                }
            }
            // Get referenced documents
            if (ReturnObject.Markup.Topic.ShouldSerializeDocumentReferences() && ReturnObject.Markup.Topic.DocumentReferences.Any(Curr => !Curr.isExternal))
            {
                foreach (var InternalDocument in ReturnObject.Markup.Topic.DocumentReferences.Where(Curr => !Curr.isExternal))
                {
                    var FilePathInArchive = GetAbsolutePath(TopicID, InternalDocument.ReferencedDocument);
                    var Entry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == FilePathInArchive);
                    // Only append if not known already
                    if (!Container.FileAttachments.ContainsKey(Entry.Name))
                    {
                        using (var MemStream = new MemoryStream())
                        {
                            Entry.Open().CopyTo(MemStream);
                            Container.FileAttachments.Add(Entry.Name, MemStream.ToArray());
                        }
                    }
                }
            }
            // Get viewpoints
            for (var i = 0; i < ReturnObject.Markup.Viewpoints.Count; i++)
            {
                var DeserializedViewpoint = VisualizationInfo.Deserialize(Archive.Entries.First(Entry => Entry.FullName == TopicID + "/" + ReturnObject.Markup.Viewpoints[i].Viewpoint).Open());
                DeserializedViewpoint.GUID = ReturnObject.Markup.Viewpoints[i].Guid;
                ReturnObject.Viewpoints.Add(DeserializedViewpoint);
                // Get viewpoint bitmaps if present
                if (ReturnObject.Viewpoints[i].Bitmaps.Count > 0)
                {
                    foreach (var ViewpointBitmap in ReturnObject.Viewpoints[i].Bitmaps)
                    {
                        using (var BytesMemoryStream = new MemoryStream())
                        {
                            var BitmapPathInArchive = GetAbsolutePath(TopicID, ViewpointBitmap.Reference);
                            var BitmapFileEntry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == BitmapPathInArchive);
                            if (BitmapFileEntry == null)
                            {
                                // File entry was not found, possible because it was referenced with an absolute path
                                BitmapPathInArchive = GetAbsolutePath(TopicID, TransformToRelativePath(ViewpointBitmap.Reference, TopicID));
                                BitmapFileEntry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == BitmapPathInArchive);
                                if (BitmapFileEntry != null)
                                {
                                    ViewpointBitmap.Reference = TransformToRelativePath(ViewpointBitmap.Reference, TopicID);
                                }
                                else
                                {
                                    throw new ArgumentNullException("BitmapFileEntry", "Could not locate bitmap file in archive");
                                }
                            }
                            BitmapFileEntry.Open().CopyTo(BytesMemoryStream);
                            if (!ReturnObject.ViewpointBitmaps.ContainsKey(ReturnObject.Viewpoints[i]))
                            {
                                ReturnObject.ViewpointBitmaps.Add(ReturnObject.Viewpoints[i], new List<byte[]>());
                            }
                            ReturnObject.ViewpointBitmaps[ReturnObject.Viewpoints[i]].Add(BytesMemoryStream.ToArray());
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(ReturnObject.Markup.Viewpoints[i].Snapshot))
                {
                    using (var BytesMemoryStream = new MemoryStream())
                    {
                        Archive.Entries.First(Entry => Entry.FullName == TopicID + "/" + ReturnObject.Markup.Viewpoints[i].Snapshot)..Open().CopyTo(BytesMemoryStream);
                        ReturnObject.AddOrUpdateSnapshot(ReturnObject.Viewpoints[i].GUID, BytesMemoryStream.ToArray());
                    }
                }
            }
            return ReturnObject;
        }
    }
}