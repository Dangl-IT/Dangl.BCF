using iabi.BCF.APIObjects.Extensions;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using iabi.BCF.Converter;
using iabi.BCF.Test.BCFTestCases;
using iabi.BCF.Test.BCFTestCases.CreateAndExport;
using iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace iabi.BCF.Test.Converter
{
    [TestClass]
    public class APIFromPhysical
    {
        [TestClass]
        public class TestCaxeMaximumInformation
        {
            [ClassInitialize]
            public static void Create(TestContext GivenContext)
            {
                // Taking the container from the unit test
                CreatedContainer = BCFTestCases.CreateAndExport.Factory.BCFTestCaseFactory.GetContainerByTestName(TestCaseEnum.MaximumInformation);
                CreatedAPIContainer = iabi.BCF.Converter.APIFromPhysical.Convert(CreatedContainer);
            }

            public static BCFv2Container CreatedContainer { get; set; }

            public static APIContainer CreatedAPIContainer { get; set; }

            [TestMethod]
            public void JustCreate()
            {
                Assert.IsNotNull(CreatedAPIContainer);
            }

            [TestMethod]
            public void ProjectCorrect()
            {
                Assert.AreEqual(CreatedContainer.BCFProject.Project.Name, CreatedAPIContainer.Project.name);
                Assert.AreEqual(CreatedContainer.BCFProject.Project.ProjectId, CreatedAPIContainer.Project.project_id);
            }

            [TestMethod]
            public void ExtensionsPresent()
            {
                Assert.IsNotNull(CreatedAPIContainer.Extensions);
            }

            [TestMethod]
            public void Extensions_TopicTypesPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.TopicType;
                var ActualValues = CreatedAPIContainer.Extensions.topic_type;
                // Check all present
                Assert.IsTrue(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.IsTrue(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [TestMethod]
            public void Extensions_TopicStatiPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.TopicStatus;
                var ActualValues = CreatedAPIContainer.Extensions.topic_status;
                // Check all present
                Assert.IsTrue(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.IsTrue(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [TestMethod]
            public void Extensions_TopicLabelsPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.TopicLabel;
                var ActualValues = CreatedAPIContainer.Extensions.topic_label;
                // Check all present
                Assert.IsTrue(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.IsTrue(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [TestMethod]
            public void Extensions_PrioritesPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.Priority;
                var ActualValues = CreatedAPIContainer.Extensions.priority;
                // Check all present
                Assert.IsTrue(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.IsTrue(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [TestMethod]
            public void Extensions_TopicUsersPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.UserIdType;
                var ActualValues = CreatedAPIContainer.Extensions.user_id_type;
                // Check all present
                Assert.IsTrue(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.IsTrue(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [TestMethod]
            public void Extensions_SnippetTypesPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.SnippetType;
                var ActualValues = CreatedAPIContainer.Extensions.snippet_type;
                // Check all present
                Assert.IsTrue(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.IsTrue(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [TestMethod]
            public void Topics_CountCorrect()
            {
                Assert.AreEqual(CreatedContainer.Topics.Count, CreatedAPIContainer.Topics.Count);
            }

            [TestMethod]
            public void Topics_AllGuidsPresent()
            {
                Assert.IsTrue(CreatedContainer.Topics.All(Curr => CreatedAPIContainer.Topics.Any(ApiCurr => ApiCurr.Topic.guid == Curr.Markup.Topic.Guid)));
            }

            [TestMethod]
            public void Topics_AuthorsCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedAuthor = CurrentTopic.Markup.Topic.CreationAuthor;
                    var ActualAuthor = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.creation_author;
                    Assert.AreEqual(ExpectedAuthor, ActualAuthor);

                    var ExpectedModifiedAuthor = CurrentTopic.Markup.Topic.ModifiedAuthor;
                    if (!string.IsNullOrWhiteSpace(ExpectedModifiedAuthor))
                    {
                        var ActualModifiedAuthor = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.modified_author;
                        Assert.AreEqual(ExpectedModifiedAuthor, ActualModifiedAuthor);
                    }
                }
            }

            [TestMethod]
            public void Topics_DatesCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedDate = CurrentTopic.Markup.Topic.CreationDate;
                    var ActualDate = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.creation_date;
                    Assert.AreEqual(ExpectedDate, ActualDate);

                    if (CurrentTopic.Markup.Topic.ShouldSerializeModifiedDate())
                    {
                        var ExpectedModifiedDate = CurrentTopic.Markup.Topic.ModifiedDate;
                        var ActualModifiedDate = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.modified_date;
                        Assert.AreEqual(ExpectedModifiedDate, ActualModifiedDate);
                    }
                }
            }

            [TestMethod]
            public void Topics_DescriptionCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedDescription = CurrentTopic.Markup.Topic.Description;
                    var ActualDescription = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.description;
                    Assert.AreEqual(ExpectedDescription, ActualDescription);
                }
            }

            [TestMethod]
            public void Topics_BIMSnippetCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Topic.ShouldSerializeBimSnippet()))
                {
                    var ExpectedSnippet = CurrentTopic.Markup.Topic.BimSnippet;
                    var ActualSnippet = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.bim_snippet;
                    Assert.AreEqual(ExpectedSnippet.isExternal, ActualSnippet.is_external);
                    Assert.AreEqual(ExpectedSnippet.Reference, ActualSnippet.reference);
                    Assert.AreEqual(ExpectedSnippet.ReferenceSchema, ActualSnippet.reference_schema);
                    Assert.AreEqual(ExpectedSnippet.SnippetType, ActualSnippet.snippet_type);
                }
                // Should be null in api container when not given in input
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => !Curr.Markup.Topic.ShouldSerializeBimSnippet()))
                {
                    Assert.IsNull(CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.bim_snippet);
                }
            }

            [TestMethod]
            public void Topics_BIMSnippetDataPresent()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Topic.ShouldSerializeBimSnippet() && Curr.SnippetData != null))
                {
                    var ExpectedSnippet = CurrentTopic.SnippetData;
                    var ActualData = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).SnippetData;
                    Assert.IsTrue(ExpectedSnippet.SequenceEqual(ActualData));
                }
            }

            [TestMethod]
            public void Topics_TypeCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedType = CurrentTopic.Markup.Topic.TopicType;
                    var ActualType = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.topic_type;
                    Assert.AreEqual(ExpectedType, ActualType);
                }
            }

            [TestMethod]
            public void Topics_StatusCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.TopicStatus;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.topic_status;
                    Assert.AreEqual(Expected, Actual);
                }
            }

            [TestMethod]
            public void Topics_TitleCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.Title;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.title;
                    Assert.AreEqual(Expected, Actual);
                }
            }

            [TestMethod]
            public void Topics_LabelsCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.Labels;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.labels;
                    Assert.IsTrue(Expected.SequenceEqual(Actual));
                }
            }

            [TestMethod]
            public void Topics_AssignedToCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.AssignedTo;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.assigned_to;
                    Assert.AreEqual(Expected, Actual);
                }
            }

            [TestMethod]
            public void RelatedTopicsCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.RelatedTopics;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).RelatedTopics;
                    Assert.IsTrue(Expected.Select(Curr => Curr.Guid).ToList().SequenceEqual(Actual.Select(Curr => Curr.related_topic_guid).ToList()));
                }
            }

            [TestMethod]
            public void Topics_ReferenceCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.ReferenceLink;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.reference_link;
                    Assert.AreEqual(Expected, Actual);
                }
            }

            [TestMethod]
            public void Topics_Viewpoints_GuidsPresentAndNoneAdded()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var ApiTopic = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);

                    var ApiViewpointGuids = ApiTopic.Viewpoints.Select(Curr => Curr.Viewpoint.guid);
                    var XmlViewpointGuids = CurrentTopic.Viewpoints.Select(Curr => Curr.GUID);

                    // Check all present
                    Assert.IsTrue(ApiViewpointGuids.All(Curr => XmlViewpointGuids.Contains(Curr)));
                    Assert.IsTrue(CurrentTopic.Markup.Viewpoints.All(Curr => ApiTopic.Viewpoints.Any(ApiViewPt => ApiViewPt.Viewpoint.guid == Curr.Guid)));

                    // Check none superfluous
                    Assert.IsTrue(XmlViewpointGuids.All(Curr => ApiViewpointGuids.Contains(Curr)));
                    Assert.IsTrue(ApiTopic.Viewpoints.All(Curr => CurrentTopic.Markup.Viewpoints.Any(XmlVP => XmlVP.Guid == Curr.Viewpoint.guid)));
                }
            }

            [TestMethod]
            public void Topics_Viewpoints_CountCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Markup.Viewpoints;
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;
                    Assert.AreEqual(XmlViewpoints.Count, ApiViewpoints.Count);
                }
            }

            [TestMethod]
            public void Topics_Viewpoints_SnapshotsPresentAndDataCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Markup.Viewpoints;
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    // Viewpoints that have a snapshot
                    foreach (var CurrXmlViewpoint in XmlViewpoints.Where(Curr => CurrentTopic.ViewpointSnapshots.ContainsKey(Curr.Guid)))
                    {
                        var XmlViewpointSnapshotData = CurrentTopic.ViewpointSnapshots[CurrXmlViewpoint.Guid];
                        var CurrApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrXmlViewpoint.Guid);
                        Assert.IsNotNull(CurrApiViewpoint.Snapshot, "Snapshot is null");
                        Assert.IsTrue(CurrApiViewpoint.Snapshot.SequenceEqual(XmlViewpointSnapshotData), "Snapshot changed");
                    }
                    // Viewpoints that dont have one
                    foreach (var CurrXmlViewpoint in XmlViewpoints.Where(Curr => !CurrentTopic.ViewpointSnapshots.ContainsKey(Curr.Guid)))
                    {
                        var CurrApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrXmlViewpoint.Guid);
                        Assert.IsNull(CurrApiViewpoint.Snapshot, "Snapshot should be null");
                    }
                }
            }

            [TestMethod]
            public void Topics_Viewpoints_PerspectiveCamerasPresentAndCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.AreEqual(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.ShouldSerializePerspectiveCamera()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraDirection.X, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_direction.x);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraDirection.Y, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_direction.y);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraDirection.Z, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_direction.z);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraUpVector.X, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_up_vector.x);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraUpVector.Y, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_up_vector.y);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraUpVector.Z, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_up_vector.z);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraViewPoint.X, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_view_point.x);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraViewPoint.Y, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_view_point.y);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.CameraViewPoint.Z, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_view_point.z);
                        Assert.AreEqual(CurrentViewpoint.PerspectiveCamera.FieldOfView, CurrentApiViewpoint.Viewpoint.perspective_camera.field_of_view);
                    }
                }
            }

            [TestMethod]
            public void Topics_Viewpoints_OrthogonalCamerasPresentAndCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.AreEqual(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.ShouldSerializeOrthogonalCamera()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraDirection.X, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_direction.x);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraDirection.Y, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_direction.y);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraDirection.Z, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_direction.z);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraUpVector.X, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.x);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraUpVector.Y, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.y);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraUpVector.Z, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.z);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraViewPoint.X, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_view_point.x);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraViewPoint.Y, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_view_point.y);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.CameraViewPoint.Z, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_view_point.z);
                        Assert.AreEqual(CurrentViewpoint.OrthogonalCamera.ViewToWorldScale, CurrentApiViewpoint.Viewpoint.orthogonal_camera.view_to_world_scale);
                    }
                }
            }

            [TestMethod]
            public void Topics_Viewpoints_LinesPresentAndDataCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.AreEqual(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.Lines.Any()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);

                        // Lines count correct
                        Assert.AreEqual(CurrentViewpoint.Lines.Count, CurrentApiViewpoint.Viewpoint.lines.line.Count);

                        var ApiLines = CurrentApiViewpoint.Viewpoint.lines.line.ToList();
                        for (int i = 0; i < CurrentViewpoint.Lines.Count; i++)
                        {
                            // start
                            Assert.AreEqual(CurrentViewpoint.Lines[i].StartPoint.X, ApiLines[i].start_point.x);
                            Assert.AreEqual(CurrentViewpoint.Lines[i].StartPoint.Y, ApiLines[i].start_point.y);
                            Assert.AreEqual(CurrentViewpoint.Lines[i].StartPoint.Z, ApiLines[i].start_point.z);
                            // end
                            Assert.AreEqual(CurrentViewpoint.Lines[i].EndPoint.X, ApiLines[i].end_point.x);
                            Assert.AreEqual(CurrentViewpoint.Lines[i].EndPoint.Y, ApiLines[i].end_point.y);
                            Assert.AreEqual(CurrentViewpoint.Lines[i].EndPoint.Z, ApiLines[i].end_point.z);
                        }
                    }
                }
            }

            [TestMethod]
            public void Topics_Viewpoints_PlanesPresentAndDataCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.AreEqual(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.ClippingPlanes.Any()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);

                        // Lines count correct
                        Assert.AreEqual(CurrentViewpoint.ClippingPlanes.Count, CurrentApiViewpoint.Viewpoint.clipping_planes.clipping_plane.Count);

                        var ApiPlanes = CurrentApiViewpoint.Viewpoint.clipping_planes.clipping_plane.ToList();
                        for (int i = 0; i < CurrentViewpoint.ClippingPlanes.Count; i++)
                        {
                            // location
                            Assert.AreEqual(CurrentViewpoint.ClippingPlanes[i].Location.X, ApiPlanes[i].location.x);
                            Assert.AreEqual(CurrentViewpoint.ClippingPlanes[i].Location.Y, ApiPlanes[i].location.y);
                            Assert.AreEqual(CurrentViewpoint.ClippingPlanes[i].Location.Z, ApiPlanes[i].location.z);
                            // direction
                            Assert.AreEqual(CurrentViewpoint.ClippingPlanes[i].Direction.X, ApiPlanes[i].direction.x);
                            Assert.AreEqual(CurrentViewpoint.ClippingPlanes[i].Direction.Y, ApiPlanes[i].direction.y);
                            Assert.AreEqual(CurrentViewpoint.ClippingPlanes[i].Direction.Z, ApiPlanes[i].direction.z);
                        }
                    }
                }
            }

            [TestMethod]
            public void Components_CountCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    foreach (var CurrentViewpoint in CurrentTopic.Viewpoints)
                    {
                        var ApiComponents = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid)
                    .Viewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID)
                    .Components.ToList();

                        Assert.AreEqual(CurrentViewpoint.Components.Count, ApiComponents.Count);
                    }
                }
            }

            [TestMethod]
            public void Components_ValuesCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    foreach (var CurrentViewpoint in CurrentTopic.Viewpoints)
                    {
                        var ApiComponents = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid)
                    .Viewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID)
                    .Components.ToList();
                        for (int i = 0; i < CurrentViewpoint.Components.Count; i++)
                        {
                            var ApiComponent = ApiComponents[i];
                            var XmlComponent = CurrentViewpoint.Components[i];

                            if (XmlComponent.SelectedSpecified)
                            {
                                Assert.AreEqual(XmlComponent.Selected, ApiComponent.selected);
                            }
                            else
                            {
                                Assert.IsFalse(ApiComponent.selected);
                            }
                            if (XmlComponent.ShouldSerializeVisible())
                            {
                                Assert.AreEqual(XmlComponent.Visible, ApiComponent.visible);
                            }
                            else
                            {
                                Assert.IsFalse(ApiComponent.visible);
                            }

                            Assert.AreEqual(XmlComponent.AuthoringToolId, ApiComponent.authoring_tool_id);
                            Assert.AreEqual(XmlComponent.Color, ApiComponent.color);
                            Assert.AreEqual(XmlComponent.IfcGuid, ApiComponent.ifc_guid);
                            Assert.AreEqual(XmlComponent.OriginatingSystem, ApiComponent.originating_system);
                        }
                    }
                }
            }

            [TestMethod]
            public void FileAttachments_CountCorrect()
            {
                var ApiFileAttachments = CreatedAPIContainer.FileAttachments.Count;
                var XmlFileAttachments = CreatedContainer.FileAttachments.Count;
                Assert.AreEqual(ApiFileAttachments, XmlFileAttachments);
            }

            [TestMethod]
            public void FileAttachments_BinaryDataCorrect()
            {
                foreach (var CurrentXmlFile in CreatedContainer.FileAttachments)
                {
                    Assert.IsTrue(CreatedAPIContainer.FileAttachments.Any(Curr => Curr.Value.SequenceEqual(CurrentXmlFile.Value)));
                }
            }

            [TestMethod]
            public void Topics_DocumentReferencesCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var CurrentApiTopic = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentDocRef in CurrentTopic.Markup.Topic.DocumentReferences)
                    {
                        var CurrentApiDocRef = CurrentApiTopic.ReferencedDocuments.FirstOrDefault(Curr => !(string.IsNullOrWhiteSpace(Curr.guid) && Curr.guid == CurrentDocRef.Guid) || Curr.description == CurrentDocRef.Description);
                        if (CurrentApiDocRef == null || (string.IsNullOrWhiteSpace(CurrentApiDocRef.guid) && string.IsNullOrWhiteSpace(CurrentApiDocRef.description)))
                        {
                            Assert.Fail("Could not locate document reference, neither by guid nor description");
                        }
                        if (CurrentDocRef.isExternal)
                        {
                            Assert.AreEqual(CurrentDocRef.Guid, CurrentApiDocRef.guid); // This is kinda clear hear, but it looks better=)
                            Assert.AreEqual(CurrentDocRef.ReferencedDocument, CurrentApiDocRef.referenced_document);
                            Assert.AreEqual(CurrentDocRef.Description, CurrentApiDocRef.description);
                        }
                        else
                        {
                            Assert.AreEqual(CurrentDocRef.Guid, CurrentApiDocRef.guid); // This is kinda clear hear, but it looks better=)
                            Assert.AreEqual(CurrentDocRef.Description, CurrentApiDocRef.description);

                            // Attachment present and equal
                            var ApiFileNameForAttachment = BCFv2Container.GetFilenameFromReference(CurrentApiDocRef.referenced_document);

                            Assert.IsTrue(CreatedContainer.GetAttachmentForDocumentReference(CurrentDocRef).SequenceEqual(CreatedAPIContainer.FileAttachments[ApiFileNameForAttachment]));
                        }
                    }
                }
            }

            [TestMethod]
            public void Comments_CountCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    Assert.AreEqual(CurrentTopic.Markup.Comment.Count, ApiComments.Comments.Count);
                }
            }

            [TestMethod]
            public void Comments_AuthorsAndDatesCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        Assert.AreEqual(CurrentComment.Author, ApiComment.author);
                        Assert.AreEqual(CurrentComment.Date, ApiComment.date);
                    }
                }
            }

            [TestMethod]
            public void Comments_ModifiedInfoCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        if (CurrentComment.ShouldSerializeModifiedDate())
                        {
                            Assert.AreEqual(CurrentComment.ModifiedAuthor, ApiComment.modified_author);
                            Assert.AreEqual(CurrentComment.ModifiedDate, ApiComment.modified_date);
                        }
                        else
                        {
                            Assert.IsTrue(string.IsNullOrWhiteSpace(ApiComment.modified_author));
                            Assert.IsNull(ApiComment.modified_date);
                        }
                    }
                }
            }

            [TestMethod]
            public void Comments_StatusCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        Assert.AreEqual(CurrentComment.Status, ApiComment.status);
                        Assert.AreEqual(CurrentComment.VerbalStatus, ApiComment.verbal_status);
                    }
                }
            }

            [TestMethod]
            public void Comments_TextCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        Assert.AreEqual(CurrentComment.Comment1, ApiComment.comment);
                    }
                }
            }

            [TestMethod]
            public void Comments_ViewpointReferenceCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        if (CurrentComment.Viewpoint != null && !string.IsNullOrWhiteSpace(CurrentComment.Viewpoint.Guid))
                        {
                            Assert.AreEqual(CurrentComment.Viewpoint.Guid, ApiComment.viewpoint_guid);
                        }
                        else
                        {
                            Assert.IsTrue(string.IsNullOrWhiteSpace(ApiComment.viewpoint_guid));
                        }
                    }
                }
            }

            [TestMethod]
            public void ConvertBackToPhysicalAndDontCrash()
            {
                var PhysicalAgain = iabi.BCF.Converter.PhysicalFromAPI.Convert(CreatedAPIContainer);
                Assert.IsNotNull(PhysicalAgain);
            }

            [TestMethod]
            public void ConvertBackToPhysicalAndStillCorrect()
            {
                var PhysicalAgain = iabi.BCF.Converter.PhysicalFromAPI.Convert(CreatedAPIContainer);
                CompareTool.CompareContainers(CreatedContainer, PhysicalAgain);
            }
        }

        [TestClass]
        public class SnapshotConversion
        {
            [TestMethod]
            public void ReadSnapshotInfo()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                Instance.Topics.First().AddOrUpdateSnapshot(Instance.Topics.First().Viewpoints.First().GUID, new byte[] { 15, 15, 15, 15, 15, 15 });

                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;

                Assert.IsNotNull(ApiContainerInstance.Topics.First().Viewpoints.First().Snapshot);

                var ReadBackContainer = iabi.BCF.Converter.PhysicalFromAPI.Convert(ApiContainerInstance);

                Assert.IsNotNull(ReadBackContainer.Topics.First().ViewpointSnapshots[Instance.Topics.First().Viewpoints.First().GUID]);
                Assert.IsNotNull(ReadBackContainer.Topics.First().ViewpointSnapshots[Instance.Topics.First().Viewpoints.First().GUID]);
            }
        }

        [TestClass]
        public class ExtensionsConversion
        {
            [TestMethod]
            public void DontCreateExtensionsWhenNotPresent()
            {
                BCFv2Container Instance = new BCFv2Container();
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsNull(ApiContainerInstance.Extensions);
            }

            [TestMethod]
            public void CreateExtensionsWhenPresent()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.ProjectExtensions = new Extensions_XSD();
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsNotNull(ApiContainerInstance.Extensions);
            }

            [TestMethod]
            public void CreatedExtensionsMatch()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.ProjectExtensions = new Extensions_XSD();

                Instance.ProjectExtensions.Priority.Add("Lorem");
                Instance.ProjectExtensions.Priority.Add("ipsum");
                Instance.ProjectExtensions.SnippetType.Add("dolor");
                Instance.ProjectExtensions.SnippetType.Add("sit");
                Instance.ProjectExtensions.TopicLabel.Add("amet");
                Instance.ProjectExtensions.TopicLabel.Add("consetetur");
                Instance.ProjectExtensions.TopicStatus.Add("sadipscing");
                Instance.ProjectExtensions.TopicStatus.Add("elitr");
                Instance.ProjectExtensions.TopicType.Add("sed");
                Instance.ProjectExtensions.TopicType.Add("diam");
                Instance.ProjectExtensions.UserIdType.Add("nonumy");
                Instance.ProjectExtensions.UserIdType.Add("eirmod");

                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;

                CompareExtensions(Instance.ProjectExtensions, ApiContainerInstance.Extensions);
            }

            private void CompareExtensions(Extensions_XSD ExpectedExtensions, extensions_GET ActualExtensions)
            {
                Assert.IsTrue(ExpectedExtensions.Priority.SequenceEqual(ActualExtensions.priority));
                Assert.IsTrue(ExpectedExtensions.SnippetType.SequenceEqual(ActualExtensions.snippet_type));
                Assert.IsTrue(ExpectedExtensions.TopicLabel.SequenceEqual(ActualExtensions.topic_label));
                Assert.IsTrue(ExpectedExtensions.TopicStatus.SequenceEqual(ActualExtensions.topic_status));
                Assert.IsTrue(ExpectedExtensions.TopicType.SequenceEqual(ActualExtensions.topic_type));
                Assert.IsTrue(ExpectedExtensions.UserIdType.SequenceEqual(ActualExtensions.user_id_type));
            }
        }

        [TestClass]
        public class GeneralTests
        {
            [TestMethod]
            public void ReadEmptyContainer()
            {
                BCFv2Container Instance = new BCFv2Container();
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsNotNull(ApiContainerInstance);
            }

            [TestMethod]
            public void ReadContainerWithEmptyViewpoint()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsNotNull(ApiContainerInstance);
            }

            [TestMethod]
            public void ReadContainerWithEmptyTopic()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsNotNull(ApiContainerInstance);
            }

            [TestMethod]
            public void ReadAndWriteEmptyContainer()
            {
                BCFv2Container Instance = new BCFv2Container();
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                var ReadBackContainer = iabi.BCF.Converter.PhysicalFromAPI.Convert(ApiContainerInstance);
                Assert.IsNotNull(ReadBackContainer);
            }

            [TestMethod]
            public void ReadAndWriteContainerWithEmptyViewpoint()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                var ReadBackContainer = iabi.BCF.Converter.PhysicalFromAPI.Convert(ApiContainerInstance);
                Assert.IsNotNull(ReadBackContainer);
            }

            [TestMethod]
            public void ReadAndWriteContainerWithEmptyTopic()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                var ReadBackContainer = iabi.BCF.Converter.PhysicalFromAPI.Convert(ApiContainerInstance);
                Assert.IsNotNull(ReadBackContainer);
            }
        }

        [TestClass]
        public class TopicInformation
        {
            [TestMethod]
            public void NoModifiedInfoWhenNotPresent()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsTrue(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Topic.modified_author));
                Assert.IsTrue(default(DateTime) == ApiContainerInstance.Topics.First().Topic.modified_date);
            }

            [TestMethod]
            public void ModifiedInfoWhenSpecified_01()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Topic.ModifiedAuthor = "Georg";
                Instance.Topics.First().Markup.Topic.ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.AreEqual("Georg", ApiContainerInstance.Topics.First().Topic.modified_author);
                Assert.AreEqual(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Topic.modified_date);
            }

            [TestMethod]
            public void ModifiedInfoWhenSpecified_02()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Topic.ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsTrue(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Topic.modified_author));
                Assert.AreEqual(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Topic.modified_date);
            }

            [TestMethod]
            public void ModifiedInfoWhenSpecified_03()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Topic.ModifiedAuthor = "Georg";
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.AreEqual("Georg", ApiContainerInstance.Topics.First().Topic.modified_author);
                Assert.IsTrue(default(DateTime) == ApiContainerInstance.Topics.First().Topic.modified_date);
            }
        }

        [TestClass]
        public class CommentInformation
        {
            [TestMethod]
            public void NoModifiedInfoWhenNotPresent()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsTrue(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Comments.First().modified_author));
                Assert.IsTrue(default(DateTime) == ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }

            [TestMethod]
            public void ModifiedInfoWhenSpecified_01()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                Instance.Topics.First().Markup.Comment.First().ModifiedAuthor = "Georg";
                Instance.Topics.First().Markup.Comment.First().ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.AreEqual("Georg", ApiContainerInstance.Topics.First().Comments.First().modified_author);
                Assert.AreEqual(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }

            [TestMethod]
            public void ModifiedInfoWhenSpecified_02()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                Instance.Topics.First().Markup.Comment.First().ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.IsTrue(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Comments.First().modified_author));
                Assert.AreEqual(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }

            [TestMethod]
            public void ModifiedInfoWhenSpecified_03()
            {
                BCFv2Container Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                Instance.Topics.First().Markup.Comment.First().ModifiedAuthor = "Georg";
                var ApiContainerInstance = iabi.BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new iabi.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.AreEqual("Georg", ApiContainerInstance.Topics.First().Comments.First().modified_author);
                Assert.IsTrue(default(DateTime) == ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }
        }
    }
}