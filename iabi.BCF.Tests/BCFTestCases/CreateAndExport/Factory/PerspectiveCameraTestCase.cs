using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.CreateAndExport.Factory
{
    public static class PerspectiveCameraTestCase
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
            ReturnTopic.AddOrUpdateSnapshot(ReturnTopic.Viewpoints.Last().GUID, (byte[])ImageConverter.ConvertTo(BCFTestCaseData.PerspectiveCamera_Snapshot_01, typeof(byte[])));
            return ReturnTopic;
        }

        private static Markup CreateMarkup()
        {
            var Markup = new Markup();
            Markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 16, 12, 13, 14, DateTimeKind.Utc),
                Description = "This topic has a single viewpoint with a perspective camera.",
                Guid = BCFTestCaseData.PerspectiveCamera_TopicGuid,
                Index = "0",
                Title = "Perspective Camera"                
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
                PerspectiveCamera = new PerspectiveCamera
                {
                    FieldOfView = 60,
                    CameraViewPoint = new BCF.BCFv2.Schemas.Point
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
