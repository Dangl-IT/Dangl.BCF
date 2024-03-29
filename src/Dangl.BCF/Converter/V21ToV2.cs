using Dangl.BCF.BCFv2;
using System.Collections.Generic;
using System.Linq;

namespace Dangl.BCF.Converter
{
    public class V21ToV2
    {
        private readonly BCFv21.BCFv21Container _source;
        private BCFv2Container _destination;

        public V21ToV2(BCFv21.BCFv21Container source)
        {
            _source = source;
        }

        // TODO ADD ROUNDTRIP TESTS TO INTEGRATION TESTS
        public BCFv2Container Convert()
        {
            if (_destination != null)
            {
                return _destination;
            }

            _destination = new BCFv2Container();

            GetBcfProject();
            GetFileAttachments();
            GetProjectExtensions();
            GetTopics();

            return _destination;
        }


        private void GetBcfProject()
        {
            _destination.BcfProject = new BCFv2.Schemas.ProjectExtension
            {
                ExtensionSchema = _source.BcfProject?.ExtensionSchema
            };

            if (_source.BcfProject?.ShouldSerializeProject() == true)
            {
                _destination.BcfProject.Project = new BCFv2.Schemas.Project
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

        private void GetSingleTopic(BCFv21.BCFTopic sourceTopic)
        {
            var topic = new BCFv2.BCFTopic();

            topic.SnippetData = sourceTopic.SnippetData;

            topic.Markup = GetMarkup(sourceTopic);

            GetTopicViewpoints(topic, sourceTopic);

            _destination.Topics.Add(topic);
        }

        private BCFv2.Schemas.Markup GetMarkup(BCFv21.BCFTopic sourceTopic)
        {
            if (sourceTopic.Markup == null)
            {
                return null;
            }

            var markup = new BCFv2.Schemas.Markup();

            GetMarkupHeader(markup, sourceTopic);
            GetMarkupComment(markup, sourceTopic);
            GetMarkupTopic(markup, sourceTopic);
            GetMarkupViewpoints(markup, sourceTopic);

            return markup;
        }

        private void GetMarkupHeader(BCFv2.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeHeader())
            {
                return;
            }

            markup.Header = sourceTopic.Markup.Header
                .Select(source => new BCFv2.Schemas.HeaderFile
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

        private void GetMarkupComment(BCFv2.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeComment())
            {
                return;
            }

            markup.Comment = sourceTopic.Markup.Comment
                .Select(source => new BCFv2.Schemas.Comment
                {
                    Author = source.Author,
                    Comment1 = source.Comment1,
                    Date = source.Date,
                    Guid = source.Guid,
                    ModifiedAuthor = source.ModifiedAuthor,
                    ModifiedDate = source.ModifiedDate,
                    Viewpoint = source.Viewpoint == null
                                ? null
                                : new BCFv2.Schemas.CommentViewpoint
                                {
                                    Guid = source.Viewpoint.Guid
                                }
                })
                .ToList();
        }

        private void GetMarkupTopic(BCFv2.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeTopic())
            {
                return;
            }

            var srcTopic = sourceTopic.Markup.Topic;

            markup.Topic = new BCFv2.Schemas.Topic();

            {

                if (srcTopic.ShouldSerializeAssignedTo())
                {
                    markup.Topic.AssignedTo = srcTopic.AssignedTo;
                }
                if (srcTopic.ShouldSerializeBimSnippet())
                {
                    markup.Topic.BimSnippet = srcTopic.BimSnippet == null
                        ? null
                        : new BCFv2.Schemas.BimSnippet
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

                if (srcTopic.ShouldSerializeDocumentReference())
                {
                    markup.Topic.DocumentReferences = srcTopic.DocumentReference
                        .Select(src => new BCFv2.Schemas.TopicDocumentReferences
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
                if (srcTopic.ShouldSerializeReferenceLink() && srcTopic.ReferenceLink.Any())
                {
                    markup.Topic.ReferenceLink = srcTopic.ReferenceLink.First();
                }
                if (srcTopic.ShouldSerializeTitle())
                {
                    markup.Topic.Title = srcTopic.Title;
                }
                if (srcTopic.ShouldSerializeRelatedTopic())
                {
                    markup.Topic.RelatedTopics = srcTopic.RelatedTopic?
                        .Select(src => new BCFv2.Schemas.TopicRelatedTopics
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

        private void GetMarkupViewpoints(BCFv2.Schemas.Markup markup, BCFv21.BCFTopic sourceTopic)
        {
            if (!sourceTopic.Markup.ShouldSerializeViewpoints())
            {
                return;
            }

            // The source viewpoints have an 'Index' property that could be used
            // for ordering. However, this isn't often used in practice, so it's
            // safer to simply use the same ordering as in the original input file
            markup.Viewpoints = sourceTopic.Markup.Viewpoints
                .Select((source, index) => new BCFv2.Schemas.ViewPoint
                {
                    Guid = source.Guid,
                    Snapshot = source.Snapshot,
                    Viewpoint = source.Viewpoint
                })
                .ToList();
        }

        private void GetTopicViewpoints(BCFv2.BCFTopic topic, BCFv21.BCFTopic sourceTopic)
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
                    topic.ViewpointBitmaps.Add(topic.Viewpoints.Single(vp => vp.GUID == sourceBitmap.Key.Guid), sourceBitmap.Value.ToList());
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

        private void GetSingleTopicViewpoint(BCFv2.BCFTopic topic, BCFv21.Schemas.VisualizationInfo sourceViewpoint)
        {
            var viewpoint = new BCFv2.Schemas.VisualizationInfo();

            if (sourceViewpoint.ShouldSerializeBitmap())
            {
                viewpoint.Bitmaps = sourceViewpoint.Bitmap
                    .Select(bitmap => new BCFv2.Schemas.VisualizationInfoBitmaps
                    {
                        Height = bitmap.Height,
                        Location = GetV2PointFromV21(bitmap.Location),
                        Normal = GetV2DirectionFromV21(bitmap.Normal),
                        Up = GetV2DirectionFromV21(bitmap.Up),
                        Reference = bitmap.Reference,
                        Bitmap = bitmap.Bitmap == BCFv21.Schemas.BitmapFormat.JPG
                                ? BCFv2.Schemas.BitmapFormat.JPG
                                : BCFv2.Schemas.BitmapFormat.PNG
                    })
                    .ToList();
            }

            if (sourceViewpoint.ShouldSerializeClippingPlanes())
            {
                viewpoint.ClippingPlanes = sourceViewpoint.ClippingPlanes
                    .Select(plane => new BCFv2.Schemas.ClippingPlane
                    {
                        Direction = GetV2DirectionFromV21(plane.Direction),
                        Location = GetV2PointFromV21(plane.Location)
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
                    .Select(line => new BCFv2.Schemas.Line
                    {
                        EndPoint = GetV2PointFromV21(line.EndPoint),
                        StartPoint = GetV2PointFromV21(line.StartPoint)
                    })
                    .ToList();
            }

            if (sourceViewpoint.ShouldSerializeOrthogonalCamera())
            {
                viewpoint.OrthogonalCamera = new BCFv2.Schemas.OrthogonalCamera
                {
                    CameraDirection = GetV2DirectionFromV21(sourceViewpoint.OrthogonalCamera.CameraDirection),
                    CameraUpVector = GetV2DirectionFromV21(sourceViewpoint.OrthogonalCamera.CameraUpVector),
                    CameraViewPoint = GetV2PointFromV21(sourceViewpoint.OrthogonalCamera.CameraViewPoint),
                    ViewToWorldScale = sourceViewpoint.OrthogonalCamera.ViewToWorldScale
                };
            }

            if (sourceViewpoint.ShouldSerializePerspectiveCamera())
            {
                viewpoint.PerspectiveCamera = new BCFv2.Schemas.PerspectiveCamera
                {
                    CameraDirection = GetV2DirectionFromV21(sourceViewpoint.PerspectiveCamera.CameraDirection),
                    CameraUpVector = GetV2DirectionFromV21(sourceViewpoint.PerspectiveCamera.CameraUpVector),
                    CameraViewPoint = GetV2PointFromV21(sourceViewpoint.PerspectiveCamera.CameraViewPoint),
                    FieldOfView = sourceViewpoint.PerspectiveCamera.FieldOfView
                };
            }

            viewpoint.GUID = sourceViewpoint.Guid;

            topic.Viewpoints.Add(viewpoint);
        }

        private static List<BCFv2.Schemas.Component> GetComponentsForViewpoint(BCFv21.Schemas.VisualizationInfo sourceViewpoint)
        {
            var components = new List<BCFv2.Schemas.Component>();

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
                    if (srcColor.ShouldSerializeComponent())
                    {
                        components.AddRange(srcColor.Component.Select(c => new BCFv2.Schemas.Component
                        {
                            AuthoringToolId = c.AuthoringToolId,
                            Color = srcColor.Color.ToByteArrayFromHexRgbColor(),
                            IfcGuid = c.IfcGuid,
                            OriginatingSystem = c.OriginatingSystem,
                            Visible = true,
                            Selected = false
                        }));
                    }
                }
            }

            if (sourceViewpoint.Components.ShouldSerializeSelection())
            {
                var srcSelection = sourceViewpoint.Components.Selection;
                components.AddRange(srcSelection.Select(s => new BCFv2.Schemas.Component
                {
                    AuthoringToolId = s.AuthoringToolId,
                    IfcGuid = s.IfcGuid,
                    OriginatingSystem = s.OriginatingSystem,
                    Selected = true,
                    Visible = true
                }));
            }

            if (sourceViewpoint.Components.ShouldSerializeVisibility())
            {
                var srcVisibility = sourceViewpoint.Components.Visibility;
                if (srcVisibility.DefaultVisibility && srcVisibility.ShouldSerializeExceptions())
                {
                    components.AddRange(srcVisibility.Exceptions.Select(e => new BCFv2.Schemas.Component
                    {
                        AuthoringToolId = e.AuthoringToolId,
                        IfcGuid = e.IfcGuid,
                        OriginatingSystem = e.OriginatingSystem,
                        Visible = true,
                        Selected = false
                    }));
                }
            }

            return components;
        }

        private static BCFv2.Schemas.Point GetV2PointFromV21(BCFv21.Schemas.Point source)
        {
            return new BCFv2.Schemas.Point
            {
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
        }

        private static BCFv2.Schemas.Direction GetV2DirectionFromV21(BCFv21.Schemas.Direction source)
        {
            return new BCFv2.Schemas.Direction
            {
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
        }
    }
}
