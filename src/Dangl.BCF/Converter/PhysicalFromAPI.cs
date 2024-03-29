using System;
using System.Collections.Generic;
using System.Linq;
using Dangl.BCF.BCFv2;
using Dangl.BCF.BCFv2.Schemas;

namespace Dangl.BCF.Converter
{
    /// <summary>
    ///     Will create an <see cref="BCFv2Container" /> instance from a <see cref="APIContainer" />
    /// </summary>
    public static class PhysicalFromApi
    {
        /// <summary>
        ///     Will create an <see cref="BCFv2Container" /> instance from a <see cref="APIContainer" />
        /// </summary>
        public static BCFv2Container Convert(APIContainer apiContainer)
        {
            var bcfContainer = new BCFv2Container();
            // Get file attachments
            foreach (var fileAttachment in apiContainer.FileAttachments)
            {
                bcfContainer.FileAttachments.Add(fileAttachment.Key, fileAttachment.Value);
            }
            if (apiContainer.Project != null)
            {
                bcfContainer.BcfProject = new ProjectExtension();
                bcfContainer.BcfProject.Project = new Project();
                bcfContainer.BcfProject.Project.Name = apiContainer.Project.name;
                bcfContainer.BcfProject.Project.ProjectId = apiContainer.Project.project_id;
            }
            if (apiContainer.Extensions != null && !apiContainer.Extensions.IsEmpty())
            {
                if (bcfContainer.BcfProject == null)
                {
                    bcfContainer.BcfProject = new ProjectExtension();
                }
                bcfContainer.BcfProject.ExtensionSchema = "extensions.xsd";
                bcfContainer.ProjectExtensions = new ProjectExtensions();
                bcfContainer.ProjectExtensions.Priority = apiContainer.Extensions.priority;
                bcfContainer.ProjectExtensions.SnippetType = apiContainer.Extensions.snippet_type;
                bcfContainer.ProjectExtensions.TopicLabel = apiContainer.Extensions.topic_label;
                bcfContainer.ProjectExtensions.TopicStatus = apiContainer.Extensions.topic_status;
                bcfContainer.ProjectExtensions.TopicType = apiContainer.Extensions.topic_type;
                bcfContainer.ProjectExtensions.UserIdType = apiContainer.Extensions.user_id_type;
            }
            if (apiContainer.Topics.Count > 0)
            {
                foreach (var topic in apiContainer.Topics)
                {
                    bcfContainer.Topics.Add(GetPhysicalTopicFromApi(topic, apiContainer));
                }
            }
            return bcfContainer;
        }

