using iabi.BCF.APIObjects.Comment;
using iabi.BCF.APIObjects.Component;
using iabi.BCF.APIObjects.DocumentReference;
using iabi.BCF.APIObjects.Extensions;
using iabi.BCF.APIObjects.Project;
using iabi.BCF.APIObjects.RelatedTopic;
using iabi.BCF.APIObjects.Topic;
using iabi.BCF.APIObjects.Viewpoint;
using iabi.BCF.APIObjects.Viewpoint.Components;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iabi.BCF.Converter
{
    /// <summary>
    /// Will create an <see cref="APIContainer"/> instance from a <see cref="BCFv2Container"/>
    /// </summary>
    public static class APIFromPhysical
    {
        /// <summary>
        /// Will create an <see cref="APIContainer"/> instance from a <see cref="BCFv2Container"/>
        /// </summary>
        public static APIContainer Convert(BCFv2Container Input)
        {
            if (Input == null) throw new ArgumentNullException("Input");
            var ReturnObject = new APIContainer();
            // Get the project info
            if (Input.BCFProject != null && Input.BCFProject.Project != null)
            {
                ReturnObject.Project = new project_GET();
                ReturnObject.Project.name = Input.BCFProject.Project.Name;
                ReturnObject.Project.project_id = Input.BCFProject.Project.ProjectId;
            }
            // Get the extensions
            if (Input.ProjectExtensions != null)
            {
                ReturnObject.Extensions = new extensions_GET();
                ReturnObject.Extensions.priority = Input.ProjectExtensions.Priority;
                ReturnObject.Extensions.snippet_type = Input.ProjectExtensions.SnippetType;
                ReturnObject.Extensions.topic_label = Input.ProjectExtensions.TopicLabel;
                ReturnObject.Extensions.topic_status = Input.ProjectExtensions.TopicStatus;
                ReturnObject.Extensions.topic_type = Input.ProjectExtensions.TopicType;
                ReturnObject.Extensions.user_id_type = Input.ProjectExtensions.UserIdType;
            }
            ReturnObject.Topics = new List<TopicContainer>();
            for (int i = 0; i < Input.Topics.Count; i++)
            {
                ReturnObject.Topics.Add(GetAPITopicFromPhysicalBCF(Input.Topics[i], i, Input));
            }
            // Get file attachments
            foreach (var CurrentAttachment in Input.FileAttachments)
            {
                ReturnObject.FileAttachments.Add(CurrentAttachment.Key, CurrentAttachment.Value);
            }
            return ReturnObject;
        }

        private static TopicContainer GetAPITopicFromPhysicalBCF(BCFTopic GivenPhysicalBCFv2, int TopicIndex, BCFv2Container Container)
        {
            TopicContainer ReturnObject = new TopicContainer();
            if (GivenPhysicalBCFv2.SnippetData != null)
            {
                ReturnObject.SnippetData = GivenPhysicalBCFv2.SnippetData;
            }
            // Get topic files
            if (GivenPhysicalBCFv2.Markup != null && GivenPhysicalBCFv2.Markup.ShouldSerializeHeader() && GivenPhysicalBCFv2.Markup.Header.Any())
            {
                foreach (var CurrentFile in GivenPhysicalBCFv2.Markup.Header)
                {
                    ReturnObject.Files.Add(new APIObjects.File.file_GET
                    {
                        date = CurrentFile.Date,
                        file_name = CurrentFile.Filename,
                        ifc_project = CurrentFile.IfcProject,
                        ifc_spatial_structure_element = CurrentFile.IfcSpatialStructureElement,
                        reference = CurrentFile.Reference
                    });
                }
            }
            // Get topic info
            ReturnObject.Topic = GetSingleTopicInfo(GivenPhysicalBCFv2);
            // Get the comments
            if (GivenPhysicalBCFv2.Markup != null)
            {
                foreach (var CurrentComment in GivenPhysicalBCFv2.Markup.Comment)
                {
                    ReturnObject.Comments.Add(GetSingleCommentInfo(CurrentComment));
                }
                // Get referenced documents
                foreach (var CurrentDocument in GivenPhysicalBCFv2.Markup.Topic.DocumentReferences)
                {
                    // TODO do something clever in the case of the physical file having an INTERNAL document.
                    ReturnObject.ReferencedDocuments.Add(new document_reference_GET());
                    ReturnObject.ReferencedDocuments[ReturnObject.ReferencedDocuments.Count - 1].description = CurrentDocument.Description;
                    ReturnObject.ReferencedDocuments[ReturnObject.ReferencedDocuments.Count - 1].guid = CurrentDocument.Guid;
                    ReturnObject.ReferencedDocuments[ReturnObject.ReferencedDocuments.Count - 1].referenced_document = CurrentDocument.ReferencedDocument;
                }
                // Get related topics
                foreach (var CurrentRelatedTopic in GivenPhysicalBCFv2.Markup.Topic.RelatedTopics)
                {
                    ReturnObject.RelatedTopics.Add(new related_topic_GET());
                    ReturnObject.RelatedTopics[ReturnObject.RelatedTopics.Count - 1].related_topic_guid = CurrentRelatedTopic.Guid;
                }
            }
            // Get Viewpoints
            foreach (var CurrentViewpoint in GivenPhysicalBCFv2.Viewpoints)
            {
                if (CurrentViewpoint.Bitmaps.Count > 0)
                {
                    for (int i = 0; i < CurrentViewpoint.Bitmaps.Count; i++)
                    {
                        ReturnObject.Viewpoints.Add(GetSingleViewpoint(CurrentViewpoint, i, TopicIndex, Container));
                    }
                }
                else
                {
                    ReturnObject.Viewpoints.Add(GetSingleViewpoint(CurrentViewpoint, TopicIndex, Container));
                }
            }
            // Get Viewpoint snapshots
            foreach (var CurrentSnapshot in GivenPhysicalBCFv2.ViewpointSnapshots)
            {
                ReturnObject.Viewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentSnapshot.Key).Snapshot = CurrentSnapshot.Value;
            }
            return ReturnObject;
        }

        private static topic_GET GetSingleTopicInfo(BCFTopic GivenPhysicalBCFv2)
        {
            var ReturnObject = new topic_GET();
            if (GivenPhysicalBCFv2.Markup == null) return ReturnObject;
            ReturnObject.assigned_to = GivenPhysicalBCFv2.Markup.Topic.AssignedTo;
            if (GivenPhysicalBCFv2.Markup.Topic.BimSnippet != null)
            {
                ReturnObject.bim_snippet = new bim_snippet();
                ReturnObject.bim_snippet.is_external = GivenPhysicalBCFv2.Markup.Topic.BimSnippet.isExternal;
                ReturnObject.bim_snippet.reference = GivenPhysicalBCFv2.Markup.Topic.BimSnippet.Reference;
                ReturnObject.bim_snippet.reference_schema = GivenPhysicalBCFv2.Markup.Topic.BimSnippet.ReferenceSchema;
                ReturnObject.bim_snippet.snippet_type = GivenPhysicalBCFv2.Markup.Topic.BimSnippet.SnippetType;
            }
            ReturnObject.creation_author = GivenPhysicalBCFv2.Markup.Topic.CreationAuthor;
            ReturnObject.creation_date = GivenPhysicalBCFv2.Markup.Topic.CreationDate;
            ReturnObject.description = GivenPhysicalBCFv2.Markup.Topic.Description;
            ReturnObject.guid = GivenPhysicalBCFv2.Markup.Topic.Guid;
            ReturnObject.index = System.Convert.ToInt32(GivenPhysicalBCFv2.Markup.Topic.Index);
            ReturnObject.labels = GivenPhysicalBCFv2.Markup.Topic.Labels;
            ReturnObject.modified_author = GivenPhysicalBCFv2.Markup.Topic.ModifiedAuthor;
            ReturnObject.modified_date = GivenPhysicalBCFv2.Markup.Topic.ModifiedDate;
            ReturnObject.priority = GivenPhysicalBCFv2.Markup.Topic.Priority;
            ReturnObject.reference_link = GivenPhysicalBCFv2.Markup.Topic.ReferenceLink;
            ReturnObject.title = GivenPhysicalBCFv2.Markup.Topic.Title;
            ReturnObject.topic_status = GivenPhysicalBCFv2.Markup.Topic.TopicStatus;
            ReturnObject.topic_type = GivenPhysicalBCFv2.Markup.Topic.TopicType;
            return ReturnObject;
        }

        private static comment_GET GetSingleCommentInfo(Comment GivenComment)
        {
            var ReturnObject = new comment_GET();
            ReturnObject.author = GivenComment.Author;
            ReturnObject.comment = GivenComment.Comment1;
            ReturnObject.date = GivenComment.Date;
            ReturnObject.guid = GivenComment.Guid;
            ReturnObject.modified_author = GivenComment.ModifiedAuthor;
            ReturnObject.modified_date = GivenComment.ModifiedDate;
            ReturnObject.reply_to_comment_guid = GivenComment.ReplyToComment.Guid;
            ReturnObject.status = GivenComment.Status;
            ReturnObject.verbal_status = GivenComment.VerbalStatus;
            ReturnObject.viewpoint_guid = GivenComment.Viewpoint.Guid;
            return ReturnObject;
        }

        private static ViewpointContainer GetSingleViewpoint(VisualizationInfo GivenPhysicalViewpoint, int TopicIndex, BCFv2Container Container)
        {
            return GetSingleViewpoint(GivenPhysicalViewpoint, TopicIndex, -1, Container);
        }

        private static ViewpointContainer GetSingleViewpoint(VisualizationInfo GivenPhysicalViewpoint, int TopicIndex, int BitmapIndex, BCFv2Container Container)
        {
            var ReturnObject = new ViewpointContainer();
            ReturnObject.Viewpoint = new viewpoint_GET();
            // Get the components
            foreach (var CurrentComponent in GivenPhysicalViewpoint.Components)
            {
                if (ReturnObject.Components == null)
                {
                    ReturnObject.Components = new List<component_GET>();
                }
                ReturnObject.Components.Add(GetSingleViewpointComponent(CurrentComponent));
            }
            if (GivenPhysicalViewpoint.ClippingPlanes.Count > 0)
            {
                ReturnObject.Viewpoint.clipping_planes = new clipping_planes();
                ReturnObject.Viewpoint.clipping_planes.clipping_plane = new List<clipping_plane>();
                foreach (var CurrentClippingPlane in GivenPhysicalViewpoint.ClippingPlanes)
                {
                    ReturnObject.Viewpoint.clipping_planes.clipping_plane.Add(GetViewpointClippingPlane(CurrentClippingPlane));
                }
            }
            if (GivenPhysicalViewpoint.Lines.Count > 0)
            {
                ReturnObject.Viewpoint.lines = new lines();
                ReturnObject.Viewpoint.lines.line = new List<line>();
                foreach (var CurrentLine in GivenPhysicalViewpoint.Lines)
                {
                    ReturnObject.Viewpoint.lines.line.Add(GetViewpointLine(CurrentLine));
                }
            }
            ReturnObject.Viewpoint.guid = GivenPhysicalViewpoint.GUID;
            if (GivenPhysicalViewpoint.ShouldSerializeOrthogonalCamera())
            {
                ReturnObject.Viewpoint.orthogonal_camera = GetViewpointOrthogonalCamera(GivenPhysicalViewpoint.OrthogonalCamera);
            }
            // Else if here because only on of the cameras may be set
            else if (GivenPhysicalViewpoint.ShouldSerializePerspectiveCamera())
            {
                ReturnObject.Viewpoint.perspective_camera = GetViewpointPerspectiveCamera(GivenPhysicalViewpoint.PerspectiveCamera);
            }
            // Get the bitmaps if there are any
            // TODO
            //if (BitmapIndex > -1)
            //{
            //    ReturnObject.Viewpoint.snapshot = new snapshot();
            //    ReturnObject.Viewpoint.snapshot.height = GivenPhysicalViewpoint.Bitmaps[BitmapIndex].Height;
            //    ReturnObject.Viewpoint.snapshot.location = new PointOrVector();
            //    ReturnObject.Viewpoint.snapshot.location.x = GivenPhysicalViewpoint.Bitmaps[BitmapIndex].Location.X;
            //    ReturnObject.Viewpoint.snapshot.location.y = GivenPhysicalViewpoint.Bitmaps[BitmapIndex].Location.Y;
            //    ReturnObject.Viewpoint.snapshot.location.z = GivenPhysicalViewpoint.Bitmaps[BitmapIndex].Location.Z;
            //    ReturnObject.Viewpoint.snapshot.normal = new PointOrVector();
            //    ReturnObject.Viewpoint.snapshot.normal.x = GivenPhysicalViewpoint.Bitmaps[BitmapIndex].Normal.X;
            //    ReturnObject.Viewpoint.snapshot.normal.y = GivenPhysicalViewpoint.Bitmaps[BitmapIndex].Normal.Y;
            //    ReturnObject.Viewpoint.snapshot.normal.z = GivenPhysicalViewpoint.Bitmaps[BitmapIndex].Normal.Z;
            //    if (Container.Topics.FirstOrDefault(Curr => Curr.Markup.Viewpoints.Any(VP => VP.Guid == GivenPhysicalViewpoint.GUID)).ViewpointSnapshots.ContainsKey(GivenPhysicalViewpoint.GUID))
            //    {
            //        ReturnObject.Snapshot = Container.Topics.FirstOrDefault(Curr => Curr.Markup.Viewpoints.Any(VP => VP.Guid == GivenPhysicalViewpoint.GUID)).ViewpointSnapshots[GivenPhysicalViewpoint.GUID];
            //    }
            //}
            return ReturnObject;
        }

        private static component_GET GetSingleViewpointComponent(Component GivenComponent)
        {
            var ReturnObject = new component_GET();
            ReturnObject.authoring_tool_id = GivenComponent.AuthoringToolId;
            ReturnObject.color = GivenComponent.Color == null ? null : BitConverter.ToString(GivenComponent.Color).Replace("-", "");
            ReturnObject.ifc_guid = GivenComponent.IfcGuid;
            ReturnObject.originating_system = GivenComponent.OriginatingSystem;
            ReturnObject.selected = GivenComponent.SelectedSpecified ? GivenComponent.Selected : false;
            ReturnObject.visible = GivenComponent.Visible;
            return ReturnObject;
        }

        private static clipping_plane GetViewpointClippingPlane(ClippingPlane GivenClippingPlane)
        {
            var ReturnObject = new clipping_plane();
            ReturnObject.direction = new PointOrVector();
            ReturnObject.direction.x = GivenClippingPlane.Direction.X;
            ReturnObject.direction.y = GivenClippingPlane.Direction.Y;
            ReturnObject.direction.z = GivenClippingPlane.Direction.Z;
            ReturnObject.location = new PointOrVector();
            ReturnObject.location.x = GivenClippingPlane.Location.X;
            ReturnObject.location.y = GivenClippingPlane.Location.Y;
            ReturnObject.location.z = GivenClippingPlane.Location.Z;
            return ReturnObject;
        }

        private static line GetViewpointLine(Line GivenLine)
        {
            var ReturnObject = new line();
            ReturnObject.start_point = new PointOrVector();
            ReturnObject.start_point.x = GivenLine.StartPoint.X;
            ReturnObject.start_point.y = GivenLine.StartPoint.Y;
            ReturnObject.start_point.z = GivenLine.StartPoint.Z;
            ReturnObject.end_point = new PointOrVector();
            ReturnObject.end_point.x = GivenLine.EndPoint.X;
            ReturnObject.end_point.y = GivenLine.EndPoint.Y;
            ReturnObject.end_point.z = GivenLine.EndPoint.Z;
            return ReturnObject;
        }

        private static orthogonal_camera GetViewpointOrthogonalCamera(OrthogonalCamera GivenOrthogonalCamera)
        {
            var ReturnObject = new orthogonal_camera();
            ReturnObject.camera_direction = new PointOrVector();
            ReturnObject.camera_direction.x = GivenOrthogonalCamera.CameraDirection.X;
            ReturnObject.camera_direction.y = GivenOrthogonalCamera.CameraDirection.Y;
            ReturnObject.camera_direction.z = GivenOrthogonalCamera.CameraDirection.Z;
            ReturnObject.camera_up_vector = new PointOrVector();
            ReturnObject.camera_up_vector.x = GivenOrthogonalCamera.CameraUpVector.X;
            ReturnObject.camera_up_vector.y = GivenOrthogonalCamera.CameraUpVector.Y;
            ReturnObject.camera_up_vector.z = GivenOrthogonalCamera.CameraUpVector.Z;
            ReturnObject.camera_view_point = new PointOrVector();
            ReturnObject.camera_view_point.x = GivenOrthogonalCamera.CameraViewPoint.X;
            ReturnObject.camera_view_point.y = GivenOrthogonalCamera.CameraViewPoint.Y;
            ReturnObject.camera_view_point.z = GivenOrthogonalCamera.CameraViewPoint.Z;
            ReturnObject.view_to_world_scale = GivenOrthogonalCamera.ViewToWorldScale;
            return ReturnObject;
        }

        private static perspective_camera GetViewpointPerspectiveCamera(PerspectiveCamera GivenPerspectiveCamera)
        {
            var ReturnObject = new perspective_camera();
            ReturnObject.camera_direction = new PointOrVector();
            ReturnObject.camera_direction.x = GivenPerspectiveCamera.CameraDirection.X;
            ReturnObject.camera_direction.y = GivenPerspectiveCamera.CameraDirection.Y;
            ReturnObject.camera_direction.z = GivenPerspectiveCamera.CameraDirection.Z;
            ReturnObject.camera_up_vector = new PointOrVector();
            ReturnObject.camera_up_vector.x = GivenPerspectiveCamera.CameraUpVector.X;
            ReturnObject.camera_up_vector.y = GivenPerspectiveCamera.CameraUpVector.Y;
            ReturnObject.camera_up_vector.z = GivenPerspectiveCamera.CameraUpVector.Z;
            ReturnObject.camera_view_point = new PointOrVector();
            ReturnObject.camera_view_point.x = GivenPerspectiveCamera.CameraViewPoint.X;
            ReturnObject.camera_view_point.y = GivenPerspectiveCamera.CameraViewPoint.Y;
            ReturnObject.camera_view_point.z = GivenPerspectiveCamera.CameraViewPoint.Z;
            ReturnObject.field_of_view = GivenPerspectiveCamera.FieldOfView;
            return ReturnObject;
        }
    }
}