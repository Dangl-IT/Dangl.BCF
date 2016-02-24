using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using iabi.BCF.Tests.BCFTestCases.CreateAndExport.Factory;
using Xunit;

namespace iabi.BCF.Tests.BCFv2
{
     
    public class BCFv2Container_Test
    {
         
        public class GetFilenameFromReference
        {
            [Fact]
            public void EmptyOnNullInput()
            {
                Assert.True(string.IsNullOrWhiteSpace(BCFv2Container.GetFilenameFromReference(null)));
            }

            [Fact]
            public void EmptyOnEmptyInput()
            {
                Assert.True(string.IsNullOrWhiteSpace(BCFv2Container.GetFilenameFromReference(string.Empty)));
            }

            [Fact]
            public void Check_01()
            {
                var Input = "some.file";
                var Expected = "some.file";
                var Actual = BCFv2Container.GetFilenameFromReference(Input);
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Check_02()
            {
                var Input = "/some.file";
                var Expected = "some.file";
                var Actual = BCFv2Container.GetFilenameFromReference(Input);
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Check_03()
            {
                var Input = "../some.file";
                var Expected = "some.file";
                var Actual = BCFv2Container.GetFilenameFromReference(Input);
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Check_04()
            {
                var Input = "123/some.file";
                var Expected = "some.file";
                var Actual = BCFv2Container.GetFilenameFromReference(Input);
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Check_05()
            {
                var Input = "../123hallo/some.file";
                var Expected = "some.file";
                var Actual = BCFv2Container.GetFilenameFromReference(Input);
                Assert.Equal(Expected, Actual);
            }
        }

         
        public class GetAttachmentForDocumentReference
        {
            [Fact]
            public void ExceptionOnExternalReference()
            {
                var Container = new BCFv2Container();
                var DocRef = new TopicDocumentReferences();
                DocRef.isExternal = true;
                Assert.Throws(typeof (ArgumentException), () =>
                {
                    var Data = Container.GetAttachmentForDocumentReference(DocRef);
                });
            }

            [Fact]
            public void Check_01()
            {
                var Container = BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.MaximumInformation);

                var References = Container.Topics.SelectMany(Curr => Curr.Markup.Topic.DocumentReferences);

                foreach (var CurrentDocument in References.Where(Curr => !Curr.isExternal))
                {
                    var RetrievedFile = Container.GetAttachmentForDocumentReference(CurrentDocument);
                    Assert.NotNull(RetrievedFile);
                    Assert.True(RetrievedFile.SequenceEqual(Container.FileAttachments[BCFv2Container.GetFilenameFromReference(CurrentDocument.ReferencedDocument)]));
                }
            }
        }

         
        public class TransformToRelativePath
        {
            [Fact]
            public void EmptyOnEmptyInput_01()
            {
                var Created = BCFv2Container.TransformToRelativePath(null, null);
                Assert.True(string.IsNullOrWhiteSpace(Created));
            }

            [Fact]
            public void EmptyOnEmptyInput_02()
            {
                var Created = BCFv2Container.TransformToRelativePath(string.Empty, null);
                Assert.True(string.IsNullOrWhiteSpace(Created));
            }

            [Fact]
            public void EmptyOnEmptyInput_03()
            {
                var Created = BCFv2Container.TransformToRelativePath(null, string.Empty);
                Assert.True(string.IsNullOrWhiteSpace(Created));
            }

            [Fact]
            public void EmptyOnEmptyInput_04()
            {
                var Created = BCFv2Container.TransformToRelativePath(string.Empty, string.Empty);
                Assert.True(string.IsNullOrWhiteSpace(Created));
            }

            [Fact]
            public void Create_01()
            {
                var InputAbsolute = "none.jpg";
                var InputCurrentLoc = string.Empty;
                var Expected = "none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_01a()
            {
                var InputAbsolute = "someFolder/none.jpg";
                var InputCurrentLoc = string.Empty;
                var Expected = "someFolder/none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_02()
            {
                var InputAbsolute = "none.jpg";
                var InputCurrentLoc = "someFolder";
                var Expected = "../none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_03()
            {
                var InputAbsolute = "none.jpg";
                var InputCurrentLoc = "someFolder/nested";
                var Expected = "../../none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_04()
            {
                var InputAbsolute = "different/none.jpg";
                var InputCurrentLoc = "someFolder/nested";
                var Expected = "../../different/none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_05()
            {
                var InputAbsolute = "someFolder/none.jpg";
                var InputCurrentLoc = "someFolder";
                var Expected = "none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_06()
            {
                var InputAbsolute = "someFolder/nested/none.jpg";
                var InputCurrentLoc = "someFolder/nested";
                var Expected = "none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_07()
            {
                var InputAbsolute = "totally/different/folder/none.jpg";
                var InputCurrentLoc = "some/other/structure";
                var Expected = "../../../totally/different/folder/none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_08()
            {
                var InputAbsolute = "first/topic/none.jpg";
                var InputCurrentLoc = "second/topic";
                var Expected = "../../first/topic/none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_09()
            {
                var InputAbsolute = "first/topic/none.jpg";
                var InputCurrentLoc = string.Empty;
                var Expected = "first/topic/none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_10()
            {
                var InputAbsolute = "first/topic/none.jpg";
                string InputCurrentLoc = null;
                var Expected = "first/topic/none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }

            [Fact]
            public void Create_11()
            {
                var InputAbsolute = "first/topic/none.jpg";
                string InputCurrentLoc = "some/other/folder/dir/hello";
                var Expected = "../../../../../first/topic/none.jpg";
                var Created = BCFv2Container.TransformToRelativePath(InputAbsolute, InputCurrentLoc);
                Assert.Equal(Expected, Created);
            }
        }

         
        public class GetAbsolutePath
        {
            [Fact]
            public void EmptyOnEmptyInput_01()
            {
                var Created = BCFv2Container.GetAbsolutePath(null, null);
                Assert.True(string.IsNullOrWhiteSpace(Created));
            }

            [Fact]
            public void EmptyOnEmptyInput_02()
            {
                var Created = BCFv2Container.GetAbsolutePath(null, string.Empty);
                Assert.True(string.IsNullOrWhiteSpace(Created));
            }

            [Fact]
            public void EmptyOnEmptyInput_03()
            {
                var Created = BCFv2Container.GetAbsolutePath(string.Empty, null);
                Assert.True(string.IsNullOrWhiteSpace(Created));
            }

            [Fact]
            public void OnlyBasePath_01()
            {
                var Created = BCFv2Container.GetAbsolutePath("SomePath", null);
                Assert.Equal("SomePath", Created);
            }

            [Fact]
            public void OnlyBasePath_02()
            {
                var Created = BCFv2Container.GetAbsolutePath("SomePath/SomeOther", null);
                Assert.Equal("SomePath/SomeOther", Created);
            }

            [Fact]
            public void Combine_01()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456", "example.jpg");
                Assert.Equal("123456/example.jpg", Created);
            }

            [Fact]
            public void Combine_02()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456", "../example.jpg");
                Assert.Equal("example.jpg", Created);
            }

            [Fact]
            public void Combine_03()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456/789", "example.jpg");
                Assert.Equal("123456/789/example.jpg", Created);
            }

            [Fact]
            public void Combine_04()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456/789", "../example.jpg");
                Assert.Equal("123456/example.jpg", Created);
            }

            [Fact]
            public void Combine_05()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456/789", "../../example.jpg");
                Assert.Equal("example.jpg", Created);
            }

            [Fact]
            public void Combine_05a()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456/789/abc/def", "../../../../example.jpg");
                Assert.Equal("example.jpg", Created);
            }

            [Fact]
            public void Combine_05b()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456/789/abc/def", "../../../example.jpg");
                Assert.Equal("123456/example.jpg", Created);
            }

            [Fact]
            public void Combine_06()
            {
                Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
                {
                    var Created = BCFv2Container.GetAbsolutePath("123456", "../../example.jpg");
                });
            }

            [Fact]
            public void Combine_07()
            {
                var Created = BCFv2Container.GetAbsolutePath("123456", "789/example.jpg");
                Assert.Equal("123456/789/example.jpg", Created);
            }
        }

         
        public class WriteStream
        {
            public BCFTopic MockBCFv2_WithValues()
            {
                BCFTopic ReturnObject = new BCFTopic();

                // Topic definition
                ReturnObject.Markup = MockTopicMarkup();

                // Append one comment
                ReturnObject.Markup.Comment = new System.Collections.Generic.List<Comment>();
                ReturnObject.Markup.Comment.Add(new Comment());
                ReturnObject.Markup.Comment[0].Author = "dangl@iabi.eu";
                ReturnObject.Markup.Comment[0].Comment1 = "I like the idea of collaboration!";
                ReturnObject.Markup.Comment[0].Date = new DateTime(2015, 05, 20, 16, 59, 30);
                ReturnObject.Markup.Comment[0].Guid = "9FFDF348-11B7-48D7-A586-E3EE56BAA351";
                ReturnObject.Markup.Comment[0].Status = "Info";
                ReturnObject.Markup.Comment[0].VerbalStatus = "Info";

                // Get some viewpoints
                foreach (var CurrentViewpoint in MockViewpoints())
                {
                    ReturnObject.Viewpoints.Add(CurrentViewpoint);
                }
                return ReturnObject;
            }

            public Markup MockTopicMarkup()
            {
                var ReturnObject = new Markup();
                ReturnObject.Topic.AssignedTo = "linhard@iabi.eu";
                ReturnObject.Topic.CreationAuthor = "dangl@iabi.eu";
                ReturnObject.Topic.CreationDate = new DateTime(2015, 05, 20, 16, 11, 34);
                ReturnObject.Topic.Description = "This is a topic that is just intended to test the functionality."
                                                   + Environment.NewLine + "It even features linebreaks!";
                ReturnObject.Topic.DocumentReferences = new System.Collections.Generic.List<TopicDocumentReferences>();
                ReturnObject.Topic.DocumentReferences.Add(new TopicDocumentReferences());
                ReturnObject.Topic.DocumentReferences[0].Description = "This is what a referenced document's description would look like.";
                ReturnObject.Topic.DocumentReferences[0].Guid = "E181000F-959B-46E4-A37A-7C57704DAA2D";
                ReturnObject.Topic.DocumentReferences[0].isExternal = true;
                ReturnObject.Topic.DocumentReferences[0].ReferencedDocument = "https://bim--it-net/Expect404.pdf";
                ReturnObject.Topic.Guid = "69434D23-0E03-4961-87C3-2DF7DEBCD316";

                ReturnObject.Topic.Labels = new System.Collections.Generic.List<string>();
                ReturnObject.Topic.Labels.Add("Architecture");
                ReturnObject.Topic.Labels.Add("Unit Testing");
                ReturnObject.Topic.Labels.Add("Development");
                ReturnObject.Topic.Labels.Add("BIM Collaborators");

                ReturnObject.Topic.Priority = "High";

                ReturnObject.Topic.ReferenceLink = "http://www.iabi.eu";

                // ReturnObject.Viewpoints // Should be set up automagically!

                return ReturnObject;
            }

            public Extensions_XSD MockExtensions()
            {
                var ReturnObject = new Extensions_XSD();
                ReturnObject.Priority.Add("Low");
                ReturnObject.Priority.Add("Medium");
                ReturnObject.Priority.Add("High");
                ReturnObject.SnippetType.Add("IFC2x3");
                ReturnObject.SnippetType.Add("IFC4");
                ReturnObject.TopicLabel.Add("Architecture");
                ReturnObject.TopicLabel.Add("BIM Collaborators");
                ReturnObject.TopicLabel.Add("Development");
                ReturnObject.TopicLabel.Add("Unit Testing");
                ReturnObject.TopicStatus.Add("Open");
                ReturnObject.TopicStatus.Add("Closed");
                ReturnObject.TopicStatus.Add("Reopened");
                ReturnObject.TopicType.Add("Info");
                ReturnObject.TopicType.Add("Warning");
                ReturnObject.TopicType.Add("Error");
                ReturnObject.UserIdType.Add("dangl@iabi.eu");
                ReturnObject.UserIdType.Add("linhard@iabi.eu");
                return ReturnObject;
            }

            public System.Collections.Generic.List<VisualizationInfo> MockViewpoints()
            {
                var ReturnObject = new System.Collections.Generic.List<VisualizationInfo>();

                // Make a viewpoint with an orthogonal view
                ReturnObject.Add(new VisualizationInfo());
                ReturnObject[0].OrthogonalCamera = new OrthogonalCamera();
                ReturnObject[0].OrthogonalCamera.CameraDirection = new Direction();
                ReturnObject[0].OrthogonalCamera.CameraDirection.X = 12.34;
                ReturnObject[0].OrthogonalCamera.CameraDirection.Y = double.MaxValue;
                ReturnObject[0].OrthogonalCamera.CameraDirection.Z = 0;
                ReturnObject[0].OrthogonalCamera.CameraUpVector = new Direction();
                ReturnObject[0].OrthogonalCamera.CameraUpVector.X = 0;
                ReturnObject[0].OrthogonalCamera.CameraUpVector.Y = 0;
                ReturnObject[0].OrthogonalCamera.CameraUpVector.Z = 1;
                ReturnObject[0].OrthogonalCamera.CameraViewPoint = new iabi.BCF.BCFv2.Schemas.Point();
                ReturnObject[0].OrthogonalCamera.CameraViewPoint.X = 42;
                ReturnObject[0].OrthogonalCamera.CameraViewPoint.Y = 1337;
                ReturnObject[0].OrthogonalCamera.CameraViewPoint.Z = 80085;
                ReturnObject[0].OrthogonalCamera.ViewToWorldScale = 1.123452266578545266425;

                // Make a viewpoint with a perspective view
                ReturnObject.Add(new VisualizationInfo());
                ReturnObject[1].PerspectiveCamera = new iabi.BCF.BCFv2.Schemas.PerspectiveCamera();
                ReturnObject[1].PerspectiveCamera.CameraDirection = new Direction();
                ReturnObject[1].PerspectiveCamera.CameraDirection.X = 12.34;
                ReturnObject[1].PerspectiveCamera.CameraDirection.Y = double.MaxValue;
                ReturnObject[1].PerspectiveCamera.CameraDirection.Z = 0;
                ReturnObject[1].PerspectiveCamera.CameraUpVector = new Direction();
                ReturnObject[1].PerspectiveCamera.CameraUpVector.X = 0;
                ReturnObject[1].PerspectiveCamera.CameraUpVector.Y = 0;
                ReturnObject[1].PerspectiveCamera.CameraUpVector.Z = 1;
                ReturnObject[1].PerspectiveCamera.CameraViewPoint = new iabi.BCF.BCFv2.Schemas.Point();
                ReturnObject[1].PerspectiveCamera.CameraViewPoint.X = 42;
                ReturnObject[1].PerspectiveCamera.CameraViewPoint.Y = 1337;
                ReturnObject[1].PerspectiveCamera.CameraViewPoint.Z = 80085;
                ReturnObject[1].PerspectiveCamera.FieldOfView = 1.123452266578545266425;

                // Some components
                ReturnObject[1].Components.Add(new Component());
                ReturnObject[1].Components[0].AuthoringToolId = "14055C39-C439-4ECF-A162-D89A66F352C6";
                ReturnObject[1].Components[0].IfcGuid = "dsfhdsiufg";
                ReturnObject[1].Components[0].Visible = true;
                ReturnObject[1].Components[0].OriginatingSystem = "UnitTestTool";
                ReturnObject[1].Components.Add(new Component());
                ReturnObject[1].Components[1].AuthoringToolId = "E6ABD615-A632-4E27-9D77-AF5ECAA29B86";
                ReturnObject[1].Components[1].IfcGuid = "89fh475fdfhif";
                ReturnObject[1].Components[1].Visible = false;
                ReturnObject[1].Components[1].OriginatingSystem = "UnitTestTool";

                // TODO NOT WORKING
                // Make a viewpoint with a snapshot
                ReturnObject.Add(new VisualizationInfo());
                //ReturnObject[2].Bitmaps = new System.Collections.Generic.List<iabi.BCF_REST_API.BCFv2.Schemas.VisualizationInfoBitmaps>();
                //ReturnObject[2].Bitmaps.Add(new iabi.BCF_REST_API.BCFv2.Schemas.VisualizationInfoBitmaps());
                //ReturnObject[2].Bitmaps[0].Bitmap = iabi.BCF_REST_API.BCFv2.Schemas.BitmapFormat.PNG;
                //ReturnObject[2].Bitmaps[0].Height = 1.0; // Noone knows what this actually means.. Heigth in Pixels? Ratio-Scale? Let's better define it!
                //// Will not set a reference since the snapshot will be included in the file

                //// Make one with another snapshot
                ReturnObject.Add(new VisualizationInfo());
                //ReturnObject[3].Bitmaps = new System.Collections.Generic.List<iabi.BCF_REST_API.BCFv2.Schemas.VisualizationInfoBitmaps>();
                //ReturnObject[3].Bitmaps.Add(new iabi.BCF_REST_API.BCFv2.Schemas.VisualizationInfoBitmaps());
                //ReturnObject[3].Bitmaps[0].Bitmap = iabi.BCF_REST_API.BCFv2.Schemas.BitmapFormat.PNG;
                //ReturnObject[3].Bitmaps[0].Height = 1.0; // Noone knows what this actually means.. Heigth in Pixels? Ratio-Scale? Let's better define it!
                // Will not set a reference since the snapshot will be included in the file

                return ReturnObject;
            }

            [Fact]
            public void Simple_WriteSingleTopic_Extensions()
            {
                BCFv2Container TestContainerInstance = new BCFv2Container();

                TestContainerInstance.ProjectExtensions = MockExtensions();

                TestContainerInstance.Topics.Add(new BCFTopic());
                TestContainerInstance.Topics[0].Markup = new Markup();
                TestContainerInstance.Topics[0].Markup.Topic = new Topic();
                TestContainerInstance.Topics[0].Markup.Topic.Title = "Sample with extensions.";
                TestContainerInstance.Topics[0].Markup.Topic.Guid = Guid.NewGuid().ToString();

                using (var MemStream = new MemoryStream())
                {
                    TestContainerInstance.WriteStream(MemStream);
                    Assert.True(MemStream.Length > 0);

                    MemStream.Position = 0;
                    var Archive = new ZipArchive(MemStream);
                    var ExtensionsEntry = Archive.Entries.FirstOrDefault(Curr => Curr.FullName == "extensions.xsd");
                    Assert.NotNull(ExtensionsEntry);
                }
            }

            [Fact]
            public void Simple_WriteSingleTopic()
            {
                BCFv2Container TestContainerInstance = new BCFv2Container();

                TestContainerInstance.ProjectExtensions = MockExtensions();

                TestContainerInstance.Topics.Add(MockBCFv2_WithValues());

                using (var MemStream = new MemoryStream())
                {
                    TestContainerInstance.WriteStream(MemStream);
                    Assert.True(MemStream.Length > 0);
                }
            }

            [Fact]
            public void AppendSnapshotInfoToMarkup()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                Instance.Topics.First().AddOrUpdateSnapshot(Instance.Topics.First().Viewpoints.First().GUID, new byte[] { 15, 15, 15, 15, 15, 15 });

                using (var MemStream = new MemoryStream())
                {
                    Instance.WriteStream(MemStream);
                    MemStream.Position = 0;
                    var ReadAgain = BCFv2Container.ReadStream(MemStream);
                    Assert.False(string.IsNullOrWhiteSpace(ReadAgain.Topics.First().Markup.Viewpoints.First().Viewpoint));
                    Assert.NotNull(ReadAgain.Topics.First().ViewpointSnapshots.FirstOrDefault());
                }
            }
        }

         
        public class ProjectAndExtensions
        {
            [Fact]
            public void SetPathToExtensionsWhenExtensionsSet()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.ProjectExtensions = new Extensions_XSD();
                Assert.Equal("extensions.xsd", Instance.BCFProject.ExtensionSchema);
            }
        }

        /// <summary>
        /// Not expecting an exception and the markup should then be automatically created (with the viewpoint data)
        /// </summary>
        [Fact]
        public void AddViewpointWhenThereIsNoMarkupInstance()
        {
            BCFv2Container Instance = new BCFv2Container();
            Instance.Topics.Add(new BCFTopic());

            // Markup is empty
            Assert.Null(Instance.Topics.First().Markup);

            Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());

            // Viewpoint defined
            Assert.NotNull(Instance.Topics.First().Markup);
            Assert.Equal(Instance.Topics.First().Markup.Viewpoints.First().Guid, Instance.Topics.First().Viewpoints.First().GUID);
        }

        [Fact]
        public void AddviewpointSnapshotReferenceInMarkup()
        {
            BCFv2Container Instance = new BCFv2Container();
            Instance.Topics.Add(new BCFTopic());
            Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());

            Instance.Topics.First().AddOrUpdateSnapshot(Instance.Topics.First().Viewpoints.First().GUID, new byte[] { 10, 11, 12, 13, 14, 15 });

            Assert.False(string.IsNullOrWhiteSpace(Instance.Topics.First().Markup.Viewpoints.FirstOrDefault().Snapshot), "Reference not created for viewpoint snapshot");
        }

         
        public class ReadStream
        {
             
            public class EmptyProject
            {
                [Fact]
                public void JustRead()
                {
                    var Instance = TestDataFactory.GetContainerForTestCase(TestDataFactory.TestCase.EmptyProject);
                    Assert.NotNull(Instance);
                }

                [Fact]
                public void DontReadEmptyProjectDefinitions()
                {
                    var Instance = TestDataFactory.GetContainerForTestCase(TestDataFactory.TestCase.EmptyProject);
                    Assert.Null(Instance.BCFProject);
                }
            }

             
            public class Extensions
            {
                [Fact]
                public void SimpleExtensions()
                {
                    var OriginalContainer = new BCFv2Container();
                    OriginalContainer.ProjectExtensions = new Extensions_XSD();
                    OriginalContainer.ProjectExtensions.UserIdType.Add("Some user");
                    using (var MemStream = new MemoryStream())
                    {
                        OriginalContainer.WriteStream(MemStream);
                        var ReadContainer = BCFv2Container.ReadStream(MemStream);
                        Assert.NotNull(ReadContainer.ProjectExtensions);
                        Assert.Equal("Some user", ReadContainer.ProjectExtensions.UserIdType.First());
                        Assert.Equal("extensions.xsd", ReadContainer.BCFProject.ExtensionSchema);
                    }
                }
            }
        }
    }

    public static class TestDataFactory
    {
        public static BCFv2Container GetContainerForTestCase(TestCase DesiredTestCase)
        {
            if (!Containers.ContainsKey(DesiredTestCase))
            {
                switch (DesiredTestCase)
                {
                    case TestCase.EmptyProject:
                        Containers.Add(TestCase.EmptyProject, BCFv2Container.ReadStream(new MemoryStream(BCFv2TestResources.EmptyProject)));
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            return Containers[DesiredTestCase];
        }

        private static Dictionary<TestCase, BCFv2Container> _Containers;

        private static Dictionary<TestCase, BCFv2Container> Containers
        {
            get
            {
                return _Containers ?? (_Containers = new Dictionary<TestCase, BCFv2Container>());
            }
        }

        public enum TestCase { EmptyProject }
    }
}
