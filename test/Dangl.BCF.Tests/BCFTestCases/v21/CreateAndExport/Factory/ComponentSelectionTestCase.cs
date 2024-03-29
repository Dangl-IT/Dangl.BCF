using System;
using System.Collections.Generic;
using System.Linq;
using Dangl.BCF.BCFv21;
using Dangl.BCF.BCFv21.Schemas;
using Point = Dangl.BCF.BCFv21.Schemas.Point;

namespace Dangl.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class ComponentSelectionTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var container = new BCFv21Container();
            container.Topics.Add(CreateTopic());
            container.FileAttachments.Add("Estructura.ifc", TestCaseResourceFactory.GetIfcFile(IfcFiles.Estructura));
            return container;
        }

        public static BCFTopic CreateTopic()
        {
            var returnTopic = new BCFTopic();
            returnTopic.Markup = CreateMarkup();
            returnTopic.Viewpoints.Add(CreateViewpoiont());
            returnTopic.AddOrUpdateSnapshot(returnTopic.Viewpoints.Last().Guid, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.ComponentCollection_Snapshot_01));
            return returnTopic;
        }

        private static Markup CreateMarkup()
        {
            var markup = new Markup();
            markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 16, 12, 13, 15, DateTimeKind.Utc),
                Description = "This topic has three selected components (as shown in the snapshot). All other components should be displayed with their default settings.",
                Guid = BcFv21TestCaseData.COMPONENT_SELECTION_TOPIC_GUID,
                Index = 0,
                Title = "Component Selection"
            };
            markup.Header = new List<HeaderFile>
            {
                new HeaderFile
                {
                    Date = new DateTime(2014, 02, 25, 11, 50, 32),
                    Filename = "Estructura.ifc",
                    IfcProject = "3LIQL2UvjC6xkGKOQxhhVW",
                    isExternal = false,
                    Reference = "../Estructura.ifc"
                }
            };
            return markup;
        }

        private static VisualizationInfo CreateViewpoiont()
        {
            return new VisualizationInfo
            {
                Guid = BcFv21TestCaseData.COMPONENT_SELECTION_VIEWPOINT_GUID,
                PerspectiveCamera = new PerspectiveCamera
                {
                    FieldOfView = 60,
                    CameraViewPoint = new Point
                    {
                        X = -16.4296623429377,
                        Y = -0.160221005721745,
                        Z = 4.97078624471391
                    },
                    CameraDirection = new Direction
                    {
                        X = 0.803157911518554,
                        Y = -0.590456679305644,
                        Z = -0.0793614726687355
                    },
                    CameraUpVector = new Direction
                    {
                        X = -0.379182758930675,
                        Y = 0.278763630961051,
                        Z = 0.882332858610135
                    }
                },
                Components = new Components
                {
                    Selection = new List<Component>
                    {
                        new Component {IfcGuid = "1GU8BMEqHBQxVAbwRD$4Jj" },
                        new Component {IfcGuid = "0AQJSsoeDDvwVqSNcwjy55" },
                        new Component {IfcGuid = "3DOu_tSXP6evQgY8Ml4CtC" }
                    },
                    ViewSetupHints = new ViewSetupHints
                    {
                        OpeningsVisible = BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_OPENINGS,
                        SpaceBoundariesVisible = BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_SPACE_BOUNDARIES,
                        SpacesVisible = BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_SPACES
                    },
                    Visibility = new ComponentVisibility
                    {
                        DefaultVisibility = BcFv21TestCaseData.COMPONENT_SELECTION_DEFAULT_VISIBILITY_COMPONENTS
                    }
                }
            };
        }
    }
}