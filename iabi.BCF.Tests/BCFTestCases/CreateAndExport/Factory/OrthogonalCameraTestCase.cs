using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory
{
    public static class OrthogonalCameraTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var Container = new BCFv2Container();
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
            // Using the snapshot from the perspective camera since I can't create an orthogonal camera in the viewer=)
            ReturnTopic.AddOrUpdateSnapshot(ReturnTopic.Viewpoints.Last().GUID, (byte[])ImageConverter.ConvertTo(BCFTestCaseData.PerspectiveCamera_Snapshot_01, typeof(byte[])));
            return ReturnTopic;
        }

        private static Markup CreateMarkup()
        {
            var Markup = new Markup();
            Markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 16, 12, 13, 15, DateTimeKind.Utc),
                Description = "This topic has a single viewpoint with an orthogonal camera. The viewpoint camera was manually created and should provide a straight view from the center of the coordinate system top down to the model.",
                Guid = BCFTestCaseData.OrthogonalCamera_TopicGuid,
                Index = "0",
                Title = "Orthogonal Camera"
            };
            Markup.Header = new List<HeaderFile>{
                new HeaderFile{
                    Date = new DateTime(2014,02,25,11,50,32),
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
                    CameraViewPoint = new BCF.BCFv2.Schemas.Point
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
