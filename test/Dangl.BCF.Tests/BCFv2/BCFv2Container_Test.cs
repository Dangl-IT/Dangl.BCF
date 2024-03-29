using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Dangl.BCF.BCFv2;
using Dangl.BCF.BCFv2.Schemas;
using Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using Xunit;

namespace Dangl.BCF.Tests.BCFv2
{
    public class BcFv2ContainerTest
    {
        /// <summary>
        ///     Not expecting an exception and the markup should then be automatically created (with the viewpoint data)
        /// </summary>
        [Fact]
        public void AddViewpointWhenThereIsNoMarkupInstance()
        {
            var bcfContainer = new BCFv2Container();
            bcfContainer.Topics.Add(new BCFTopic());

            // Markup is empty
            Assert.Null(bcfContainer.Topics.First().Markup);

            bcfContainer.Topics.First().Viewpoints.Add(new VisualizationInfo());

            // Viewpoint defined
            Assert.NotNull(bcfContainer.Topics.First().Markup);
            Assert.Equal(bcfContainer.Topics.First().Markup.Viewpoints.First().Guid, bcfContainer.Topics.First().Viewpoints.First().GUID);
        }

        [Fact]
        public void AddviewpointSnapshotReferenceInMarkup()
        {
            var bcfContainer = new BCFv2Container();
            bcfContainer.Topics.Add(new BCFTopic());
            bcfContainer.Topics.First().Viewpoints.Add(new VisualizationInfo());

            bcfContainer.Topics.First().AddOrUpdateSnapshot(bcfContainer.Topics.First().Viewpoints.First().GUID, new byte[] {10, 11, 12, 13, 14, 15});

            Assert.False(string.IsNullOrWhiteSpace(bcfContainer.Topics.First().Markup.Viewpoints.FirstOrDefault().Snapshot), "Reference not created for viewpoint snapshot");
        }

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
                var input = "some.file";
                var expected = "some.file";
                var actual = BCFv2Container.GetFilenameFromReference(input);
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Check_02()
            {
                var input = "/some.file";
                var expected = "some.file";
                var actual = BCFv2Container.GetFilenameFromReference(input);
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Check_03()
            {
                var input = "../some.file";
                var expected = "some.file";
                var actual = BCFv2Container.GetFilenameFromReference(input);
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Check_04()
            {
                var input = "123/some.file";
                var expected = "some.file";
                var actual = BCFv2Container.GetFilenameFromReference(input);
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Check_05()
            {
                var input = "../123hallo/some.file";
                var expected = "some.file";
                var actual = BCFv2Container.GetFilenameFromReference(input);
                Assert.Equal(expected, actual);
            }
        }


        public class GetAttachmentForDocumentReference
        {
            [Fact]
            public void ExceptionOnExternalReference()
            {
                var container = new BCFv2Container();
                var docRef = new TopicDocumentReferences();
                docRef.isExternal = true;
                Assert.Throws<ArgumentException>(() => { var data = container.GetAttachmentForDocumentReference(docRef); });
            }

            [Fact]
            public void Check_01()
            {
                var container = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.MaximumInformation);

                var references = container.Topics.SelectMany(t => t.Markup.Topic.DocumentReferences);

                foreach (var currentDocument in references.Where(r => !r.isExternal))
                {
                    var retrievedFile = container.GetAttachmentForDocumentReference(currentDocument);
                    Assert.NotNull(retrievedFile);
                    Assert.True(retrievedFile.SequenceEqual(container.FileAttachments[BCFv2Container.GetFilenameFromReference(currentDocument.ReferencedDocument)]));
                }
            }
        }


        public class TransformToRelativePath
        {
            [Fact]
            public void EmptyOnEmptyInput_01()
            {
                var created = BCFv2Container.TransformToRelativePath(null, null);
                Assert.True(string.IsNullOrWhiteSpace(created));
            }

            [Fact]
            public void EmptyOnEmptyInput_02()
            {
                var created = BCFv2Container.TransformToRelativePath(string.Empty, null);
                Assert.True(string.IsNullOrWhiteSpace(created));
            }

            [Fact]
            public void EmptyOnEmptyInput_03()
            {
                var created = BCFv2Container.TransformToRelativePath(null, string.Empty);
                Assert.True(string.IsNullOrWhiteSpace(created));
            }

            [Fact]
            public void EmptyOnEmptyInput_04()
            {
                var created = BCFv2Container.TransformToRelativePath(string.Empty, string.Empty);
                Assert.True(string.IsNullOrWhiteSpace(created));
            }

            [Fact]
            public void Create_01()
            {
                var inputAbsolute = "none.jpg";
                var inputCurrentLoc = string.Empty;
                var expected = "none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_01a()
            {
                var inputAbsolute = "someFolder/none.jpg";
                var inputCurrentLoc = string.Empty;
                var expected = "someFolder/none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_02()
            {
                var inputAbsolute = "none.jpg";
                var inputCurrentLoc = "someFolder";
                var expected = "../none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_03()
            {
                var inputAbsolute = "none.jpg";
                var inputCurrentLoc = "someFolder/nested";
                var expected = "../../none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_04()
            {
                var inputAbsolute = "different/none.jpg";
                var inputCurrentLoc = "someFolder/nested";
                var expected = "../../different/none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_05()
            {
                var inputAbsolute = "someFolder/none.jpg";
                var inputCurrentLoc = "someFolder";
                var expected = "none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_06()
            {
                var inputAbsolute = "someFolder/nested/none.jpg";
                var inputCurrentLoc = "someFolder/nested";
                var expected = "none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_07()
            {
                var inputAbsolute = "totally/different/folder/none.jpg";
                var inputCurrentLoc = "some/other/structure";
                var expected = "../../../totally/different/folder/none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_08()
            {
                var inputAbsolute = "first/topic/none.jpg";
                var inputCurrentLoc = "second/topic";
                var expected = "../../first/topic/none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_09()
            {
                var inputAbsolute = "first/topic/none.jpg";
                var inputCurrentLoc = string.Empty;
                var expected = "first/topic/none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_10()
            {
                var inputAbsolute = "first/topic/none.jpg";
                var expected = "first/topic/none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, null);
                Assert.Equal(expected, created);
            }

            [Fact]
            public void Create_11()
            {
                var inputAbsolute = "first/topic/none.jpg";
                var inputCurrentLoc = "some/other/folder/dir/hello";
                var expected = "../../../../../first/topic/none.jpg";
                var created = BCFv2Container.TransformToRelativePath(inputAbsolute, inputCurrentLoc);
                Assert.Equal(expected, created);
            }
        }


        public class GetAbsolutePath
        {
            [Fact]
            public void EmptyOnEmptyInput_01()
            {
                var created = BCFv2Container.GetAbsolutePath(null, null);
                Assert.True(string.IsNullOrWhiteSpace(created));
            }

            [Fact]
            public void EmptyOnEmptyInput_02()
            {
                var created = BCFv2Container.GetAbsolutePath(null, string.Empty);
                Assert.True(string.IsNullOrWhiteSpace(created));
            }

            [Fact]
            public void EmptyOnEmptyInput_03()
            {
                var created = BCFv2Container.GetAbsolutePath(string.Empty, null);
                Assert.True(string.IsNullOrWhiteSpace(created));
            }

            [Fact]
            public void OnlyBasePath_01()
            {
                var created = BCFv2Container.GetAbsolutePath("SomePath", null);
                Assert.Equal("SomePath", created);
            }

            [Fact]
            public void OnlyBasePath_02()
            {
                var created = BCFv2Container.GetAbsolutePath("SomePath/SomeOther", null);
                Assert.Equal("SomePath/SomeOther", created);
            }

            [Fact]
            public void Combine_01()
            {
                var created = BCFv2Container.GetAbsolutePath("123456", "example.jpg");
                Assert.Equal("123456/example.jpg", created);
            }

            [Fact]
            public void Combine_02()
            {
                var created = BCFv2Container.GetAbsolutePath("123456", "../example.jpg");
                Assert.Equal("example.jpg", created);
            }

            [Fact]
            public void Combine_03()
            {
                var created = BCFv2Container.GetAbsolutePath("123456/789", "example.jpg");
                Assert.Equal("123456/789/example.jpg", created);
            }

            [Fact]
            public void Combine_04()
            {
                var created = BCFv2Container.GetAbsolutePath("123456/789", "../example.jpg");
                Assert.Equal("123456/example.jpg", created);
            }

            [Fact]
            public void Combine_05()
            {
                var created = BCFv2Container.GetAbsolutePath("123456/789", "../../example.jpg");
                Assert.Equal("example.jpg", created);
            }

            [Fact]
            public void Combine_05a()
            {
                var created = BCFv2Container.GetAbsolutePath("123456/789/abc/def", "../../../../example.jpg");
                Assert.Equal("example.jpg", created);
            }

            [Fact]
            public void Combine_05b()
            {
                var created = BCFv2Container.GetAbsolutePath("123456/789/abc/def", "../../../example.jpg");
                Assert.Equal("123456/example.jpg", created);
            }

            [Fact]
            public void Combine_06()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { var created = BCFv2Container.GetAbsolutePath("123456", "../../example.jpg"); });
            }

            [Fact]
            public void Combine_07()
            {
                var created = BCFv2Container.GetAbsolutePath("123456", "789/example.jpg");
                Assert.Equal("123456/789/example.jpg", created);
            }
        }


        public class WriteStream
        {
            public BCFTopic MockBCFv2_WithValues()
            {
                var returnObject = new BCFTopic();

                // Topic definition
                returnObject.Markup = MockTopicMarkup();

                // Append one comment
                returnObject.Markup.Comment = new List<Comment>();
                returnObject.Markup.Comment.Add(new Comment());
                returnObject.Markup.Comment[0].Author = "dangl@iabi.eu";
                returnObject.Markup.Comment[0].Comment1 = "I like the idea of collaboration!";
                returnObject.Markup.Comment[0].Date = new DateTime(2015, 05, 20, 16, 59, 30);
                returnObject.Markup.Comment[0].Guid = "9FFDF348-11B7-48D7-A586-E3EE56BAA351";
                returnObject.Markup.Comment[0].Status = "Info";
                returnObject.Markup.Comment[0].VerbalStatus = "Info";

                // Get some viewpoints
                foreach (var currentViewpoint in MockViewpoints())
                {
                    returnObject.Viewpoints.Add(currentViewpoint);
                }
                return returnObject;
            }

            public Markup MockTopicMarkup()
            {
                var returnObject = new Markup();
                returnObject.Topic.AssignedTo = "linhard@iabi.eu";
                returnObject.Topic.CreationAuthor = "dangl@iabi.eu";
                returnObject.Topic.CreationDate = new DateTime(2015, 05, 20, 16, 11, 34);
                returnObject.Topic.Description = "This is a topic that is just intended to test the functionality."
                                                 + Environment.NewLine + "It even features linebreaks!";
                returnObject.Topic.DocumentReferences = new List<TopicDocumentReferences>();
                returnObject.Topic.DocumentReferences.Add(new TopicDocumentReferences());
                returnObject.Topic.DocumentReferences[0].Description = "This is what a referenced document's description would look like.";
                returnObject.Topic.DocumentReferences[0].Guid = "E181000F-959B-46E4-A37A-7C57704DAA2D";
                returnObject.Topic.DocumentReferences[0].isExternal = true;
                returnObject.Topic.DocumentReferences[0].ReferencedDocument = "https://bim--it-net/Expect404.pdf";
                returnObject.Topic.Guid = "69434D23-0E03-4961-87C3-2DF7DEBCD316";

                returnObject.Topic.Labels = new List<string>();
                returnObject.Topic.Labels.Add("Architecture");
                returnObject.Topic.Labels.Add("Unit Testing");
                returnObject.Topic.Labels.Add("Development");
                returnObject.Topic.Labels.Add("BIM Collaborators");

                returnObject.Topic.Priority = "High";

                returnObject.Topic.ReferenceLink = "http://www.iabi.eu";

                // ReturnObject.Viewpoints // Should be set up automagically!

                return returnObject;
            }

            public ProjectExtensions MockExtensions()
            {
                var returnObject = new ProjectExtensions();
                returnObject.Priority.Add("Low");
                returnObject.Priority.Add("Medium");
                returnObject.Priority.Add("High");
                returnObject.SnippetType.Add("IFC2x3");
                returnObject.SnippetType.Add("IFC4");
                returnObject.TopicLabel.Add("Architecture");
                returnObject.TopicLabel.Add("BIM Collaborators");
                returnObject.TopicLabel.Add("Development");
                returnObject.TopicLabel.Add("Unit Testing");
                returnObject.TopicStatus.Add("Open");
                returnObject.TopicStatus.Add("Closed");
                returnObject.TopicStatus.Add("Reopened");
                returnObject.TopicType.Add("Info");
                returnObject.TopicType.Add("Warning");
                returnObject.TopicType.Add("Error");
                returnObject.UserIdType.Add("dangl@iabi.eu");
                returnObject.UserIdType.Add("linhard@iabi.eu");
                return returnObject;
            }

            public List<VisualizationInfo> MockViewpoints()
            {
                var returnObject = new List<VisualizationInfo>();

                // Make a viewpoint with an orthogonal view
                returnObject.Add(new VisualizationInfo());
                returnObject[0].OrthogonalCamera = new OrthogonalCamera();
                returnObject[0].OrthogonalCamera.CameraDirection = new Direction();
                returnObject[0].OrthogonalCamera.CameraDirection.X = 12.34;
                returnObject[0].OrthogonalCamera.CameraDirection.Y = double.MaxValue;
                returnObject[0].OrthogonalCamera.CameraDirection.Z = 0;
                returnObject[0].OrthogonalCamera.CameraUpVector = new Direction();
                returnObject[0].OrthogonalCamera.CameraUpVector.X = 0;
                returnObject[0].OrthogonalCamera.CameraUpVector.Y = 0;
                returnObject[0].OrthogonalCamera.CameraUpVector.Z = 1;
                returnObject[0].OrthogonalCamera.CameraViewPoint = new Point();
                returnObject[0].OrthogonalCamera.CameraViewPoint.X = 42;
                returnObject[0].OrthogonalCamera.CameraViewPoint.Y = 1337;
                returnObject[0].OrthogonalCamera.CameraViewPoint.Z = 80085;
                returnObject[0].OrthogonalCamera.ViewToWorldScale = 1.123452266578545266425;

                // Make a viewpoint with a perspective view
                returnObject.Add(new VisualizationInfo());
                returnObject[1].PerspectiveCamera = new PerspectiveCamera();
                returnObject[1].PerspectiveCamera.CameraDirection = new Direction();
                returnObject[1].PerspectiveCamera.CameraDirection.X = 12.34;
                returnObject[1].PerspectiveCamera.CameraDirection.Y = double.MaxValue;
                returnObject[1].PerspectiveCamera.CameraDirection.Z = 0;
                returnObject[1].PerspectiveCamera.CameraUpVector = new Direction();
                returnObject[1].PerspectiveCamera.CameraUpVector.X = 0;
                returnObject[1].PerspectiveCamera.CameraUpVector.Y = 0;
                returnObject[1].PerspectiveCamera.CameraUpVector.Z = 1;
                returnObject[1].PerspectiveCamera.CameraViewPoint = new Point();
                returnObject[1].PerspectiveCamera.CameraViewPoint.X = 42;
                returnObject[1].PerspectiveCamera.CameraViewPoint.Y = 1337;
                returnObject[1].PerspectiveCamera.CameraViewPoint.Z = 80085;
                returnObject[1].PerspectiveCamera.FieldOfView = 1.123452266578545266425;

                // Some components
                returnObject[1].Components.Add(new Component());
                returnObject[1].Components[0].AuthoringToolId = "14055C39-C439-4ECF-A162-D89A66F352C6";
                returnObject[1].Components[0].IfcGuid = "dsfhdsiufg";
                returnObject[1].Components[0].Visible = true;
                returnObject[1].Components[0].OriginatingSystem = "UnitTestTool";
                returnObject[1].Components.Add(new Component());
                returnObject[1].Components[1].AuthoringToolId = "E6ABD615-A632-4E27-9D77-AF5ECAA29B86";
                returnObject[1].Components[1].IfcGuid = "89fh475fdfhif";
                returnObject[1].Components[1].Visible = false;
                returnObject[1].Components[1].OriginatingSystem = "UnitTestTool";

                // Make a viewpoint with a snapshot
                returnObject.Add(new VisualizationInfo());

                //// Make one with another snapshot
                returnObject.Add(new VisualizationInfo());

                return returnObject;
            }

            [Fact]
            public void Simple_WriteSingleTopic_Extensions()
            {
                var testContainerInstance = new BCFv2Container();

                testContainerInstance.ProjectExtensions = MockExtensions();

                testContainerInstance.Topics.Add(new BCFTopic());
                testContainerInstance.Topics[0].Markup = new Markup();
                testContainerInstance.Topics[0].Markup.Topic = new Topic();
                testContainerInstance.Topics[0].Markup.Topic.Title = "Sample with extensions.";
                testContainerInstance.Topics[0].Markup.Topic.Guid = Guid.NewGuid().ToString();

                using (var memStream = new MemoryStream())
                {
                    testContainerInstance.WriteStream(memStream);
                    Assert.True(memStream.Length > 0);

                    memStream.Position = 0;
                    var archive = new ZipArchive(memStream);
                    var extensionsEntry = archive.Entries.FirstOrDefault(e => e.FullName == "extensions.xsd");
                    Assert.NotNull(extensionsEntry);
                }
            }

            [Fact]
            public void Simple_WriteSingleTopic()
            {
                var testContainerInstance = new BCFv2Container();

                testContainerInstance.ProjectExtensions = MockExtensions();

                testContainerInstance.Topics.Add(MockBCFv2_WithValues());

                using (var memStream = new MemoryStream())
                {
                    testContainerInstance.WriteStream(memStream);
                    Assert.True(memStream.Length > 0);
                }
            }

            [Fact]
            public void AppendSnapshotInfoToMarkup()
            {
                var instance = new BCFv2Container();
                instance.Topics.Add(new BCFTopic());
                instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                instance.Topics.First().AddOrUpdateSnapshot(instance.Topics.First().Viewpoints.First().GUID, new byte[] {15, 15, 15, 15, 15, 15});

                using (var memStream = new MemoryStream())
                {
                    instance.WriteStream(memStream);
                    memStream.Position = 0;
                    var readAgain = BCFv2Container.ReadStream(memStream);
                    Assert.False(string.IsNullOrWhiteSpace(readAgain.Topics.First().Markup.Viewpoints.First().Viewpoint));
                    Assert.True(readAgain.Topics.First().ViewpointSnapshots.Any());
                }
            }
        }


        public class ProjectAndExtensions
        {
            [Fact]
            public void SetPathToExtensionsWhenExtensionsSet()
            {
                var instance = new BCFv2Container();
                instance.ProjectExtensions = new ProjectExtensions();
                Assert.Equal("extensions.xsd", instance.BcfProject.ExtensionSchema);
            }
        }


        public class ReadStream
        {
            public class EmptyProject
            {
                [Fact]
                public void JustRead()
                {
                    var instance = TestCaseResourceFactory.GetCustomTestContainerV2(CustomTestFilesv2.EmptyProject);
                    Assert.NotNull(instance);
                }

                [Fact]
                public void DontReadEmptyProjectDefinitions()
                {
                    var instance = TestCaseResourceFactory.GetCustomTestContainerV2(CustomTestFilesv2.EmptyProject);
                    Assert.Null(instance.BcfProject);
                }
            }


            public class Extensions
            {
                [Fact]
                public void SimpleExtensions()
                {
                    var originalContainer = new BCFv2Container();
                    originalContainer.ProjectExtensions = new ProjectExtensions();
                    originalContainer.ProjectExtensions.UserIdType.Add("Some user");
                    using (var memStream = new MemoryStream())
                    {
                        originalContainer.WriteStream(memStream);
                        var readContainer = BCFv2Container.ReadStream(memStream);
                        Assert.NotNull(readContainer.ProjectExtensions);
                        Assert.Equal("Some user", readContainer.ProjectExtensions.UserIdType.First());
                        Assert.Equal("extensions.xsd", readContainer.BcfProject.ExtensionSchema);
                    }
                }
            }
        }
    }
}