        private static BCFTopic GetPhysicalTopicFromApi(TopicContainer apiTopicContainer, APIContainer apiContainer)
        {
            if (apiTopicContainer.Topic == null)
            {
                throw new ArgumentNullException(nameof(apiTopicContainer.Topic));
            }
            var bcfTopic = new BCFTopic();
            bcfTopic.Markup = new Markup();
            {
                bcfTopic.Markup.Topic = new Topic();

                if (apiTopicContainer.Files.Any())
                {
                    foreach (var file in apiTopicContainer.Files)
                    {
                        bcfTopic.Markup.Header.Add(new HeaderFile
                        {
                            Date = file.date,
                            Filename = file.file_name,
                            IfcProject = file.ifc_project,
                            IfcSpatialStructureElement = file.ifc_spatial_structure_element,
                            isExternal = !apiContainer.FileAttachments.ContainsKey(BCFv2Container.GetFilenameFromReference(file.reference)),
                            Reference = file.reference
                        });
                    }
                }
                if (!string.IsNullOrWhiteSpace(apiTopicContainer.Topic.assigned_to))
                {
                    bcfTopic.Markup.Topic.AssignedTo = apiTopicContainer.Topic.assigned_to;
                }
                if (apiTopicContainer.Topic.bim_snippet != null && apiTopicContainer.Topic.bim_snippet.HasValues())
                {
                    bcfTopic.Markup.Topic.BimSnippet = new BimSnippet();

                    if (apiTopicContainer.SnippetData != null)
                    {
                        bcfTopic.SnippetData = apiTopicContainer.SnippetData;
                        bcfTopic.Markup.Topic.BimSnippet.isExternal = false;
                        bcfTopic.Markup.Topic.BimSnippet.Reference = BCFv2Container.GetFilenameFromReference(apiTopicContainer.Topic.bim_snippet.reference);
                    }
                    else
                    {
                        bcfTopic.Markup.Topic.BimSnippet.isExternal = true;
                        bcfTopic.Markup.Topic.BimSnippet.Reference = apiTopicContainer.Topic.bim_snippet.reference;
                    }
                    bcfTopic.Markup.Topic.BimSnippet.ReferenceSchema = apiTopicContainer.Topic.bim_snippet.reference_schema;
                    bcfTopic.Markup.Topic.BimSnippet.SnippetType = apiTopicContainer.Topic.bim_snippet.snippet_type;
                }
                bcfTopic.Markup.Topic.CreationAuthor = apiTopicContainer.Topic.creation_author;
                bcfTopic.Markup.Topic.CreationDate = apiTopicContainer.Topic.creation_date;
                bcfTopic.Markup.Topic.Description = apiTopicContainer.Topic.description;
                bcfTopic.Markup.Topic.Guid = apiTopicContainer.Topic.guid;
                bcfTopic.Markup.Topic.Index = apiTopicContainer.Topic.index.ToString();
                if (apiTopicContainer.Topic.labels != null && apiTopicContainer.Topic.labels.Count > 0)
                {
                    bcfTopic.Markup.Topic.Labels = apiTopicContainer.Topic.labels;
                }
                if (!string.IsNullOrWhiteSpace(apiTopicContainer.Topic.modified_author) || (apiTopicContainer.Topic.modified_date != null && default(DateTime) != apiTopicContainer.Topic.modified_date))
                {
                    bcfTopic.Markup.Topic.ModifiedAuthor = apiTopicContainer.Topic.modified_author;
                    if (apiTopicContainer.Topic.modified_date != null)
                    {
                        bcfTopic.Markup.Topic.ModifiedDate = (DateTime) apiTopicContainer.Topic.modified_date;
                    }
                }
                bcfTopic.Markup.Topic.Priority = apiTopicContainer.Topic.priority;
                bcfTopic.Markup.Topic.ReferenceLink = apiTopicContainer.Topic.reference_link;
                bcfTopic.Markup.Topic.Title = apiTopicContainer.Topic.title;
                bcfTopic.Markup.Topic.TopicStatus = apiTopicContainer.Topic.topic_status;
                bcfTopic.Markup.Topic.TopicType = apiTopicContainer.Topic.topic_type;
                if (apiTopicContainer.RelatedTopics.Count > 0)
                {
                    bcfTopic.Markup.Topic.RelatedTopics = new List<TopicRelatedTopics>();
                    foreach (var relatedTopic in apiTopicContainer.RelatedTopics)
                    {
                        bcfTopic.Markup.Topic.RelatedTopics.Add(new TopicRelatedTopics());
                        bcfTopic.Markup.Topic.RelatedTopics[bcfTopic.Markup.Topic.RelatedTopics.Count - 1].Guid = relatedTopic.related_topic_guid;
                    }
                }
                if (apiTopicContainer.ReferencedDocuments.Count > 0)
                {
                    bcfTopic.Markup.Topic.DocumentReferences = new List<TopicDocumentReferences>();
                    foreach (var referencedDocument in apiTopicContainer.ReferencedDocuments)
                    {
                        bcfTopic.Markup.Topic.DocumentReferences.Add(new TopicDocumentReferences());
                        bcfTopic.Markup.Topic.DocumentReferences.Last().Description = referencedDocument.description;
                        bcfTopic.Markup.Topic.DocumentReferences.Last().Guid = referencedDocument.guid;
                        bcfTopic.Markup.Topic.DocumentReferences.Last().isExternal = !apiContainer.FileAttachments.ContainsKey(BCFv2Container.GetFilenameFromReference(referencedDocument.referenced_document));
                        bcfTopic.Markup.Topic.DocumentReferences.Last().ReferencedDocument = referencedDocument.referenced_document;
                    }
                }
            }
            {
                if (apiTopicContainer.Comments.Count > 0)
                {
                    bcfTopic.Markup.Comment = new List<Comment>();
                    foreach (var comment in apiTopicContainer.Comments)
                    {
                        bcfTopic.Markup.Comment.Add(new Comment());
                        bcfTopic.Markup.Comment.Last().Author = comment.author;
                        bcfTopic.Markup.Comment.Last().Comment1 = comment.comment;
                        bcfTopic.Markup.Comment.Last().Date = comment.date;
                        bcfTopic.Markup.Comment.Last().Guid = comment.guid;
                        if (!string.IsNullOrWhiteSpace(comment.modified_author) || (comment.modified_date != null && default(DateTime) != comment.modified_date))
                        {
                            bcfTopic.Markup.Comment.Last().ModifiedAuthor = comment.modified_author;
                            if (comment.modified_date != null)
                            {
                                bcfTopic.Markup.Comment.Last().ModifiedDate = (DateTime) comment.modified_date;
                            }
                        }
                        bcfTopic.Markup.Comment.Last().ReplyToComment = new CommentReplyToComment();
                        bcfTopic.Markup.Comment.Last().ReplyToComment.Guid = comment.reply_to_comment_guid;
                        bcfTopic.Markup.Comment.Last().Status = comment.status;
                        bcfTopic.Markup.Comment.Last().VerbalStatus = comment.verbal_status;
                        bcfTopic.Markup.Comment.Last().Viewpoint = new CommentViewpoint();
                        bcfTopic.Markup.Comment.Last().Viewpoint.Guid = comment.viewpoint_guid;
                    }
                }
            }
            {
                if (apiTopicContainer.Viewpoints.Count > 0)
                {
                    foreach (var viewpoint in apiTopicContainer.Viewpoints)
                    {
                        bcfTopic.Viewpoints.Add(new VisualizationInfo
                        {
                            GUID = viewpoint.Viewpoint.guid
                        });
                        if (viewpoint.Snapshot != null)
                        {
                            bcfTopic.AddOrUpdateSnapshot(viewpoint.Viewpoint.guid, viewpoint.Snapshot);
                        }
                        if (viewpoint.Viewpoint.lines != null && viewpoint.Viewpoint.lines.line.Count > 0)
                        {
                            bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines = new List<Line>();
                            foreach (var line in viewpoint.Viewpoint.lines.line)
                            {
                                bcfTopic.Viewpoints.Last().Lines.Add(new Line());
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].EndPoint = new Point();
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].EndPoint.X = line.end_point.x;
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].EndPoint.Y = line.end_point.y;
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].EndPoint.Z = line.end_point.z;
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].StartPoint = new Point();
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].StartPoint.X = line.start_point.x;
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].StartPoint.Y = line.start_point.y;
                                bcfTopic.Viewpoints.Last().Lines[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Lines.Count - 1].StartPoint.Z = line.start_point.z;
                            }
                        }
                        if (viewpoint.Viewpoint.clipping_planes != null && viewpoint.Viewpoint.clipping_planes.clipping_plane.Count > 0)
                        {
                            bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes = new List<ClippingPlane>();
                            foreach (var clippingPlane in viewpoint.Viewpoint.clipping_planes.clipping_plane)
                            {
                                bcfTopic.Viewpoints.Last().ClippingPlanes.Add(new ClippingPlane());
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction = new Direction();
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction.X = clippingPlane.direction.x;
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction.Y = clippingPlane.direction.y;
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Direction.Z = clippingPlane.direction.z;
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location = new Point();
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location.X = clippingPlane.location.x;
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location.Y = clippingPlane.location.y;
                                bcfTopic.Viewpoints.Last().ClippingPlanes[bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].ClippingPlanes.Count - 1].Location.Z = clippingPlane.location.z;
                            }
                        }
                        if (viewpoint.Components != null && viewpoint.Components.Count > 0)
                        {
                            bcfTopic.Viewpoints[bcfTopic.Viewpoints.Count - 1].Components = new List<Component>();
                            foreach (var component in viewpoint.Components)
                            {
                                bcfTopic.Viewpoints.Last().Components.Add(new Component());
                                bcfTopic.Viewpoints.Last().Components.Last().AuthoringToolId = component.authoring_tool_id;
                                bcfTopic.Viewpoints.Last().Components.Last().Color = component.color == null ? null : Enumerable.Range(0, component.color.Length)
                                    .Where(x => x%2 == 0)
                                    .Select(x => System.Convert.ToByte(component.color.Substring(x, 2), 16))
                                    .ToArray();
                                bcfTopic.Viewpoints.Last().Components.Last().IfcGuid = component.ifc_guid;
                                bcfTopic.Viewpoints.Last().Components.Last().OriginatingSystem = component.originating_system;
                                if (component.selected)
                                {
                                    bcfTopic.Viewpoints.Last().Components.Last().Selected = component.selected;
                                }
                                bcfTopic.Viewpoints.Last().Components.Last().Visible = component.visible;
                            }
                        }
                        if (viewpoint.Viewpoint.orthogonal_camera != null)
                        {
                            bcfTopic.Viewpoints.Last().OrthogonalCamera = new OrthogonalCamera();
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraDirection = new Direction();
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraDirection.X = viewpoint.Viewpoint.orthogonal_camera.camera_direction.x;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraDirection.Y = viewpoint.Viewpoint.orthogonal_camera.camera_direction.y;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraDirection.Z = viewpoint.Viewpoint.orthogonal_camera.camera_direction.z;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraUpVector = new Direction();
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraUpVector.X = viewpoint.Viewpoint.orthogonal_camera.camera_up_vector.x;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraUpVector.Y = viewpoint.Viewpoint.orthogonal_camera.camera_up_vector.y;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraUpVector.Z = viewpoint.Viewpoint.orthogonal_camera.camera_up_vector.z;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraViewPoint = new Point();
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraViewPoint.X = viewpoint.Viewpoint.orthogonal_camera.camera_view_point.x;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraViewPoint.Y = viewpoint.Viewpoint.orthogonal_camera.camera_view_point.y;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.CameraViewPoint.Z = viewpoint.Viewpoint.orthogonal_camera.camera_view_point.z;
                            bcfTopic.Viewpoints.Last().OrthogonalCamera.ViewToWorldScale = viewpoint.Viewpoint.orthogonal_camera.view_to_world_scale;
                        }
                        if (viewpoint.Viewpoint.perspective_camera != null)
                        {
                            bcfTopic.Viewpoints.Last().PerspectiveCamera = new PerspectiveCamera();
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraDirection = new Direction();
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraDirection.X = viewpoint.Viewpoint.perspective_camera.camera_direction.x;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraDirection.Y = viewpoint.Viewpoint.perspective_camera.camera_direction.y;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraDirection.Z = viewpoint.Viewpoint.perspective_camera.camera_direction.z;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraUpVector = new Direction();
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraUpVector.X = viewpoint.Viewpoint.perspective_camera.camera_up_vector.x;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraUpVector.Y = viewpoint.Viewpoint.perspective_camera.camera_up_vector.y;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraUpVector.Z = viewpoint.Viewpoint.perspective_camera.camera_up_vector.z;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraViewPoint = new Point();
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraViewPoint.X = viewpoint.Viewpoint.perspective_camera.camera_view_point.x;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraViewPoint.Y = viewpoint.Viewpoint.perspective_camera.camera_view_point.y;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.CameraViewPoint.Z = viewpoint.Viewpoint.perspective_camera.camera_view_point.z;
                            bcfTopic.Viewpoints.Last().PerspectiveCamera.FieldOfView = viewpoint.Viewpoint.perspective_camera.field_of_view;
                        }
                    }
                }
            }
            return bcfTopic;
        }
    }
}