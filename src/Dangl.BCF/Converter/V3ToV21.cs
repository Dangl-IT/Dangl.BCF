using Dangl.BCF.BCFv21;
using System.Collections.Generic;
using System.Linq;

namespace Dangl.BCF.Converter
{
    public class V3ToV21
    {
        private readonly BCFv3.BCFv3Container _source;
        private BCFv21Container _destination;

        public V3ToV21(BCFv3.BCFv3Container source)
        {
            _source = source;
        }

        public BCFv21Container Convert()
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
            _destination.BcfProject = new BCFv21.Schemas.ProjectExtension();

            if (_source.ProjectExtensions != null)
            {
                _destination.ProjectExtensions = new ProjectExtensions
                {
                    Priority = _source.ProjectExtensions.Priorities?.ToList(),
                    SnippetType = _source.ProjectExtensions.SnippetTypes?.ToList(),
                    TopicLabel = _source.ProjectExtensions.TopicLabels?.ToList(),
                    TopicStatus = _source.ProjectExtensions.TopicStatuses?.ToList(),
                    TopicType = _source.ProjectExtensions.TopicTypes?.ToList(),
                    UserIdType = _source.ProjectExtensions.Users?.ToList()
                };
            }

            if (!string.IsNullOrWhiteSpace(_source.BcfProject?.Name) || !string.IsNullOrWhiteSpace(_source.BcfProject?.ProjectId))
            {
                _destination.BcfProject ??= new BCFv21.Schemas.ProjectExtension();
                _destination.BcfProject.Project = new BCFv21.Schemas.Project
                {
                    Name = _source.BcfProject.Name,
                    ProjectId = _source.BcfProject.ProjectId
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

            if (_source.ProjectExtensions.Priorities?.Any() == true)
            {
                _destination.ProjectExtensions.Priority.AddRange(_source.ProjectExtensions.Priorities);
            }
            if (_source.ProjectExtensions.SnippetTypes?.Any() == true)
            {
                _destination.ProjectExtensions.SnippetType.AddRange(_source.ProjectExtensions.SnippetTypes);
            }
            if (_source.ProjectExtensions.TopicLabels?.Any() == true)
            {
                _destination.ProjectExtensions.TopicLabel.AddRange(_source.ProjectExtensions.TopicLabels);
            }
            if (_source.ProjectExtensions.TopicStatuses?.Any() == true)
            {
                _destination.ProjectExtensions.TopicStatus.AddRange(_source.ProjectExtensions.TopicStatuses);
            }
            if (_source.ProjectExtensions.TopicTypes?.Any() == true)
            {
                _destination.ProjectExtensions.TopicType.AddRange(_source.ProjectExtensions.TopicTypes);
            }
            if (_source.ProjectExtensions.Users?.Any() == true)
            {
                _destination.ProjectExtensions.UserIdType.AddRange(_source.ProjectExtensions.Users);
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

        private void GetSingleTopic(BCFv3.BCFTopic sourceTopic)
        {
            var topic = new BCFv21.BCFTopic();

            topic.SnippetData = sourceTopic.SnippetData;

            topic.Markup = GetMarkup(sourceTopic);

            GetTopicViewpoints(topic, sourceTopic);

            _destination.Topics.Add(topic);
        }

        private BCFv21.Schemas.Markup GetMarkup(BCFv3.BCFTopic sourceTopic)
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

        private void GetMarkupHeader(BCFv21.Schemas.Markup markup, BCFv3.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeHeader() || !sourceTopic.Markup.Header.ShouldSerializeFiles())
            {
                return;
            }

            markup.Header = sourceTopic.Markup.Header
                .Files
                .Select(source => new BCFv21.Schemas.HeaderFile
                {
                    Date = source.Date,
                    Filename = source.Filename,
                    IfcProject = source.IfcProject,
                    IfcSpatialStructureElement = source.IfcSpatialStructureElement,
                    isExternal = source.IsExternal,
                    Reference = source.Reference
                })
                .ToList();
        }

        private void GetMarkupComment(BCFv21.Schemas.Markup markup, BCFv3.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.Topic.ShouldSerializeComments())
            {
                return;
            }

            markup.Comment = sourceTopic.Markup.Topic.Comments
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

        private void GetMarkupTopic(BCFv21.Schemas.Markup markup, BCFv3.BCFTopic sourceTopic)
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
                            isExternal = srcTopic.BimSnippet.IsExternal,
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
                    markup.Topic.DocumentReference = srcTopic.DocumentReferences
                        .Select(src => new BCFv21.Schemas.TopicDocumentReference
                        {
                            Description = src.Description,
                            Guid = src.Guid,
                            isExternal = src.ItemElementName == BCFv3.Schemas.ItemChoiceType.Url,
                            ReferencedDocument = src.Item
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
                if (srcTopic.ShouldSerializeReferenceLinks() && srcTopic.ReferenceLinks.Any())
                {
                    markup.Topic.ReferenceLink.AddRange(srcTopic.ReferenceLinks);
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

        private void GetMarkupViewpoints(BCFv21.Schemas.Markup markup, BCFv3.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.Topic.ShouldSerializeViewpoints())
            {
                return;
            }

            // The source viewpoints have an 'Index' property that could be used
            // for ordering. However, this isn't often used in practice, so it's
            // safer to simply use the same ordering as in the original input file
            markup.Viewpoints = sourceTopic.Markup.Topic.Viewpoints
                .Select((source, index) => new BCFv21.Schemas.ViewPoint
                {
                    Guid = source.Guid,
                    Snapshot = source.Snapshot,
                    Viewpoint = source.Viewpoint
                })
                .ToList();
        }

        private void GetTopicViewpoints(BCFv21.BCFTopic topic, BCFv3.BCFTopic sourceTopic)
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

        private void GetSingleTopicViewpoint(BCFv21.BCFTopic topic, BCFv3.Schemas.VisualizationInfo sourceViewpoint)
        {
            var viewpoint = new BCFv21.Schemas.VisualizationInfo();

            if (sourceViewpoint.ShouldSerializeBitmaps())
            {
                viewpoint.Bitmap = sourceViewpoint.Bitmaps
                    .Select(bitmap => new BCFv21.Schemas.VisualizationInfoBitmap
                    {
                        Height = bitmap.Height,
                        Location = GetV21PointFromV3(bitmap.Location),
                        Normal = GetV21DirectionFromV3(bitmap.Normal),
                        Up = GetV21DirectionFromV3(bitmap.Up),
                        Reference = bitmap.Reference,
                        Bitmap = bitmap.Format == BCFv3.Schemas.BitmapFormat.jpg
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
                        Direction = GetV21DirectionFromV3(plane.Direction),
                        Location = GetV21PointFromV3(plane.Location)
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
                        EndPoint = GetV21PointFromV3(line.EndPoint),
                        StartPoint = GetV21PointFromV3(line.StartPoint)
                    })
                    .ToList();
            }

            if (sourceViewpoint.Item is BCFv3.Schemas.OrthogonalCamera orthogonalCamera)
            {
                viewpoint.OrthogonalCamera = new BCFv21.Schemas.OrthogonalCamera
                {
                    CameraDirection = GetV21DirectionFromV3(orthogonalCamera.CameraDirection),
                    CameraUpVector = GetV21DirectionFromV3(orthogonalCamera.CameraUpVector),
                    CameraViewPoint = GetV21PointFromV3(orthogonalCamera.CameraViewPoint),
                    ViewToWorldScale = orthogonalCamera.ViewToWorldScale
                };
            }

            if (sourceViewpoint.Item is BCFv3.Schemas.PerspectiveCamera perspectiveCamera)
            {
                viewpoint.PerspectiveCamera = new BCFv21.Schemas.PerspectiveCamera
                {
                    CameraDirection = GetV21DirectionFromV3(perspectiveCamera.CameraDirection),
                    CameraUpVector = GetV21DirectionFromV3(perspectiveCamera.CameraUpVector),
                    CameraViewPoint = GetV21PointFromV3(perspectiveCamera.CameraViewPoint),
                    FieldOfView = perspectiveCamera.FieldOfView
                };
            }

            viewpoint.Guid = sourceViewpoint.Guid;

            topic.Viewpoints.Add(viewpoint);
        }

        private static BCFv21.Schemas.Components GetComponentsForViewpoint(BCFv3.Schemas.VisualizationInfo sourceViewpoint)
        {
            var components = new BCFv21.Schemas.Components();

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
                    components.Coloring.Add(new BCFv21.Schemas.ComponentColoringColor
                    {
                        Color = srcColor.Color,
                        Component = srcColor.Components.Select(c => new BCFv21.Schemas.Component
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
                components.Selection.AddRange(srcSelection.Select(s => new BCFv21.Schemas.Component
                {
                    AuthoringToolId = s.AuthoringToolId,
                    IfcGuid = s.IfcGuid,
                    OriginatingSystem = s.OriginatingSystem,
                }));
            }

            if (sourceViewpoint.Components.ShouldSerializeVisibility())
            {
                var srcVisibility = sourceViewpoint.Components.Visibility;

                components.Visibility = new BCFv21.Schemas.ComponentVisibility
                {
                    DefaultVisibility = srcVisibility.DefaultVisibility,
                    Exceptions = srcVisibility.Exceptions.Select(e => new BCFv21.Schemas.Component
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

        private static BCFv21.Schemas.Point GetV21PointFromV3(BCFv3.Schemas.Point source)
        {
            return new BCFv21.Schemas.Point
            {
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
        }

        private static BCFv21.Schemas.Direction GetV21DirectionFromV3(BCFv3.Schemas.Direction source)
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
