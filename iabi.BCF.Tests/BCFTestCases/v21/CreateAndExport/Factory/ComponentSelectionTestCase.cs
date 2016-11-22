using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using iabi.BCF.BCFv21;
using iabi.BCF.BCFv21.Schemas;
using Point = iabi.BCF.BCFv21.Schemas.Point;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class ComponentSelectionTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var Container = new BCFv21Container();
            Container.Topics.Add(CreateTopic());
            Container.FileAttachments.Add("Estructura.ifc", BCFTestCaseData.Estructura);
            return Container;
        }

        public static BCFTopic CreateTopic()
        {
            var ReturnTopic = new BCFTopic();
            ReturnTopic.Markup = CreateMarkup();
            ReturnTopic.Viewpoints.Add(CreateViewpoiont());
            var ImageConverter = new ImageConverter();
            ReturnTopic.AddOrUpdateSnapshot(ReturnTopic.Viewpoints.Last().Guid, (byte[]) ImageConverter.ConvertTo(BCFTestCaseData.ComponentCollection_Snapshot_01, typeof (byte[])));
            return ReturnTopic;
        }

        private static Markup CreateMarkup()
        {
            var Markup = new Markup();
            Markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 16, 12, 13, 15, DateTimeKind.Utc),
                Description = "This topic has three selected components (as shown in the snapshot). All other components should be displayed with their default settings.",
                Guid = BCFTestCaseData.ComponentSelection_TopicGuid,
                Index = 0,
                Title = "Component Selection"
            };
            Markup.Header = new List<HeaderFile>
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
            return Markup;
        }

        private static VisualizationInfo CreateViewpoiont()
        {
            return new VisualizationInfo
            {
                Guid = BCFTestCaseData.ComponentSelection_ViewpointGuid,
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
                Components = new VisualizationInfoComponents
                {
                    DefaultVisibilityComponents = bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilityComponents),
                    DefaultVisibilityOpenings = bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilityOpenings),
                    DefaultVisibilitySpaceBoundaries = bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilitySpaceBoundaries),
                    DefaultVisibilitySpaces = bool.Parse(BCFTestCaseData.ComponentSelection_DefaultVisibilitySpaces),
                    Component = new List<Component> { 
                    new Component
                    {
                        IfcGuid = "1GU8BMEqHBQxVAbwRD$4Jj",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "0AQJSsoeDDvwVqSNcwjy55",
                        Selected = true
                    },
                    new Component
                    {
                        IfcGuid = "3DOu_tSXP6evQgY8Ml4CtC",
                        Selected = true
                    }
                }
}
                //Components = new List<Component>
                //{
                //    new Component
                //    {
                //        IfcGuid = "1GU8BMEqHBQxVAbwRD$4Jj",
                //        Selected = true
                //    },
                //    new Component
                //    {
                //        IfcGuid = "0AQJSsoeDDvwVqSNcwjy55",
                //        Selected = true
                //    },
                //    new Component
                //    {
                //        IfcGuid = "3DOu_tSXP6evQgY8Ml4CtC",
                //        Selected = true
                //    }
                //}
            };
        }
    }
}