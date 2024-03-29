using System;
using System.Collections.Generic;
using System.Linq;
using Dangl.BCF.BCFv21;

namespace Dangl.BCF.Converter
{
    public class V2ToV21
    {
        private readonly BCFv2.BCFv2Container _source;
        private BCFv21Container _destination;

        public V2ToV21(BCFv2.BCFv2Container source)
        {
            _source = source;
        }

        public BCFv21.BCFv21Container Convert()
        {
            if (_destination != null)
            {
                return _destination;
            }

            _destination = new BCFv21Container();

            GetBcfProject();
            GetFileAttachments();
            GetProjectExtensions();
            GetTopics();

            return _destination;
        }

        private void GetBcfProject()
        {
            _destination.BcfProject = new BCFv21.Schemas.ProjectExtension
            {
                ExtensionSchema = _source.BcfProject?.ExtensionSchema
            };

            if (_source.BcfProject?.ShouldSerializeProject() == true)
            {
                _destination.BcfProject.Project = new BCFv21.Schemas.Project
                {
                    Name = _source.BcfProject.Project.Name,
                    ProjectId = _source.BcfProject.Project.ProjectId
                };
            }
        }

        private void GetFileAttachments()
        {
            if (_source.FileAttachments?.Any() == true)
            {
                foreach (var entry in _source.FileAttachments)
                {
                    _destination.FileAttachments.Add(entry.Key, entry.Value);
                }
            }
        }

        private void GetProjectExtensions()
        {
            if (_source.ProjectExtensions == null)
            {
                return;
            }

            _destination.ProjectExtensions = new ProjectExtensions();

            if (_source.ProjectExtensions.Priority?.Any() == true)
            {
                _destination.ProjectExtensions.Priority.AddRange(_source.ProjectExtensions.Priority);
            }
            if (_source.ProjectExtensions.SnippetType?.Any() == true)
            {
                _destination.ProjectExtensions.SnippetType.AddRange(_source.ProjectExtensions.SnippetType);
            }
            if (_source.ProjectExtensions.TopicLabel?.Any() == true)
            {
                _destination.ProjectExtensions.TopicLabel.AddRange(_source.ProjectExtensions.TopicLabel);
            }
            if (_source.ProjectExtensions.TopicStatus?.Any() == true)
            {
                _destination.ProjectExtensions.TopicStatus.AddRange(_source.ProjectExtensions.TopicStatus);
            }
            if (_source.ProjectExtensions.TopicType?.Any() == true)
            {
                _destination.ProjectExtensions.TopicType.AddRange(_source.ProjectExtensions.TopicType);
            }
            if (_source.ProjectExtensions.UserIdType?.Any() == true)
            {
                _destination.ProjectExtensions.UserIdType.AddRange(_source.ProjectExtensions.UserIdType);
            }
        }

        private void GetTopics()
        {
            if (_source.Topics == null)
            {
                return;
            }

            foreach (var sourceTopic in _source.Topics)
            {
                GetSingleTopic(sourceTopic);
            }
        }

        private void GetSingleTopic(BCFv2.BCFTopic sourceTopic)
        {
            var topic = new BCFv21.BCFTopic();

            topic.SnippetData = sourceTopic.SnippetData;

            topic.Markup = GetMarkup(sourceTopic);

            GetTopicViewpoints(topic, sourceTopic);

            _destination.Topics.Add(topic);
        }

        private BCFv21.Schemas.Markup GetMarkup(BCFv2.BCFTopic sourceTopic)
        {
            if (sourceTopic.Markup == null)
            {
                return null;
            }

            var markup = new BCFv21.Schemas.Markup();

            GetMarkupHeader(markup, sourceTopic);
            GetMarkupComment(markup, sourceTopic);
            GetMarkupTopic(markup, sourceTopic);
            GetMarkupViewpoints(markup, sourceTopic);

            return markup;
        }

        private void GetMarkupHeader(BCFv21.Schemas.Markup markup, BCFv2.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeHeader())
            {
                return;
            }

