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
            var Container = new BCFv21Container();
            // Set the project
            Container.BCFProject = new ProjectExtension();
            Container.BCFProject.Project = new Project();
            Container.BCFProject.Project.Name = "BCF API Implementation";
            Container.BCFProject.Project.ProjectId = "F338B6F0-A93E-40FF-A4D6-6117CD21EC2A";

            // Set extensions
            Container.ProjectExtensions = CreateExtensions();

            // Set Topics
            Container.Topics.Add(CreateSimpleTopicToActAsReference());
            Container.Topics.Add(CreateTopic());

            Container.FileAttachments.Add("IfcPile_01.ifc", TestCaseResourceFactory.GetIfcFile(IfcFiles.IfcPile));

            Container.FileAttachments.Add("markup.xsd", TestCaseResourceFactory.GetFileAttachment(FileAttachments.MarkupSchemaV21));

            return Container;
        }

        public static BCFExtensions CreateExtensions()
        {
            var ReturnObject = new BCFExtensions();
            ReturnObject.TopicType.Add("Architecture");
            ReturnObject.TopicType.Add("Hidden Type");
            ReturnObject.TopicType.Add("Structural");
            ReturnObject.TopicStatus.Add("Finished status");
            ReturnObject.TopicStatus.Add("Open");
            ReturnObject.TopicStatus.Add("Closed");
            ReturnObject.TopicLabel.Add("Architecture");
            ReturnObject.TopicLabel.Add("IT Development");
            ReturnObject.TopicLabel.Add("Management");
            ReturnObject.TopicLabel.Add("Mechanical");
            ReturnObject.TopicLabel.Add("Structural");
            ReturnObject.SnippetType.Add("IFC2X3");
            ReturnObject.SnippetType.Add("PDF");
            ReturnObject.SnippetType.Add("XLSX");
            ReturnObject.Priority.Add("Low");
            ReturnObject.Priority.Add("High");
            ReturnObject.Priority.Add("Medium");
            ReturnObject.UserIdType.Add("dangl@iabi.eu");
            ReturnObject.UserIdType.Add("linhard@iabi.eu");
            return ReturnObject;
        }

        public static BCFTopic CreateTopic()
        {
            var ReturnTopic = new BCFTopic();

            // Set Markup
            ReturnTopic.Markup = CreateMarkup();

            ReturnTopic.SnippetData = TestCaseResourceFactory.GetFileAttachment(FileAttachments.JsonElement);

            //ReturnTopic.ViewpointBitmaps = new Dictionary<VisualizationInfo,List<byte[]>>;

            foreach (var CurrViewpoint in CreateViewpoints())
            {
                ReturnTopic.Viewpoints.Add(CurrViewpoint);
            }

            ReturnTopic.AddOrUpdateSnapshot(BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_01));
            ReturnTopic.AddOrUpdateSnapshot(BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_02));
            ReturnTopic.AddOrUpdateSnapshot(BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.MaximumInfo_Snapshot_03));

            return ReturnTopic;
        }

        public static IEnumerable<VisualizationInfo> CreateViewpoints()
        {
            // First
            yield return new VisualizationInfo
            {
                //Bitmaps = "",
                ClippingPlanes = GetPlanes().ToList(),
                Components = GetList_01(),
                Guid = BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01,
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
                Guid = BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02,
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
                Guid = BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03,
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
                DefaultVisibilityComponents = BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilityComponents,
                DefaultVisibilityOpenings = BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilityOpenings,
                DefaultVisibilitySpaceBoundaries = BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilitySpaceBoundaries,
                DefaultVisibilitySpaces = BCFv21TestCaseData.MaximimumInformation_Viewpoint_01_DefaultVisibilitySpaces,
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
                DefaultVisibilityComponents = BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilityComponents,
                DefaultVisibilityOpenings = BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilityOpenings,
                DefaultVisibilitySpaceBoundaries = BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilitySpaceBoundaries,
                DefaultVisibilitySpaces = BCFv21TestCaseData.MaximimumInformation_Viewpoint_02_DefaultVisibilitySpaces,
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
                DefaultVisibilityComponents = BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilityComponents,
                DefaultVisibilityOpenings = BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilityOpenings,
                DefaultVisibilitySpaceBoundaries = BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilitySpaceBoundaries,
                DefaultVisibilitySpaces = BCFv21TestCaseData.MaximimumInformation_Viewpoint_03_DefaultVisibilitySpaces,
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
            var Topic = new BCFTopic();

            Topic.Markup = new Markup
            {
                Topic = new Topic
                {
                    Title = "Referenced topic",
                    Guid = BCFv21TestCaseData.MaximumInformation_ReferencedTopicGuid,
                    Description = "This is just an empty topic that acts as a referenced topic."
                }
            };

            return Topic;
        }

        private static Markup CreateMarkup()
        {
            var Markup = new Markup();

            Markup.Comment = CreateComments().ToList();

            Markup.Header = new List<HeaderFile>
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

            Markup.Topic = new Topic
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
                Guid = BCFv21TestCaseData.MaximumInformation_TopicGuid,
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

            Markup.Viewpoints = new List<ViewPoint>
            {
                new ViewPoint
                {
                    Guid = BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01
                },
                new ViewPoint
                {
                    Guid = BCFv21TestCaseData.MaximumInformation_ViewpointGuid_02
                },
                new ViewPoint
                {
                    Guid = BCFv21TestCaseData.MaximumInformation_ViewpointGuid_03
                }
            };

            //Markup.Viewpoints = CreateViewpoints().ToList();

            return Markup;
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
                    Guid = BCFv21TestCaseData.MaximumInformation_ViewpointGuid_01
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