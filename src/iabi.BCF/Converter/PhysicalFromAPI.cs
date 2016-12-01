using System;
using System.Collections.Generic;
using System.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Converter
{
    /// <summary>
    ///     Will create an <see cref="BCFv2Container" /> instance from a <see cref="APIContainer" />
    /// </summary>
    public static class PhysicalFromAPI
    {
        /// <summary>
        ///     Will create an <see cref="BCFv2Container" /> instance from a <see cref="APIContainer" />
        /// </summary>
        public static BCFv2Container Convert(APIContainer Input)
        {
            var ReturnObject = new BCFv2Container();
            // Get file attachments
            foreach (var CurrentFile in Input.FileAttachments)
            {
                ReturnObject.FileAttachments.Add(CurrentFile.Key, CurrentFile.Value);
            }
            if (Input.Project != null)
            {
                ReturnObject.BCFProject = new ProjectExtension();
                ReturnObject.BCFProject.Project = new Project();
                ReturnObject.BCFProject.Project.Name = Input.Project.name;
                ReturnObject.BCFProject.Project.ProjectId = Input.Project.project_id;
            }
            if (Input.Extensions != null && !Input.Extensions.IsEmpty())
            {
                if (ReturnObject.BCFProject == null)
                {
                    ReturnObject.BCFProject = new ProjectExtension();
                }
                ReturnObject.BCFProject.ExtensionSchema = "extensions.xsd";
                ReturnObject.ProjectExtensions = new Extensions_XSD();
                ReturnObject.ProjectExtensions.Priority = Input.Extensions.priority;
                ReturnObject.ProjectExtensions.SnippetType = Input.Extensions.snippet_type;
                ReturnObject.ProjectExtensions.TopicLabel = Input.Extensions.topic_label;
                ReturnObject.ProjectExtensions.TopicStatus = Input.Extensions.topic_status;
                ReturnObject.ProjectExtensions.TopicType = Input.Extensions.topic_type;
                ReturnObject.ProjectExtensions.UserIdType = Input.Extensions.user_id_type;
            }
            if (Input.Topics.Count > 0)
            {
                foreach (var CurrentTopic in Input.Topics)
                {
                    ReturnObject.Topics.Add(GetPhysicalTopicFromAPI(CurrentTopic, Input));
                }
            }
            return ReturnObject;
        }

        private static BCFTopic GetPhysicalTopicFromAPI(TopicContainer GivenAPIContainer, APIContainer APIContainer)
        {
            if (GivenAPIContainer.Topic == null)
            {
                throw new ArgumentNullException(nameof(GivenAPIContainer.Topic));
            }
            var ReturnObject = new BCFTopic();
            ReturnObject.Markup = new Markup();
            // Get the topic stuff
            {
                ReturnObject.Markup.Topic = new Topic();

                if (GivenAPIContainer.Files.Any())
                {
                    foreach (var CurrentFile in GivenAPIContainer.Files)
                    {
                        ReturnObject.Markup.Header.Add(new HeaderFile
                        {
                            Date = CurrentFile.date,
                            Filename = CurrentFile.file_name,
                            IfcProject = CurrentFile.ifc_project,
                            IfcSpatialStructureElement = CurrentFile.ifc_spatial_structure_element,
                            isExternal = !APIContainer.FileAttachments.ContainsKey(BCFv2Container.GetFilenameFromReference(CurrentFile.reference)),
                            Reference = CurrentFile.reference
                        });
                    }
                }
                if (!string.IsNullOrWhiteSpace(GivenAPIContainer.Topic.assigned_to))
                {
                    ReturnObject.Markup.Topic.AssignedTo = GivenAPIContainer.Topic.assigned_to;
                }
                if (GivenAPIContainer.Topic.bim_snippet != null && GivenAPIContainer.Topic.bim_snippet.HasValues())
                {
                    ReturnObject.Markup.Topic.BimSnippet = new BimSnippet();

                    if (GivenAPIContainer.SnippetData != null)
                    {
                        ReturnObject.SnippetData = GivenAPIContainer.SnippetData;
                        ReturnObject.Markup.Topic.BimSnippet.isExternal = false;
                        ReturnObject.Markup.Topic.BimSnippet.Reference = BCFv2Container.GetFilenameFromReference(GivenAPIContainer.Topic.bim_snippet.reference);
                    }
                    else
                    {
                        ReturnObject.Markup.Topic.BimSnippet.isExternal = true;
                        ReturnObject.Markup.Topic.BimSnippet.Reference = GivenAPIContainer.Topic.bim_snippet.reference;
                    }
                    ReturnObject.Markup.Topic.BimSnippet.ReferenceSchema = GivenAPIContainer.Topic.bim_snippet.reference_schema;
                    ReturnObject.Markup.Topic.BimSnippet.SnippetType = GivenAPIContainer.Topic.bim_snippet.snippet_type;
                }
                ReturnObject.Markup.Topic.CreationAuthor = GivenAPIContainer.Topic.creation_author;
                ReturnObject.Markup.Topic.CreationDate = GivenAPIContainer.Topic.creation_date;
                ReturnObject.Markup.Topic.Description = GivenAPIContainer.Topic.description;
                ReturnObject.Markup.Topic.Guid = GivenAPIContainer.Topic.guid;
                ReturnObject.Markup.Topic.Index = GivenAPIContainer.Topic.index.ToString();
                if (GivenAPIContainer.Topic.labels != null && GivenAPIContainer.Topic.labels.Count > 0)
                {
                    ReturnObject.Markup.Topic.Labels = GivenAPIContainer.Topic.labels;
                }
                if (!string.IsNullOrWhiteSpace(GivenAPIContainer.Topic.modified_author) || (GivenAPIContainer.Topic.modified_date != null && default(DateTime) != GivenAPIContainer.Topic.modified_date))
                {
                    ReturnObject.Markup.Topic.ModifiedAuthor = GivenAPIContainer.Topic.modified_author;
                    if (GivenAPIContainer.Topic.modified_date != null)
                    {
                        ReturnObject.Markup.Topic.ModifiedDate = (DateTime) GivenAPIContainer.Topic.modified_date;
                    }
                }
                ReturnObject.Markup.Topic.Priority = GivenAPIContainer.Topic.priority;
                ReturnObject.Markup.Topic.ReferenceLink = GivenAPIContainer.Topic.reference_link;
                ReturnObject.Markup.Topic.Title = GivenAPIContainer.Topic.title;
                ReturnObject.Markup.Topic.TopicStatus = GivenAPIContainer.Topic.topic_status;
                ReturnObject.Markup.Topic.TopicType = GivenAPIContainer.Topic.topic_type;
                if (GivenAPIContainer.RelatedTopics.Count > 0)
                {
                    ReturnObject.Markup.Topic.RelatedTopics = new List<TopicRelatedTopics>();
                    foreach (var CurrentRelatedTopic in GivenAPIContainer.RelatedTopics)
                    {
                        ReturnObject.Markup.Topic.RelatedTopics.Add(new TopicRelatedTopics());
                        ReturnObject.Markup.Topic.RelatedTopics[ReturnObject.Markup.Topic.RelatedTopics.Count - 1].Guid = CurrentRelatedTopic.related_topic_guid;
                    }
                }
                if (GivenAPIContainer.ReferencedDocuments.Count > 0)
                {
                    ReturnObject.Markup.Topic.DocumentReferences = new List<TopicDocumentReferences>();
                    foreach (var CurrentReferencedDocument in GivenAPIContainer.ReferencedDocuments)
                    {
                        ReturnObject.Markup.Topic.DocumentReferences.Add(new TopicDocumentReferences());
                        ReturnObject.Markup.Topic.DocumentReferences.Last().Description = CurrentReferencedDocument.description;
                        ReturnObject.Markup.Topic.DocumentReferences.Last().Guid = CurrentReferencedDocument.guid;
                        ReturnObject.Markup.Topic.DocumentReferences.Last().isExternal = !APIContainer.FileAttachments.ContainsKey(BCFv2Container.GetFilenameFromReference(CurrentReferencedDocument.referenced_document));
                        ReturnObject.Markup.Topic.DocumentReferences.Last().ReferencedDocument = CurrentReferencedDocument.referenced_document;
                    }
                }
            }
            // Get comments
            {
                if (GivenAPIContainer.Comments.Count > 0)
                {
                    ReturnObject.Markup.Comment = new List<Comment>();
                    foreach (var CurrentComment in GivenAPIContainer.Comments)
                    {
                        ReturnObject.Markup.Comment.Add(new Comment());
                        ReturnObject.Markup.Comment.Last().Author = CurrentComment.author;
                        ReturnObject.Markup.Comment.Last().Comment1 = CurrentComment.comment;
                        ReturnObject.Markup.Comment.Last().Date = CurrentComment.date;
                        ReturnObject.Markup.Comment.Last().Guid = CurrentComment.guid;
                        if (!string.IsNullOrWhiteSpace(CurrentComment.modified_author) || (CurrentComment.modified_date != null && default(DateTime) != CurrentComment.modified_date))
                        {
                            ReturnObject.Markup.Comment.Last().ModifiedAuthor = CurrentComment.modified_author;
                            if (CurrentComment.modified_date != null)
                            {
                                ReturnObject.Markup.Comment.Last().ModifiedDate = (DateTime) CurrentComment.modified_date;
                            }
                        }
                        ReturnObject.Markup.Comment.Last().ReplyToComment = new CommentReplyToComment();
                        ReturnObject.Markup.Comment.Last().ReplyToComment.Guid = CurrentComment.reply_to_comment_guid;
                        ReturnObject.Markup.Comment.Last().Status = CurrentComment.status;
                        ReturnObject.Markup.Comment.Last().VerbalStatus = CurrentComment.verbal_status;
                        ReturnObject.Markup.Comment.Last().Viewpoint = new CommentViewpoint();
                        ReturnObject.Markup.Comment.Last().Viewpoint.Guid = CurrentComment.viewpoint_guid;
                    }
                }
            }
            // Get viewpoints
            {
                if (GivenAPIContainer.Viewpoints.Count > 0)
                {
                    foreach (var CurrentViewpoint in GivenAPIContainer.Viewpoints)
                    {
                        ReturnObject.Viewpoints.Add(new VisualizationInfo
                        {
                            GUID = CurrentViewpoint.Viewpoint.guid
                        });
                        if (CurrentViewpoint.Snapshot != null)
                        {
                            ReturnObject.AddOrUpdateSnapshot(CurrentViewpoint.Viewpoint.guid, CurrentViewpoint.Snapshot);
                        }
                        if (CurrentViewpoint.Viewpoint.lines != null && CurrentViewpoint.Viewpoint.lines.line.Count > 0)
                        {
                            ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines = new List<Line>();
                            foreach (var CurrentLine in CurrentViewpoint.Viewpoint.lines.line)
                            {
                                ReturnObject.Viewpoints.Last().Lines.Add(new Line());
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].EndPoint = new Point();
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].EndPoint.X = CurrentLine.end_point.x;
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].EndPoint.Y = CurrentLine.end_point.y;
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].EndPoint.Z = CurrentLine.end_point.z;
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].StartPoint = new Point();
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].StartPoint.X = CurrentLine.start_point.x;
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].StartPoint.Y = CurrentLine.start_point.y;
                                ReturnObject.Viewpoints.Last().Lines[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Lines.Count - 1].StartPoint.Z = CurrentLine.start_point.z;
                            }
                        }
                        if (CurrentViewpoint.Viewpoint.clipping_planes != null && CurrentViewpoint.Viewpoint.clipping_planes.clipping_plane.Count > 0)
                        {
                            ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes = new List<ClippingPlane>();
                            foreach (var CurrentClippingPlane in CurrentViewpoint.Viewpoint.clipping_planes.clipping_plane)
                            {
                                ReturnObject.Viewpoints.Last().ClippingPlanes.Add(new ClippingPlane());
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction = new Direction();
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction.X = CurrentClippingPlane.direction.x;
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction.Y = CurrentClippingPlane.direction.y;
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction.Z = CurrentClippingPlane.direction.z;
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location = new Point();
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location.X = CurrentClippingPlane.location.x;
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location.Y = CurrentClippingPlane.location.y;
                                ReturnObject.Viewpoints.Last().ClippingPlanes[ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location.Z = CurrentClippingPlane.location.z;
                            }
                        }
                        if (CurrentViewpoint.Components != null && CurrentViewpoint.Components.Count > 0)
                        {
                            ReturnObject.Viewpoints[ReturnObject.Viewpoints.Count - 1].Components = new List<Component>();
                            foreach (var CurrentComponent in CurrentViewpoint.Components)
                            {
                                ReturnObject.Viewpoints.Last().Components.Add(new Component());
                                ReturnObject.Viewpoints.Last().Components.Last().AuthoringToolId = CurrentComponent.authoring_tool_id;
                                ReturnObject.Viewpoints.Last().Components.Last().Color = CurrentComponent.color == null ? null : Enumerable.Range(0, CurrentComponent.color.Length)
                                    .Where(x => x%2 == 0)
                                    .Select(x => System.Convert.ToByte(CurrentComponent.color.Substring(x, 2), 16))
                                    .ToArray();
                                ReturnObject.Viewpoints.Last().Components.Last().IfcGuid = CurrentComponent.ifc_guid;
                                ReturnObject.Viewpoints.Last().Components.Last().OriginatingSystem = CurrentComponent.originating_system;
                                if (CurrentComponent.selected)
                                {
                                    ReturnObject.Viewpoints.Last().Components.Last().Selected = CurrentComponent.selected;
                                }
                                ReturnObject.Viewpoints.Last().Components.Last().Visible = CurrentComponent.visible;
                            }
                        }
                        if (CurrentViewpoint.Viewpoint.orthogonal_camera != null)
                        {
                            ReturnObject.Viewpoints.Last().OrthogonalCamera = new OrthogonalCamera();
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraDirection = new Direction();
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraDirection.X = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_direction.x;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraDirection.Y = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_direction.y;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraDirection.Z = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_direction.z;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraUpVector = new Direction();
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraUpVector.X = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.x;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraUpVector.Y = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.y;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraUpVector.Z = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.z;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraViewPoint = new Point();
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraViewPoint.X = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_view_point.x;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraViewPoint.Y = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_view_point.y;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.CameraViewPoint.Z = CurrentViewpoint.Viewpoint.orthogonal_camera.camera_view_point.z;
                            ReturnObject.Viewpoints.Last().OrthogonalCamera.ViewToWorldScale = CurrentViewpoint.Viewpoint.orthogonal_camera.view_to_world_scale;
                        }
                        if (CurrentViewpoint.Viewpoint.perspective_camera != null)
                        {
                            ReturnObject.Viewpoints.Last().PerspectiveCamera = new PerspectiveCamera();
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraDirection = new Direction();
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraDirection.X = CurrentViewpoint.Viewpoint.perspective_camera.camera_direction.x;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraDirection.Y = CurrentViewpoint.Viewpoint.perspective_camera.camera_direction.y;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraDirection.Z = CurrentViewpoint.Viewpoint.perspective_camera.camera_direction.z;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraUpVector = new Direction();
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraUpVector.X = CurrentViewpoint.Viewpoint.perspective_camera.camera_up_vector.x;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraUpVector.Y = CurrentViewpoint.Viewpoint.perspective_camera.camera_up_vector.y;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraUpVector.Z = CurrentViewpoint.Viewpoint.perspective_camera.camera_up_vector.z;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraViewPoint = new Point();
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraViewPoint.X = CurrentViewpoint.Viewpoint.perspective_camera.camera_view_point.x;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraViewPoint.Y = CurrentViewpoint.Viewpoint.perspective_camera.camera_view_point.y;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.CameraViewPoint.Z = CurrentViewpoint.Viewpoint.perspective_camera.camera_view_point.z;
                            ReturnObject.Viewpoints.Last().PerspectiveCamera.FieldOfView = CurrentViewpoint.Viewpoint.perspective_camera.field_of_view;
                        }
                    }
                }
            }
            return ReturnObject;
        }
    }
}