            markup.Header = sourceTopic.Markup.Header
                .Select(source => new BCFv21.Schemas.HeaderFile
                {
                    Date = source.Date,
                    Filename = source.Filename,
                    IfcProject = source.IfcProject,
                    IfcSpatialStructureElement = source.IfcSpatialStructureElement,
                    isExternal = source.isExternal,
                    Reference = source.Reference
                })
                .ToList();
        }

        private void GetMarkupComment(BCFv21.Schemas.Markup markup, BCFv2.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeComment())
            {
                return;
            }

            markup.Comment = sourceTopic.Markup.Comment
                .Select(source => new BCFv21.Schemas.Comment
                {
                    Author = source.Author,
                    Comment1 = source.Comment1,
                    Date = source.Date,
                    Guid = source.Guid,
                    ModifiedAuthor = source.ModifiedAuthor,
                    ModifiedDate = source.ModifiedDate,
                    Viewpoint = source.Viewpoint == null
                                ? null
                                : new BCFv21.Schemas.CommentViewpoint
                        {
                            Guid = source.Viewpoint.Guid
                        }
                })
                .ToList();
        }

        private void GetMarkupTopic(BCFv21.Schemas.Markup markup, BCFv2.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeTopic())
            {
                return;
            }

            var srcTopic = sourceTopic.Markup.Topic;

            markup.Topic = new BCFv21.Schemas.Topic();

            {
                
                if (srcTopic.ShouldSerializeAssignedTo())
                {
                    markup.Topic.AssignedTo = srcTopic.AssignedTo;
                }
                if (srcTopic.ShouldSerializeBimSnippet())
                {
                    markup.Topic.BimSnippet = srcTopic.BimSnippet == null
                        ? null
                        : new BCFv21.Schemas.BimSnippet
                        {
                            isExternal = srcTopic.BimSnippet.isExternal,
                            Reference = srcTopic.BimSnippet.Reference,
                            ReferenceSchema = srcTopic.BimSnippet.ReferenceSchema,
                            SnippetType = srcTopic.BimSnippet.SnippetType
                        };
                }
                if (srcTopic.ShouldSerializeCreationAuthor())
                {
                    markup.Topic.CreationAuthor = srcTopic.CreationAuthor;
                }
                if (srcTopic.ShouldSerializeCreationDate())
                {
                    markup.Topic.CreationDate = srcTopic.CreationDate;
                }
                if (srcTopic.ShouldSerializeDescription())
                {
                    markup.Topic.Description = srcTopic.Description;
                }
                if (srcTopic.ShouldSerializeDocumentReferences())
                {
                    markup.Topic.DocumentReference = srcTopic.DocumentReferences?
                        .Select(src => new BCFv21.Schemas.TopicDocumentReference
                        {
                            Description = src.Description,
                            Guid = src.Guid,
                            isExternal = src.isExternal,
                            ReferencedDocument = src.ReferencedDocument
                        })
                        .ToList();
                }
                if (srcTopic.ShouldSerializeGuid())
                {
                    markup.Topic.Guid = srcTopic.Guid;
                }
                if (srcTopic.ShouldSerializeLabels())
                {
                    markup.Topic.Labels = srcTopic.Labels?.ToList();
                }
                if (srcTopic.ShouldSerializeModifiedAuthor())
                {
                    markup.Topic.ModifiedAuthor = srcTopic.ModifiedAuthor;
                }
                if (srcTopic.ShouldSerializeModifiedDate())
                {
                    markup.Topic.ModifiedDate = srcTopic.ModifiedDate;
                }
                if (srcTopic.ShouldSerializePriority())
                {
                    markup.Topic.Priority = srcTopic.Priority;
                }
                if (srcTopic.ShouldSerializeReferenceLink())
                {
                    markup.Topic.ReferenceLink = srcTopic.ReferenceLink == null ? null : new List<string> {srcTopic.ReferenceLink};
                }
                if (srcTopic.ShouldSerializeTitle())
                {
                    markup.Topic.Title = srcTopic.Title;
                }
                if (srcTopic.ShouldSerializeRelatedTopics())
                {
                    markup.Topic.RelatedTopic = srcTopic.RelatedTopics?
                        .Select(src => new BCFv21.Schemas.TopicRelatedTopic
                        {
                            Guid = src.Guid
                        })
                        .ToList();
                }
                if (srcTopic.ShouldSerializeTopicStatus())
                {
                    markup.Topic.TopicStatus = srcTopic.TopicStatus;
                }
                if (srcTopic.ShouldSerializeTopicType())
                {
                    markup.Topic.TopicType = srcTopic.TopicType;
                }
            };
        }

        private void GetMarkupViewpoints(BCFv21.Schemas.Markup markup, BCFv2.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeViewpoints())
            {
                return;
            }

            markup.Viewpoints = sourceTopic.Markup.Viewpoints
                .Select((source, index) => new BCFv21.Schemas.ViewPoint
                {
                    Guid = source.Guid,
                    Index = index,
                    Snapshot = source.Snapshot,
                    Viewpoint = source.Viewpoint
                })
                .ToList();
        }

        private void GetTopicViewpoints(BCFv21.BCFTopic topic, BCFv2.BCFTopic sourceTopic)
        {
            if (sourceTopic.Viewpoints?.Any() != true)
            {
                return;
            }

            foreach (var sourceViewpoint in sourceTopic.Viewpoints)
            {
                GetSingleTopicViewpoint(topic, sourceViewpoint);
            }

            if (sourceTopic.ViewpointBitmaps.Any())
            {
                foreach (var sourceBitmap in sourceTopic.ViewpointBitmaps)
                {
                    topic.ViewpointBitmaps.Add(topic.Viewpoints.Single(vp => vp.Guid == sourceBitmap.Key.GUID), sourceBitmap.Value.ToList());
                }
            }

            if (sourceTopic.ViewpointSnapshots.Any())
            {
                foreach (var sourceSnapshot in sourceTopic.ViewpointSnapshots)
                {
                    topic.AddOrUpdateSnapshot(sourceSnapshot.Key, sourceSnapshot.Value);
                }
            }
        }

        private void GetSingleTopicViewpoint(BCFv21.BCFTopic topic, BCFv2.Schemas.VisualizationInfo sourceViewpoint)
        {
            var viewpoint = new BCFv21.Schemas.VisualizationInfo();

            if (sourceViewpoint.ShouldSerializeBitmaps())
            {
                viewpoint.Bitmap = sourceViewpoint.Bitmaps
                    .Select(bitmap => new BCFv21.Schemas.VisualizationInfoBitmap
                    {
                        Height = bitmap.Height,
                        Location = GetV21PointFromV2(bitmap.Location),
                        Normal = GetV21DirectionFromV2(bitmap.Normal),
                        Up = GetV21DirectionFromV2(bitmap.Up),
                        Reference = bitmap.Reference,
                        Bitmap = bitmap.Bitmap == BCFv2.Schemas.BitmapFormat.JPG
                                ? BCFv21.Schemas.BitmapFormat.JPG
                                : BCFv21.Schemas.BitmapFormat.PNG
                    })
                    .ToList();
            }

            if (sourceViewpoint.ShouldSerializeClippingPlanes())
            {
                viewpoint.ClippingPlanes = sourceViewpoint.ClippingPlanes
                    .Select(plane => new BCFv21.Schemas.ClippingPlane
                    {
                        Direction = GetV21DirectionFromV2(plane.Direction),
                        Location = GetV21PointFromV2(plane.Location)
                    })
                    .ToList();
            }

            if (sourceViewpoint.ShouldSerializeComponents())
            {
                viewpoint.Components = GetComponentsForViewpoint(sourceViewpoint);
            }

            if (sourceViewpoint.ShouldSerializeLines())
            {
                viewpoint.Lines = sourceViewpoint.Lines
                    .Select(line => new BCFv21.Schemas.Line
                    {
                        EndPoint = GetV21PointFromV2(line.EndPoint),
                        StartPoint = GetV21PointFromV2(line.StartPoint)
                    })
                    .ToList();
            }

            if (sourceViewpoint.ShouldSerializeOrthogonalCamera())
            {
                viewpoint.OrthogonalCamera = new BCFv21.Schemas.OrthogonalCamera
                {
                    CameraDirection = GetV21DirectionFromV2(sourceViewpoint.OrthogonalCamera.CameraDirection),
                    CameraUpVector = GetV21DirectionFromV2(sourceViewpoint.OrthogonalCamera.CameraUpVector),
                    CameraViewPoint = GetV21PointFromV2(sourceViewpoint.OrthogonalCamera.CameraViewPoint),
                    ViewToWorldScale = sourceViewpoint.OrthogonalCamera.ViewToWorldScale
                };
            }

            if (sourceViewpoint.ShouldSerializePerspectiveCamera())
            {
                viewpoint.PerspectiveCamera = new BCFv21.Schemas.PerspectiveCamera
                {
                    CameraDirection = GetV21DirectionFromV2(sourceViewpoint.PerspectiveCamera.CameraDirection),
                    CameraUpVector = GetV21DirectionFromV2(sourceViewpoint.PerspectiveCamera.CameraUpVector),
                    CameraViewPoint = GetV21PointFromV2(sourceViewpoint.PerspectiveCamera.CameraViewPoint),
                    FieldOfView = sourceViewpoint.PerspectiveCamera.FieldOfView
                };
            }

            viewpoint.Guid = sourceViewpoint.GUID;

            topic.Viewpoints.Add(viewpoint);
        }

        private static BCFv21.Schemas.Components GetComponentsForViewpoint(BCFv2.Schemas.VisualizationInfo sourceViewpoint)
        {
            return new BCFv21.Schemas.Components
            {
                Coloring = sourceViewpoint.Components
                    .Where(comp => comp.Color != null)
                    .GroupBy(comp => comp.Color.ToRgbHexColorString())
                    .Select(comp => new BCFv21.Schemas.ComponentColoringColor
                    {
                        Color = comp.Key,
                        Component = comp.Select(c => new BCFv21.Schemas.Component
                        {
                            AuthoringToolId = c.AuthoringToolId,
                            IfcGuid = c.IfcGuid,
                            OriginatingSystem = c.OriginatingSystem
                        })
                        .ToList()
                    })
                    .ToList(),
                Selection = sourceViewpoint.Components
                    .Where(comp => comp.ShouldSerializeSelected() && comp.Selected)
                    .Select(comp => new BCFv21.Schemas.Component
                    {
                        AuthoringToolId = comp.AuthoringToolId,
                        IfcGuid = comp.IfcGuid,
                        OriginatingSystem = comp.OriginatingSystem
                    })
                    .ToList(),
                Visibility = new BCFv21.Schemas.ComponentVisibility
                {
                    DefaultVisibility = true,
                    Exceptions = sourceViewpoint.Components
                        .Where(comp => comp.ShouldSerializeVisible() && !comp.Visible)
                        .Select(comp => new BCFv21.Schemas.Component
                        {
                            AuthoringToolId = comp.AuthoringToolId,
                            IfcGuid = comp.IfcGuid,
                            OriginatingSystem = comp.OriginatingSystem
                        })
                        .ToList()
                }
            };
        }

        private static BCFv21.Schemas.Point GetV21PointFromV2(BCFv2.Schemas.Point source)
        {
            return new BCFv21.Schemas.Point
            {
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
        }

        private static BCFv21.Schemas.Direction GetV21DirectionFromV2(BCFv2.Schemas.Direction source)
        {
            return new BCFv21.Schemas.Direction
            {
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
        }
    }
}
