using Dangl.BCF.BCFv3;
using System.Collections.Generic;
using System.Linq;

namespace Dangl.BCF.Converter
{
    public class V21ToV3
    {
        private readonly BCFv21.BCFv21Container _source;
        private BCFv3Container _destination;

        public V21ToV3(BCFv21.BCFv21Container source)
        {
            _source = source;
        }

        public BCFv3Container Convert()
        {
            if (_destination != null)
            {
                return _destination;
            }

            _destination = new BCFv3Container();

            GetBcfProject();
            GetFileAttachments();
            GetProjectExtensions();
            GetTopics();

            return _destination;
        }


        private void GetBcfProject()
        {
            _destination.BcfProject = new BCFv3.Schemas.Project();

            if (_source.ProjectExtensions != null)
            {
                _destination.ProjectExtensions = new BCFv3.Schemas.Extensions
                {
                    Priorities = _source.ProjectExtensions.Priority?.ToList(),
                    SnippetTypes = _source.ProjectExtensions.SnippetType?.ToList(),
                    TopicLabels = _source.ProjectExtensions.TopicLabel?.ToList(),
                    TopicStatuses = _source.ProjectExtensions.TopicStatus?.ToList(),
                    TopicTypes = _source.ProjectExtensions.TopicType?.ToList(),
                    Users = _source.ProjectExtensions.UserIdType?.ToList()
                };
            }

            if (!string.IsNullOrWhiteSpace(_source.BcfProject?.Project?.Name) || !string.IsNullOrWhiteSpace(_source.BcfProject?.Project?.ProjectId))
            {
                _destination.BcfProject = new BCFv3.Schemas.Project
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

            _destination.ProjectExtensions = new BCFv3.Schemas.Extensions();

            if (_source.ProjectExtensions.Priority?.Any() == true)
            {
                _destination.ProjectExtensions.Priorities.AddRange(_source.ProjectExtensions.Priority);
            }
            if (_source.ProjectExtensions.SnippetType?.Any() == true)
            {
                _destination.ProjectExtensions.SnippetTypes.AddRange(_source.ProjectExtensions.SnippetType);
            }
            if (_source.ProjectExtensions.TopicLabel?.Any() == true)
            {
                _destination.ProjectExtensions.TopicLabels.AddRange(_source.ProjectExtensions.TopicLabel);
            }
            if (_source.ProjectExtensions.TopicStatus?.Any() == true)
            {
                _destination.ProjectExtensions.TopicStatuses.AddRange(_source.ProjectExtensions.TopicStatus);
            }
            if (_source.ProjectExtensions.TopicType?.Any() == true)
            {
                _destination.ProjectExtensions.TopicTypes.AddRange(_source.ProjectExtensions.TopicType);
            }
            if (_source.ProjectExtensions.UserIdType?.Any() == true)
            {
                _destination.ProjectExtensions.Users.AddRange(_source.ProjectExtensions.UserIdType);
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

        private void GetSingleTopic(BCFv21.BCFTopic sourceTopic)
        {
            var topic = new BCFv3.BCFTopic();

            topic.SnippetData = sourceTopic.SnippetData;

            topic.Markup = GetMarkup(sourceTopic);

            GetTopicViewpoints(topic, sourceTopic);

            _destination.Topics.Add(topic);
        }

        private BCFv3.Schemas.Markup GetMarkup(BCFv21.BCFTopic sourceTopic)
        {
            if (sourceTopic.Markup == null)
            {
                return null;
            }

            var markup = new BCFv3.Schemas.Markup();

            GetMarkupHeader(markup, sourceTopic);
            GetMarkupComment(markup, sourceTopic);
            GetMarkupTopic(markup, sourceTopic);
            GetMarkupViewpoints(markup, sourceTopic);

            return markup;
        }

        private void GetMarkupHeader(BCFv3.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeHeader())
            {
                return;
            }

            markup.Header.Files = sourceTopic.Markup.Header
                .Select(source => new BCFv3.Schemas.File
                {
                    Date = source.Date,
                    Filename = source.Filename,
                    IfcProject = source.IfcProject,
                    IfcSpatialStructureElement = source.IfcSpatialStructureElement,
                    IsExternal = source.isExternal,
                    Reference = source.Reference
                })
                .ToList();
        }

        private void GetMarkupComment(BCFv3.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeComment())
            {
                return;
            }

            markup.Topic.Comments = sourceTopic.Markup.Comment
                .Select(source => new BCFv3.Schemas.Comment
                {
                    Author = source.Author,
                    Comment1 = source.Comment1,
                    Date = source.Date,
                    Guid = source.Guid,
                    ModifiedAuthor = source.ModifiedAuthor,
                    ModifiedDate = source.ModifiedDate,
                    Viewpoint = source.Viewpoint == null
                                ? null
                                : new BCFv3.Schemas.CommentViewpoint
                                {
                                    Guid = source.Viewpoint.Guid
                                }
                })
                .ToList();
        }

        private void GetMarkupTopic(BCFv3.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeTopic())
            {
                return;
            }

            var srcTopic = sourceTopic.Markup.Topic;

            markup.Topic = new BCFv3.Schemas.Topic();
            {
                if (srcTopic.ShouldSerializeAssignedTo())
                {
                    markup.Topic.AssignedTo = srcTopic.AssignedTo;
                }
                if (srcTopic.ShouldSerializeBimSnippet())
                {
                    markup.Topic.BimSnippet = srcTopic.BimSnippet == null
                        ? null
                        : new BCFv3.Schemas.BimSnippet
                        {
                            IsExternal = srcTopic.BimSnippet.isExternal,
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

                if (srcTopic.ShouldSerializeDocumentReference())
                {
                    markup.Topic.DocumentReferences = srcTopic.DocumentReference
                        .Select(src => new BCFv3.Schemas.DocumentReference
                        {
                            Description = src.Description,
                            Guid = src.Guid,
                            Item = src.ReferencedDocument,
                            ItemElementName = src.isExternal
                                ? BCFv3.Schemas.ItemChoiceType.Url
                                : BCFv3.Schemas.ItemChoiceType.DocumentGuid
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
                if (srcTopic.ShouldSerializeReferenceLink() && srcTopic.ReferenceLink.Any())
                {
                    markup.Topic.ReferenceLinks.AddRange(srcTopic.ReferenceLink);
                }
                if (srcTopic.ShouldSerializeTitle())
                {
                    markup.Topic.Title = srcTopic.Title;
                }
                if (srcTopic.ShouldSerializeRelatedTopic())
                {
                    markup.Topic.RelatedTopics = srcTopic.RelatedTopic?
                        .Select(src => new BCFv3.Schemas.TopicRelatedTopic
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

        private void GetMarkupViewpoints(BCFv3.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeViewpoints())
            {
                return;
            }

            // The source viewpoints have an 'Index' property that could be used
            // for ordering. However, this isn't often used in practice, so it's
            // safer to simply use the same ordering as in the original input file
            markup.Topic.Viewpoints = sourceTopic.Markup.Viewpoints
                .Select((source, index) => new BCFv3.Schemas.ViewPoint
                {
                    Guid = source.Guid,
                    Snapshot = source.Snapshot,
                    Viewpoint = source.Viewpoint,
                    Index = index
                })
                .ToList();
        }

        private void GetTopicViewpoints(BCFv3.BCFTopic topic, BCFv21.BCFTopic sourceTopic)
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
                    topic.ViewpointBitmaps.Add(topic.Viewpoints.Single(vp => vp.Guid == sourceBitmap.Key.Guid), sourceBitmap.Value.ToList());
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

        private void GetSingleTopicViewpoint(BCFv3.BCFTopic topic, BCFv21.Schemas.VisualizationInfo sourceViewpoint)
        {
            var viewpoint = new BCFv3.Schemas.VisualizationInfo();

            if (sourceViewpoint.ShouldSerializeBitmap())
            {
                viewpoint.Bitmaps = sourceViewpoint.Bitmap
                    .Select(bitmap => new BCFv3.Schemas.Bitmap
                    {
                        Height = bitmap.Height,
                        Location = GetV3PointFromV21(bitmap.Location),
                        Normal = GetV3DirectionFromV21(bitmap.Normal),
                        Up = GetV3DirectionFromV21(bitmap.Up),
                        Reference = bitmap.Reference,
                        Format = bitmap.Bitmap == BCFv21.Schemas.BitmapFormat.JPG
                                ? BCFv3.Schemas.BitmapFormat.jpg
                                : BCFv3.Schemas.BitmapFormat.png
                    })
                    .ToList();
            }

            if (sourceViewpoint.ShouldSerializeClippingPlanes())
            {
                viewpoint.ClippingPlanes = sourceViewpoint.ClippingPlanes
                    .Select(plane => new BCFv3.Schemas.ClippingPlane
                    {
                        Direction = GetV3DirectionFromV21(plane.Direction),
                        Location = GetV3PointFromV21(plane.Location)
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
                    .Select(line => new BCFv3.Schemas.Line
                    {
                        EndPoint = GetV3PointFromV21(line.EndPoint),
                        StartPoint = GetV3PointFromV21(line.StartPoint)
                    })
                    .ToList();
            }

            if (sourceViewpoint.ShouldSerializeOrthogonalCamera())
            {
                viewpoint.Item = new BCFv3.Schemas.OrthogonalCamera
                {
                    CameraDirection = GetV3DirectionFromV21(sourceViewpoint.OrthogonalCamera.CameraDirection),
                    CameraUpVector = GetV3DirectionFromV21(sourceViewpoint.OrthogonalCamera.CameraUpVector),
                    CameraViewPoint = GetV3PointFromV21(sourceViewpoint.OrthogonalCamera.CameraViewPoint),
                    ViewToWorldScale = sourceViewpoint.OrthogonalCamera.ViewToWorldScale
                };
            }

            if (sourceViewpoint.ShouldSerializePerspectiveCamera())
            {
                viewpoint.Item = new BCFv3.Schemas.PerspectiveCamera
                {
                    CameraDirection = GetV3DirectionFromV21(sourceViewpoint.PerspectiveCamera.CameraDirection),
                    CameraUpVector = GetV3DirectionFromV21(sourceViewpoint.PerspectiveCamera.CameraUpVector),
                    CameraViewPoint = GetV3PointFromV21(sourceViewpoint.PerspectiveCamera.CameraViewPoint),
                    FieldOfView = sourceViewpoint.PerspectiveCamera.FieldOfView
                };
            }

            viewpoint.Guid = sourceViewpoint.Guid;

            topic.Viewpoints.Add(viewpoint);
        }

        private static BCFv3.Schemas.Components GetComponentsForViewpoint(BCFv21.Schemas.VisualizationInfo sourceViewpoint)
        {
            var components = new BCFv3.Schemas.Components();

            // The 'ViewSetupHints' property is ignored

            if (!sourceViewpoint.ShouldSerializeComponents())
            {
                return components;
            }

            if (sourceViewpoint.Components.ShouldSerializeColoring())
            {
                var srcColoring = sourceViewpoint.Components.Coloring;
                foreach (var srcColor in srcColoring)
                {
                    components.Coloring.Add(new BCFv3.Schemas.ComponentColoringColor
                    {
                        Color = srcColor.Color,
                        Components = srcColor.Component.Select(c => new BCFv3.Schemas.Component
                            {
                                AuthoringToolId = c.AuthoringToolId,
                                IfcGuid = c.IfcGuid,
                                OriginatingSystem = c.OriginatingSystem,
                            })
                            .ToList()
                    });
                }
            }

            if (sourceViewpoint.Components.ShouldSerializeSelection())
            {
                var srcSelection = sourceViewpoint.Components.Selection;
                components.Selection.AddRange(srcSelection.Select(s => new BCFv3.Schemas.Component
                {
                    AuthoringToolId = s.AuthoringToolId,
                    IfcGuid = s.IfcGuid,
                    OriginatingSystem = s.OriginatingSystem,
                }));
            }

            if (sourceViewpoint.Components.ShouldSerializeVisibility())
            {
                var srcVisibility = sourceViewpoint.Components.Visibility;

                components.Visibility = new BCFv3.Schemas.ComponentVisibility
                {
                    DefaultVisibility = srcVisibility.DefaultVisibility,
                    Exceptions = srcVisibility.Exceptions.Select(e => new BCFv3.Schemas.Component
                    {
                        AuthoringToolId = e.AuthoringToolId,
                        IfcGuid = e.IfcGuid,
                        OriginatingSystem = e.OriginatingSystem,
                    })
                    .ToList()
                };
            }

            return components;
        }

        private static BCFv3.Schemas.Point GetV3PointFromV21(BCFv21.Schemas.Point source)
        {
            return new BCFv3.Schemas.Point
            {
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
        }

        private static BCFv3.Schemas.Direction GetV3DirectionFromV21(BCFv21.Schemas.Direction source)
        {
            return new BCFv3.Schemas.Direction
            {
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
        }
    }
}
