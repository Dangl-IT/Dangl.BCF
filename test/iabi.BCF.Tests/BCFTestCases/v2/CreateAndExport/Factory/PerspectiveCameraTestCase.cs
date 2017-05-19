using System;
using System.Collections.Generic;
using System.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using Point = iabi.BCF.BCFv2.Schemas.Point;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory
{
    public static class PerspectiveCameraTestCase
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
            returnTopic.AddOrUpdateSnapshot(returnTopic.Viewpoints.Last().GUID, TestCaseResourceFactory.GetViewpointSnapshot(ViewpointSnapshots.PerspectiveCamera_Snapshot_01));
            return returnTopic;
        }

        private static Markup CreateMarkup()
        {
            var markup = new Markup();
            markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 16, 12, 13, 14, DateTimeKind.Utc),
                Description = "This topic has a single viewpoint with a perspective camera.",
                Guid = BcFv2TestCaseData.PERSPECTIVE_CAMERA_TOPIC_GUID,
                Index = "0",
                Title = "Perspective Camera"
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
                PerspectiveCamera = new PerspectiveCamera
                {
                    FieldOfView = 60,
                    CameraViewPoint = new Point
                    {
                        X = 19.1479514688529,
                        Y = -22.614447888675,
                        Z = 18.3473354318386
                    },
                    CameraDirection = new Direction
                    {
                        X = -0.548840699373907,
                        Y = 0.560081664635821,
                        Z = -0.620550099642406
                    },
                    CameraUpVector = new Direction
                    {
                        X = -0.0698737081383148,
                        Y = 0.0713048104294571,
                        Z = 0.995004165278026
                    }
                }
            };
        }
    }
}