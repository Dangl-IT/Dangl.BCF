using System;
using System.Collections.Generic;
using System.Linq;
using iabi.BCF.BCFv21;
using iabi.BCF.BCFv21.Schemas;
using Point = iabi.BCF.BCFv21.Schemas.Point;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class MaximumInformationTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var container = new BCFv21Container();
            // Set the project
            container.BcfProject = new ProjectExtension();
            container.BcfProject.Project = new Project();
            container.BcfProject.Project.Name = "BCF API Implementation";
            container.BcfProject.Project.ProjectId = "F338B6F0-A93E-40FF-A4D6-6117CD21EC2A";

            // Set extensions
            container.ProjectExtensions = CreateExtensions();

            // Set Topics
            container.Topics.Add(CreateSimpleTopicToActAsReference());
            container.Topics.Add(CreateTopic());

            container.FileAttachments.Add("IfcPile_01.ifc", TestCaseResourceFactory.GetIfcFile(IfcFiles.IfcPile));

            container.FileAttachments.Add("markup.xsd", TestCaseResourceFactory.GetFileAttachment(FileAttachments.MarkupSchemaV21));

            return container;
        }

        public static ProjectExtensions CreateExtensions()
        {
            var returnObject = new ProjectExtensions();
            returnObject.TopicType.Add("Architecture");
            returnObject.TopicType.Add("Hidden Type");
            returnObject.TopicType.Add("Structural");
            returnObject.TopicStatus.Add("Finished status");
            returnObject.TopicStatus.Add("Open");
            returnObject.TopicStatus.Add("Closed");
            returnObject.TopicLabel.Add("Architecture");
            returnObject.TopicLabel.Add("IT Development");
            returnObject.TopicLabel.Add("Management");
            returnObject.TopicLabel.Add("Mechanical");
            returnObject.TopicLabel.Add("Structural");
            returnObject.SnippetType.Add("IFC2X3");
            returnObject.SnippetType.Add("PDF");
            returnObject.SnippetType.Add("XLSX");
            returnObject.Priority.Add("Low");
            returnObject.Priority.Add("High");
            returnObject.Priority.Add("Medium");
            returnObject.UserIdType.Add("dangl@iabi.eu");
            returnObject.UserIdType.Add("linhard@iabi.eu");
            return returnObject;
        }

        public static BCFTopic CreateTopic()
        {
            var returnTopic = new BCFTopic();

            // Set Markup
            returnTopic.Markup = CreateMarkup();

            returnTopic.SnippetData = TestCaseResourceFactory.GetFileAttachment(FileAttachments.JsonElement);

            //ReturnTopic.ViewpointBitmaps = new Dictionary<VisualizationInfo,List<byte[]>>;

            foreach (var currViewpoint in CreateViewpoints())
            {
                returnTopic.Viewpoints.Add(currViewpoint);
            }

            returnTopic.AddOrUpdateSnapshot(BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_01));
            returnTopic.AddOrUpdateSnapshot(BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_02));
            returnTopic.AddOrUpdateSnapshot(BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_03));

            return returnTopic;
        }

        public static IEnumerable<VisualizationInfo> CreateViewpoints()
        {
            // First
            yield return new VisualizationInfo
            {
                //Bitmaps = "",
                ClippingPlanes = GetPlanes().ToList(),
                Components = GetList_01(),
                Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01,
                Lines = GetLines().ToList(),
                //OrthogonalCamera = "",
                PerspectiveCamera = GetCamera_01()
            };
            // Second
            yield return new VisualizationInfo
            {
                //Bitmaps = "",
                //ClippingPlanes = "",
                Components = GetList_02(),
                Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02,
                //Lines = "",
                //OrthogonalCamera = "",
                PerspectiveCamera = GetCamera_02()
            };
            // Third
            yield return new VisualizationInfo
            {
                //Bitmaps = "",
                //ClippingPlanes = "",
                Components = GetList_03(),
                Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03,
                //Lines = "",
                //OrthogonalCamera = "",
                PerspectiveCamera = GetCamera_03()
            };
        }

        public static IEnumerable<ClippingPlane> GetPlanes()
        {
            yield return new ClippingPlane
            {
                Location = new Point
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                },
                Direction = new Direction
                {
                    X = 0,
                    Y = 0,
                    Z = 1
                }
            };
            yield return new ClippingPlane
            {
                Location = new Point
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                },
                Direction = new Direction
                {
                    X = 0,
                    Y = 1,
                    Z = 0
                }
            };
        }

        public static IEnumerable<Line> GetLines()
        {
            yield return new Line
            {
                StartPoint = new Point
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                },
                EndPoint = new Point
                {
                    X = 0,
                    Y = 0,
                    Z = 1
                }
            };
            yield return new Line
            {
                StartPoint = new Point
                {
                    X = 0,
                    Y = 0,
                    Z = 1
                },
                EndPoint = new Point
                {
                    X = 0,
                    Y = 1,
                    Z = 1
                }
            };
            yield return new Line
            {
                StartPoint = new Point
                {
                    X = 0,
                    Y = 1,
                    Z = 1
                },
                EndPoint = new Point
                {
                    X = 1,
                    Y = 1,
                    Z = 1
                }
            };
        }

        public static VisualizationInfoComponents GetList_01()
        {
            return new VisualizationInfoComponents
            {
                DefaultVisibilityComponents = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_01_DEFAULT_VISIBILITY_COMPONENTS,
                DefaultVisibilityOpenings = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_01_DEFAULT_VISIBILITY_OPENINGS,
                DefaultVisibilitySpaceBoundaries = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_01_DEFAULT_VISIBILITY_SPACE_BOUNDARIES,
                DefaultVisibilitySpaces = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_01_DEFAULT_VISIBILITY_SPACES,
                Component = new List<Component>
                {
                    new Component
                    {
                        IfcGuid = "0Gl71cVurFn8bxAOox6M4X",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "23Zwlpd71EyvHlH6OZ77nK",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "3DvyPxGIn8qR0KDwbL_9r1",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "0fdpeZZEX3FwJ7x0ox5kzF",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "1OpjQ1Nlv4sQuTxfUC_8zS",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "0cSRUx$EX1NRjqiKcYQ$a0",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "1jQQiGIAnFzxOUzrdmJYDS",
                        Selected = true
                    }
                }
            };
        }

        public static VisualizationInfoComponents GetList_02()
        {
            return new VisualizationInfoComponents
            {
                DefaultVisibilityComponents = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_02_DEFAULT_VISIBILITY_COMPONENTS,
                DefaultVisibilityOpenings = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_02_DEFAULT_VISIBILITY_OPENINGS,
                DefaultVisibilitySpaceBoundaries = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_02_DEFAULT_VISIBILITY_SPACE_BOUNDARIES,
                DefaultVisibilitySpaces = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_02_DEFAULT_VISIBILITY_SPACES,
                Component = new List<Component>
                {
                    new Component
                    {
                        IfcGuid = "0fdpeZZEX3FwJ7x0ox5kzF",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "23Zwlpd71EyvHlH6OZ77nK",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "1OpjQ1Nlv4sQuTxfUC_8zS",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "0cSRUx$EX1NRjqiKcYQ$a0",
                        Selected = true
                    }
                }
            };
        }

        public static VisualizationInfoComponents GetList_03()
        {
            return new VisualizationInfoComponents
            {
                DefaultVisibilityComponents = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_03_DEFAULT_VISIBILITY_COMPONENTS,
                DefaultVisibilityOpenings = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_03_DEFAULT_VISIBILITY_OPENINGS,
                DefaultVisibilitySpaceBoundaries = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_03_DEFAULT_VISIBILITY_SPACE_BOUNDARIES,
                DefaultVisibilitySpaces = BcFv21TestCaseData.MAXIMIMUM_INFORMATION_VIEWPOINT_03_DEFAULT_VISIBILITY_SPACES,
                Component = new List<Component>
                {
                    new Component
                    {
                        IfcGuid = "0fdpeZZEX3FwJ7x0ox5kzF",
                        Visible = false
                    },
                    new Component
                    {
                        IfcGuid = "23Zwlpd71EyvHlH6OZ77nK",
                        Visible = false
                    },
                    new Component
                    {
                        IfcGuid = "1OpjQ1Nlv4sQuTxfUC_8zS",
                        Visible = false
                    },
                    new Component
                    {
                        IfcGuid = "0cSRUx$EX1NRjqiKcYQ$a0",
                        Visible = false
                    }
                }
            };
        }

        public static PerspectiveCamera GetCamera_01()
        {
            return new PerspectiveCamera
            {
                CameraDirection = new Direction
                {
                    X = -0.381615611200324,
                    Y = -0.825232810204882,
                    Z = -0.416365617893758
                },
                CameraUpVector = new Direction
                {
                    X = 0.05857014928797,
                    Y = 0.126656300502579,
                    Z = 0.990215996212637
                },
                CameraViewPoint = new Point
                {
                    X = 12.2088897788292,
                    Y = 52.323145074034,
                    Z = 5.24072091171001
                },
                FieldOfView = 60
            };
        }

        public static PerspectiveCamera GetCamera_02()
        {
            return new PerspectiveCamera
            {
                CameraDirection = new Direction
                {
                    X = 0.838092829258085,
                    Y = 0.432129626000854,
                    Z = -0.332963080206749
                },
                CameraUpVector = new Direction
                {
                    X = -0.202628324336999,
                    Y = -0.104477334449235,
                    Z = 0.973666395005375
                },
                CameraViewPoint = new Point
                {
                    X = -37.3710937321924,
                    Y = -41.0844513828444,
                    Z = 0.875129294957555
                },
                FieldOfView = 60
            };
        }

        public static PerspectiveCamera GetCamera_03()
        {
            return new PerspectiveCamera
            {
                CameraDirection = new Direction
                {
                    X = 0.520915589917324,
                    Y = -0.661777065303802,
                    Z = -0.539164240672219
                },
                CameraUpVector = new Direction
                {
                    // Pretty much straight up=)
                    X = -1.13083372396512E-08,
                    Y = -8.90132135110878E-09,
                    Z = 1
                },
                CameraViewPoint = new Point
                {
                    X = -30.0807178226062,
                    Y = 17.1180195726065,
                    Z = 11.4769701040657
                },
                FieldOfView = 60
            };
        }

        public static BCFTopic CreateSimpleTopicToActAsReference()
        {
            var topic = new BCFTopic();

            topic.Markup = new Markup
            {
                Topic = new Topic
                {
                    Title = "Referenced topic",
                    Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_REFERENCED_TOPIC_GUID,
                    Description = "This is just an empty topic that acts as a referenced topic."
                }
            };

            return topic;
        }

        private static Markup CreateMarkup()
        {
            var markup = new Markup();

            markup.Comment = CreateComments().ToList();

            markup.Header = new List<HeaderFile>
            {
                new HeaderFile
                {
                    Date = new DateTime(2014, 10, 27, 16, 27, 27, DateTimeKind.Utc),
                    Filename = "IfcPile_01.ifc",
                    IfcProject = "0M6o7Znnv7hxsbWgeu7oQq",
                    IfcSpatialStructureElement = "23B$bNeGHFQuMYJzvUX0FD",
                    isExternal = false,
                    Reference = "../IfcPile_01.ifc"
                }
            };

            markup.Topic = new Topic
            {
                AssignedTo = "linhard@iabi.eu",
                BimSnippet =  new BimSnippet
                {
                    isExternal = false,
                    Reference = "JsonElement.json",
                    ReferenceSchema = "http://json-schema.org",
                    SnippetType = "JSON"
                },
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 06, 21, 12, 00, 00, DateTimeKind.Utc),
                Description = "This is a topic with all informations present.",
                DocumentReference = new List<TopicDocumentReference>
                {
                    new TopicDocumentReference
                    {
                        Description = "GitHub BCF Specification",
                        isExternal = true,
                        ReferencedDocument = "https://github.com/BuildingSMART/BCF-XML"
                    },
                    new TopicDocumentReference
                    {
                        Description = "Markup.xsd Schema",
                        isExternal = false,
                        ReferencedDocument = "../markup.xsd"
                    }
                },
                Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_TOPIC_GUID,
                Index = 0,
                Labels = new List<string> {"Structural", "IT Development"},
                ModifiedAuthor = "dangl@iabi.eu",
                ModifiedDate = new DateTime(2015, 06, 21, 14, 22, 47, DateTimeKind.Utc),
                Priority = "High",
                ReferenceLink = new [] {"https://bim--it.net"}.ToList(),
                RelatedTopic = new List<TopicRelatedTopic>
                {
                    new TopicRelatedTopic
                    {
                        Guid = "5019D939-62A4-45D9-B205-FAB602C98FE8"
                    }
                },
                Title = "Maximum Content",
                TopicStatus = "Open",
                TopicType = "Structural"
            };

            markup.Viewpoints = new List<ViewPoint>
            {
                new ViewPoint
                {
                    Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01
                },
                new ViewPoint
                {
                    Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_02
                },
                new ViewPoint
                {
                    Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_03
                }
            };

            //Markup.Viewpoints = CreateViewpoints().ToList();

            return markup;
        }

        public static IEnumerable<Comment> CreateComments()
        {
            // First
            yield return new Comment
            {
                Author = "dangl@iabi.eu",
                Comment1 = "This is an unmodified topic at the uppermost hierarchical level." + "\n" + "All times in the XML are marked as UTC times.",
                Date = new DateTime(2015, 08, 31, 12, 40, 17, DateTimeKind.Utc),
                Guid = "780FAE52-C432-42BE-ADEA-FF3E7A8CD8E1"
            };

            // Second
            yield return new Comment
            {
                Author = "dangl@iabi.eu",
                Comment1 = "This comment was a reply to the first comment in BCF v2.0. This is a no longer supported functionality and therefore is to be treated as a regular comment in v2.1.",
                Date = new DateTime(2015, 08, 31, 14, 00, 01, DateTimeKind.Utc),
                Guid = "897E4909-BDF3-4CC7-A283-6506CAFF93DD"
            };

            // Third
            yield return new Comment
            {
                Author = "dangl@iabi.eu",
                Comment1 = "This comment again is in the highest hierarchy level." + "\n" + "It references a viewpoint.",
                Date = new DateTime(2015, 08, 31, 13, 07, 11, DateTimeKind.Utc),
                Guid = "39C4B780-1B48-44E5-9802-D359007AA44E",
                Viewpoint = new CommentViewpoint
                {
                    Guid = BcFv21TestCaseData.MAXIMUM_INFORMATION_VIEWPOINT_GUID_01
                }
            };

            // Fourth
            yield return new Comment
            {
                Author = "dangl@iabi.eu",
                Comment1 = "This comment contained some spllng errs." + "\n" + "Hopefully, the modifier did catch them all.",
                Date = new DateTime(2015, 08, 31, 15, 42, 58, DateTimeKind.Utc),
                Guid = "BD17158C-4267-4433-98C1-904F9B41CA50",
                ModifiedAuthor = "dangl@iabi.eu",
                ModifiedDate = new DateTime(2015, 08, 31, 16, 07, 11, DateTimeKind.Utc)
            };
        }
    }
}