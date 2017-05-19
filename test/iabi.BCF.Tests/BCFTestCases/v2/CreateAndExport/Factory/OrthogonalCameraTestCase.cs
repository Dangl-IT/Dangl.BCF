using System;
using System.Collections.Generic;
using System.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Point = iabi.BCF.BCFv2.Schemas.Point;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory
{
    public static class OrthogonalCameraTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var container = new BCFv2Container();
            container.Topics.Add(CreateTopic());
            container.FileAttachments.Add("Estructura.ifc", TestCaseResourceFactory.GetIfcFile(IfcFiles.Estructura));
            return container;
        }

        public static BCFTopic CreateTopic()
        {
            var returnTopic = new BCFTopic();
            returnTopic.Markup = CreateMarkup();
            returnTopic.Viewpoints.Add(CreateViewpoiont());
            // Using the snapshot from the perspective camera since I can't create an orthogonal camera in the viewer=)
            returnTopic.AddOrUpdateSnapshot(returnTopic.Viewpoints.Last().GUID, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.PerspectiveCamera_Snapshot_01));
            return returnTopic;
        }

        private static Markup CreateMarkup()
        {
            var markup = new Markup();
            markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 16, 12, 13, 15, DateTimeKind.Utc),
                Description = "This topic has a single viewpoint with an orthogonal camera. The viewpoint camera was manually created and should provide a straight view from the center of the coordinate system top down to the model.",
                Guid = BcFv2TestCaseData.ORTHOGONAL_CAMERA_TOPIC_GUID,
                Index = "0",
                Title = "Orthogonal Camera"
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
                OrthogonalCamera = new OrthogonalCamera
                {
                    ViewToWorldScale = 1,
                    CameraDirection = new Direction
                    {
                        X = 0,
                        Y = 0,
                        Z = -1
                    },
                    CameraUpVector = new Direction
                    {
                        X = 0,
                        Y = 1,
                        Z = 0
                    },
                    CameraViewPoint = new Point
                    {
                        X = 0,
                        Y = 0,
                        Z = 100
                    }
                }
            };
        }
    }
}