using System;
using System.Linq;
using Dangl.BCF.APIObjects.V10.Extensions;
using Dangl.BCF.BCFv2;
using Dangl.BCF.BCFv2.Schemas;
using Dangl.BCF.Converter;
using Dangl.BCF.Tests.BCFTestCases;
using Dangl.BCF.Tests.BCFTestCases.v2;
using Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using Xunit;

// TODO MAKE CONVERTER ALSO FOR 2.1 AND 2.0

namespace Dangl.BCF.Tests.Converter
{
    public class APIFromPhysical
    {
        public class TestCaxeMaximumInformation
        {
            public TestCaxeMaximumInformation()
            {
                // Taking the container from the unit test
                if (CreatedContainer == null)
                {
                    CreatedContainer = BcfTestCaseFactory.GetContainerByTestName(TestCaseEnum.MaximumInformation);
                }
                if (CreatedAPIContainer == null)
                {
                    CreatedAPIContainer = BCF.Converter.APIFromPhysical.Convert(CreatedContainer);
                }
            }

            public static BCFv2Container CreatedContainer { get; set; }

            public static APIContainer CreatedAPIContainer { get; set; }

            [Fact]
            public void JustCreate()
            {
                Assert.NotNull(CreatedAPIContainer);
            }

            [Fact]
            public void ProjectCorrect()
            {
                Assert.Equal(CreatedContainer.BcfProject.Project.Name, CreatedAPIContainer.Project.name);
                Assert.Equal(CreatedContainer.BcfProject.Project.ProjectId, CreatedAPIContainer.Project.project_id);
            }

            [Fact]
            public void ExtensionsPresent()
            {
                Assert.NotNull(CreatedAPIContainer.Extensions);
            }

            [Fact]
            public void Extensions_TopicTypesPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.TopicType;
                var ActualValues = CreatedAPIContainer.Extensions.topic_type;
                // Check all present
                Assert.True(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.True(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [Fact]
            public void Extensions_TopicStatiPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.TopicStatus;
                var ActualValues = CreatedAPIContainer.Extensions.topic_status;
                // Check all present
                Assert.True(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.True(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [Fact]
            public void Extensions_TopicLabelsPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.TopicLabel;
                var ActualValues = CreatedAPIContainer.Extensions.topic_label;
                // Check all present
                Assert.True(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.True(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [Fact]
            public void Extensions_PrioritesPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.Priority;
                var ActualValues = CreatedAPIContainer.Extensions.priority;
                // Check all present
                Assert.True(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.True(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [Fact]
            public void Extensions_TopicUsersPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.UserIdType;
                var ActualValues = CreatedAPIContainer.Extensions.user_id_type;
                // Check all present
                Assert.True(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.True(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [Fact]
            public void Extensions_SnippetTypesPresent()
            {
                var ExpectedValues = CreatedContainer.ProjectExtensions.SnippetType;
                var ActualValues = CreatedAPIContainer.Extensions.snippet_type;
                // Check all present
                Assert.True(ExpectedValues.All(Curr => ActualValues.Contains(Curr)), "Missing entry");
                // Check no superflous
                Assert.True(ActualValues.All(Curr => ExpectedValues.Contains(Curr)), "Unexpected entry");
            }

            [Fact]
            public void Topics_CountCorrect()
            {
                Assert.Equal(CreatedContainer.Topics.Count, CreatedAPIContainer.Topics.Count);
            }

            [Fact]
            public void Topics_AllGuidsPresent()
            {
                Assert.True(CreatedContainer.Topics.All(Curr => CreatedAPIContainer.Topics.Any(ApiCurr => ApiCurr.Topic.guid == Curr.Markup.Topic.Guid)));
            }

            [Fact]
            public void Topics_AuthorsCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedAuthor = CurrentTopic.Markup.Topic.CreationAuthor;
                    var ActualAuthor = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.creation_author;
                    Assert.Equal(ExpectedAuthor, ActualAuthor);

                    var ExpectedModifiedAuthor = CurrentTopic.Markup.Topic.ModifiedAuthor;
                    if (!string.IsNullOrWhiteSpace(ExpectedModifiedAuthor))
                    {
                        var ActualModifiedAuthor = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.modified_author;
                        Assert.Equal(ExpectedModifiedAuthor, ActualModifiedAuthor);
                    }
                }
            }

            [Fact]
            public void Topics_DatesCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedDate = CurrentTopic.Markup.Topic.CreationDate;
                    var ActualDate = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.creation_date;
                    Assert.Equal(ExpectedDate, ActualDate);

                    if (CurrentTopic.Markup.Topic.ShouldSerializeModifiedDate())
                    {
                        var ExpectedModifiedDate = CurrentTopic.Markup.Topic.ModifiedDate;
                        var ActualModifiedDate = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.modified_date;
                        Assert.Equal(ExpectedModifiedDate, ActualModifiedDate);
                    }
                }
            }

            [Fact]
            public void Topics_DescriptionCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedDescription = CurrentTopic.Markup.Topic.Description;
                    var ActualDescription = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.description;
                    Assert.Equal(ExpectedDescription, ActualDescription);
                }
            }

            [Fact]
            public void Topics_BIMSnippetCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Topic.ShouldSerializeBimSnippet()))
                {
                    var ExpectedSnippet = CurrentTopic.Markup.Topic.BimSnippet;
                    var ActualSnippet = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.bim_snippet;
                    Assert.Equal(ExpectedSnippet.isExternal, ActualSnippet.is_external);
                    Assert.Equal(ExpectedSnippet.Reference, ActualSnippet.reference);
                    Assert.Equal(ExpectedSnippet.ReferenceSchema, ActualSnippet.reference_schema);
                    Assert.Equal(ExpectedSnippet.SnippetType, ActualSnippet.snippet_type);
                }
                // Should be null in api container when not given in input
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => !Curr.Markup.Topic.ShouldSerializeBimSnippet()))
                {
                    Assert.Null(CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.bim_snippet);
                }
            }

            [Fact]
            public void Topics_BIMSnippetDataPresent()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Topic.ShouldSerializeBimSnippet() && Curr.SnippetData != null))
                {
                    var ExpectedSnippet = CurrentTopic.SnippetData;
                    var ActualData = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).SnippetData;
                    Assert.True(ExpectedSnippet.SequenceEqual(ActualData));
                }
            }

            [Fact]
            public void Topics_TypeCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ExpectedType = CurrentTopic.Markup.Topic.TopicType;
                    var ActualType = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.topic_type;
                    Assert.Equal(ExpectedType, ActualType);
                }
            }

            [Fact]
            public void Topics_StatusCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.TopicStatus;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.topic_status;
                    Assert.Equal(Expected, Actual);
                }
            }

            [Fact]
            public void Topics_TitleCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.Title;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.title;
                    Assert.Equal(Expected, Actual);
                }
            }

            [Fact]
            public void Topics_LabelsCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.Labels;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.labels;
                    Assert.True(Expected.SequenceEqual(Actual));
                }
            }

            [Fact]
            public void Topics_AssignedToCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.AssignedTo;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.assigned_to;
                    Assert.Equal(Expected, Actual);
                }
            }

            [Fact]
            public void RelatedTopicsCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.RelatedTopics;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).RelatedTopics;
                    Assert.True(Expected.Select(Curr => Curr.Guid).ToList().SequenceEqual(Actual.Select(Curr => Curr.related_topic_guid).ToList()));
                }
            }

            [Fact]
            public void Topics_ReferenceCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var Expected = CurrentTopic.Markup.Topic.ReferenceLink;
                    var Actual = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Topic.reference_link;
                    Assert.Equal(Expected, Actual);
                }
            }

            [Fact]
            public void Topics_Viewpoints_GuidsPresentAndNoneAdded()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var ApiTopic = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);

                    var ApiViewpointGuids = ApiTopic.Viewpoints.Select(Curr => Curr.Viewpoint.guid);
                    var XmlViewpointGuids = CurrentTopic.Viewpoints.Select(Curr => Curr.GUID);

                    // Check all present
                    Assert.True(ApiViewpointGuids.All(Curr => XmlViewpointGuids.Contains(Curr)));
                    Assert.True(CurrentTopic.Markup.Viewpoints.All(Curr => ApiTopic.Viewpoints.Any(ApiViewPt => ApiViewPt.Viewpoint.guid == Curr.Guid)));

                    // Check none superfluous
                    Assert.True(XmlViewpointGuids.All(Curr => ApiViewpointGuids.Contains(Curr)));
                    Assert.True(ApiTopic.Viewpoints.All(Curr => CurrentTopic.Markup.Viewpoints.Any(XmlVP => XmlVP.Guid == Curr.Viewpoint.guid)));
                }
            }

            [Fact]
            public void Topics_Viewpoints_CountCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Markup.Viewpoints;
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;
                    Assert.Equal(XmlViewpoints.Count, ApiViewpoints.Count);
                }
            }

            [Fact]
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
                        Assert.NotNull(CurrApiViewpoint.Snapshot);
                        Assert.True(CurrApiViewpoint.Snapshot.SequenceEqual(XmlViewpointSnapshotData), "Snapshot changed");
                    }
                    // Viewpoints that dont have one
                    foreach (var CurrXmlViewpoint in XmlViewpoints.Where(Curr => !CurrentTopic.ViewpointSnapshots.ContainsKey(Curr.Guid)))
                    {
                        var CurrApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrXmlViewpoint.Guid);
                        Assert.Null(CurrApiViewpoint.Snapshot);
                    }
                }
            }

            [Fact]
            public void Topics_Viewpoints_PerspectiveCamerasPresentAndCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.Equal(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.ShouldSerializePerspectiveCamera()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraDirection.X, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_direction.x);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraDirection.Y, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_direction.y);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraDirection.Z, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_direction.z);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraUpVector.X, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_up_vector.x);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraUpVector.Y, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_up_vector.y);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraUpVector.Z, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_up_vector.z);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraViewPoint.X, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_view_point.x);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraViewPoint.Y, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_view_point.y);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.CameraViewPoint.Z, CurrentApiViewpoint.Viewpoint.perspective_camera.camera_view_point.z);
                        Assert.Equal(CurrentViewpoint.PerspectiveCamera.FieldOfView, CurrentApiViewpoint.Viewpoint.perspective_camera.field_of_view);
                    }
                }
            }

            [Fact]
            public void Topics_Viewpoints_OrthogonalCamerasPresentAndCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.Equal(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.ShouldSerializeOrthogonalCamera()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraDirection.X, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_direction.x);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraDirection.Y, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_direction.y);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraDirection.Z, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_direction.z);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraUpVector.X, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.x);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraUpVector.Y, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.y);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraUpVector.Z, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_up_vector.z);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraViewPoint.X, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_view_point.x);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraViewPoint.Y, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_view_point.y);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.CameraViewPoint.Z, CurrentApiViewpoint.Viewpoint.orthogonal_camera.camera_view_point.z);
                        Assert.Equal(CurrentViewpoint.OrthogonalCamera.ViewToWorldScale, CurrentApiViewpoint.Viewpoint.orthogonal_camera.view_to_world_scale);
                    }
                }
            }

            [Fact]
            public void Topics_Viewpoints_LinesPresentAndDataCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.Equal(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.Lines.Any()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);

                        // Lines count correct
                        Assert.Equal(CurrentViewpoint.Lines.Count, CurrentApiViewpoint.Viewpoint.lines.line.Count);

                        var ApiLines = CurrentApiViewpoint.Viewpoint.lines.line.ToList();
                        for (var i = 0; i < CurrentViewpoint.Lines.Count; i++)
                        {
                            // start
                            Assert.Equal(CurrentViewpoint.Lines[i].StartPoint.X, ApiLines[i].start_point.x);
                            Assert.Equal(CurrentViewpoint.Lines[i].StartPoint.Y, ApiLines[i].start_point.y);
                            Assert.Equal(CurrentViewpoint.Lines[i].StartPoint.Z, ApiLines[i].start_point.z);
                            // end
                            Assert.Equal(CurrentViewpoint.Lines[i].EndPoint.X, ApiLines[i].end_point.x);
                            Assert.Equal(CurrentViewpoint.Lines[i].EndPoint.Y, ApiLines[i].end_point.y);
                            Assert.Equal(CurrentViewpoint.Lines[i].EndPoint.Z, ApiLines[i].end_point.z);
                        }
                    }
                }
            }

            [Fact]
            public void Topics_Viewpoints_PlanesPresentAndDataCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics.Where(Curr => Curr.Markup.Viewpoints.Any()))
                {
                    var XmlViewpoints = CurrentTopic.Viewpoints.Where(Curr => CurrentTopic.Viewpoints.Any(VP => VP.GUID == Curr.GUID));
                    var ApiViewpoints = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Viewpoints;

                    Assert.Equal(XmlViewpoints.Count(), ApiViewpoints.Count);

                    foreach (var CurrentViewpoint in XmlViewpoints.Where(Curr => Curr.ClippingPlanes.Any()))
                    {
                        var CurrentApiViewpoint = ApiViewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID);

                        // Lines count correct
                        Assert.Equal(CurrentViewpoint.ClippingPlanes.Count, CurrentApiViewpoint.Viewpoint.clipping_planes.clipping_plane.Count);

                        var ApiPlanes = CurrentApiViewpoint.Viewpoint.clipping_planes.clipping_plane.ToList();
                        for (var i = 0; i < CurrentViewpoint.ClippingPlanes.Count; i++)
                        {
                            // location
                            Assert.Equal(CurrentViewpoint.ClippingPlanes[i].Location.X, ApiPlanes[i].location.x);
                            Assert.Equal(CurrentViewpoint.ClippingPlanes[i].Location.Y, ApiPlanes[i].location.y);
                            Assert.Equal(CurrentViewpoint.ClippingPlanes[i].Location.Z, ApiPlanes[i].location.z);
                            // direction
                            Assert.Equal(CurrentViewpoint.ClippingPlanes[i].Direction.X, ApiPlanes[i].direction.x);
                            Assert.Equal(CurrentViewpoint.ClippingPlanes[i].Direction.Y, ApiPlanes[i].direction.y);
                            Assert.Equal(CurrentViewpoint.ClippingPlanes[i].Direction.Z, ApiPlanes[i].direction.z);
                        }
                    }
                }
            }

            [Fact]
            public void Components_CountCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    foreach (var CurrentViewpoint in CurrentTopic.Viewpoints)
                    {
                        var ApiComponents = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid)
                            .Viewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID)
                            .Components.ToList();

                        Assert.Equal(CurrentViewpoint.Components.Count, ApiComponents.Count);
                    }
                }
            }

            [Fact]
            public void Components_ValuesCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    foreach (var CurrentViewpoint in CurrentTopic.Viewpoints)
                    {
                        var ApiComponents = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid)
                            .Viewpoints.FirstOrDefault(Curr => Curr.Viewpoint.guid == CurrentViewpoint.GUID)
                            .Components.ToList();
                        for (var i = 0; i < CurrentViewpoint.Components.Count; i++)
                        {
                            var ApiComponent = ApiComponents[i];
                            var XmlComponent = CurrentViewpoint.Components[i];

                            if (XmlComponent.SelectedSpecified)
                            {
                                Assert.Equal(XmlComponent.Selected, ApiComponent.selected);
                            }
                            else
                            {
                                Assert.False(ApiComponent.selected);
                            }
                            if (XmlComponent.ShouldSerializeVisible())
                            {
                                Assert.Equal(XmlComponent.Visible, ApiComponent.visible);
                            }
                            else
                            {
                                Assert.False(ApiComponent.visible);
                            }

                            Assert.Equal(XmlComponent.AuthoringToolId, ApiComponent.authoring_tool_id);
                            Assert.Equal(XmlComponent.Color?.ToString(), ApiComponent.color);
                            Assert.Equal(XmlComponent.IfcGuid, ApiComponent.ifc_guid);
                            Assert.Equal(XmlComponent.OriginatingSystem, ApiComponent.originating_system);
                        }
                    }
                }
            }

            [Fact]
            public void FileAttachments_CountCorrect()
            {
                var ApiFileAttachments = CreatedAPIContainer.FileAttachments.Count;
                var XmlFileAttachments = CreatedContainer.FileAttachments.Count;
                Assert.Equal(ApiFileAttachments, XmlFileAttachments);
            }

            [Fact]
            public void FileAttachments_BinaryDataCorrect()
            {
                foreach (var CurrentXmlFile in CreatedContainer.FileAttachments)
                {
                    Assert.Contains(CreatedAPIContainer.FileAttachments, Curr => Curr.Value.SequenceEqual(CurrentXmlFile.Value));
                }
            }

            [Fact]
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
                            Assert.True(false, "Could not locate document reference, neither by guid nor description");
                        }
                        if (CurrentDocRef.isExternal)
                        {
                            Assert.Equal(CurrentDocRef.Guid, CurrentApiDocRef.guid); // This is kinda clear hear, but it looks better=)
                            Assert.Equal(CurrentDocRef.ReferencedDocument, CurrentApiDocRef.referenced_document);
                            Assert.Equal(CurrentDocRef.Description, CurrentApiDocRef.description);
                        }
                        else
                        {
                            Assert.Equal(CurrentDocRef.Guid, CurrentApiDocRef.guid); // This is kinda clear hear, but it looks better=)
                            Assert.Equal(CurrentDocRef.Description, CurrentApiDocRef.description);

                            // Attachment present and equal
                            var ApiFileNameForAttachment = BCFv2Container.GetFilenameFromReference(CurrentApiDocRef.referenced_document);

                            Assert.True(CreatedContainer.GetAttachmentForDocumentReference(CurrentDocRef).SequenceEqual(CreatedAPIContainer.FileAttachments[ApiFileNameForAttachment]));
                        }
                    }
                }
            }

            [Fact]
            public void Comments_CountCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    Assert.Equal(CurrentTopic.Markup.Comment.Count, ApiComments.Comments.Count);
                }
            }

            [Fact]
            public void Comments_AuthorsAndDatesCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        Assert.Equal(CurrentComment.Author, ApiComment.author);
                        Assert.Equal(CurrentComment.Date, ApiComment.date);
                    }
                }
            }

            [Fact]
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
                            Assert.Equal(CurrentComment.ModifiedAuthor, ApiComment.modified_author);
                            Assert.Equal(CurrentComment.ModifiedDate, ApiComment.modified_date);
                        }
                        else
                        {
                            Assert.True(string.IsNullOrWhiteSpace(ApiComment.modified_author));
                            Assert.Null(ApiComment.modified_date);
                        }
                    }
                }
            }

            [Fact]
            public void Comments_StatusCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        Assert.Equal(CurrentComment.Status, ApiComment.status);
                        Assert.Equal(CurrentComment.VerbalStatus, ApiComment.verbal_status);
                    }
                }
            }

            [Fact]
            public void Comments_TextCorrect()
            {
                foreach (var CurrentTopic in CreatedContainer.Topics)
                {
                    var ApiComments = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid);
                    foreach (var CurrentComment in CurrentTopic.Markup.Comment)
                    {
                        var ApiComment = CreatedAPIContainer.Topics.FirstOrDefault(Curr => Curr.Topic.guid == CurrentTopic.Markup.Topic.Guid).Comments.FirstOrDefault(Curr => Curr.guid == CurrentComment.Guid);
                        Assert.Equal(CurrentComment.Comment1, ApiComment.comment);
                    }
                }
            }

            [Fact]
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
                            Assert.Equal(CurrentComment.Viewpoint.Guid, ApiComment.viewpoint_guid);
                        }
                        else
                        {
                            Assert.True(string.IsNullOrWhiteSpace(ApiComment.viewpoint_guid));
                        }
                    }
                }
            }

            [Fact]
            public void ConvertBackToPhysicalAndDontCrash()
            {
                var PhysicalAgain = BCF.Converter.PhysicalFromApi.Convert(CreatedAPIContainer);
                Assert.NotNull(PhysicalAgain);
            }

            [Fact]
            public void ConvertBackToPhysicalAndStillCorrect()
            {
                var PhysicalAgain = BCF.Converter.PhysicalFromApi.Convert(CreatedAPIContainer);
                CompareTool.CompareContainers(CreatedContainer, PhysicalAgain);
            }
        }


        public class SnapshotConversion
        {
            [Fact]
            public void ReadSnapshotInfo()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                Instance.Topics.First().AddOrUpdateSnapshot(Instance.Topics.First().Viewpoints.First().GUID, new byte[] {15, 15, 15, 15, 15, 15});

                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;

                Assert.NotNull(ApiContainerInstance.Topics.First().Viewpoints.First().Snapshot);

                var ReadBackContainer = BCF.Converter.PhysicalFromApi.Convert(ApiContainerInstance);

                Assert.NotNull(ReadBackContainer.Topics.First().ViewpointSnapshots[Instance.Topics.First().Viewpoints.First().GUID]);
                Assert.NotNull(ReadBackContainer.Topics.First().ViewpointSnapshots[Instance.Topics.First().Viewpoints.First().GUID]);
            }
        }


        public class ExtensionsConversion
        {
            [Fact]
            public void DontCreateExtensionsWhenNotPresent()
            {
                var Instance = new BCFv2Container();
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.Null(ApiContainerInstance.Extensions);
            }

            [Fact]
            public void CreateExtensionsWhenPresent()
            {
                var Instance = new BCFv2Container();
                Instance.ProjectExtensions = new ProjectExtensions();
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.NotNull(ApiContainerInstance.Extensions);
            }

            [Fact]
            public void CreatedExtensionsMatch()
            {
                var Instance = new BCFv2Container();
                Instance.ProjectExtensions = new ProjectExtensions();

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

                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;

                CompareExtensions(Instance.ProjectExtensions, ApiContainerInstance.Extensions);
            }

            private void CompareExtensions(ProjectExtensions ExpectedExtensions, extensions_GET ActualExtensions)
            {
                Assert.True(ExpectedExtensions.Priority.SequenceEqual(ActualExtensions.priority));
                Assert.True(ExpectedExtensions.SnippetType.SequenceEqual(ActualExtensions.snippet_type));
                Assert.True(ExpectedExtensions.TopicLabel.SequenceEqual(ActualExtensions.topic_label));
                Assert.True(ExpectedExtensions.TopicStatus.SequenceEqual(ActualExtensions.topic_status));
                Assert.True(ExpectedExtensions.TopicType.SequenceEqual(ActualExtensions.topic_type));
                Assert.True(ExpectedExtensions.UserIdType.SequenceEqual(ActualExtensions.user_id_type));
            }
        }


        public class GeneralTests
        {
            [Fact]
            public void ReadEmptyContainer()
            {
                var Instance = new BCFv2Container();
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.NotNull(ApiContainerInstance);
            }

            [Fact]
            public void ReadContainerWithEmptyViewpoint()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.NotNull(ApiContainerInstance);
            }

            [Fact]
            public void ReadContainerWithEmptyTopic()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.NotNull(ApiContainerInstance);
            }

            [Fact]
            public void ReadAndWriteEmptyContainer()
            {
                var Instance = new BCFv2Container();
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                var ReadBackContainer = BCF.Converter.PhysicalFromApi.Convert(ApiContainerInstance);
                Assert.NotNull(ReadBackContainer);
            }

            [Fact]
            public void ReadAndWriteContainerWithEmptyViewpoint()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Viewpoints.Add(new VisualizationInfo());
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                var ReadBackContainer = BCF.Converter.PhysicalFromApi.Convert(ApiContainerInstance);
                Assert.NotNull(ReadBackContainer);
            }

            [Fact]
            public void ReadAndWriteContainerWithEmptyTopic()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                var ReadBackContainer = BCF.Converter.PhysicalFromApi.Convert(ApiContainerInstance);
                Assert.NotNull(ReadBackContainer);
            }
        }


        public class TopicInformation
        {
            [Fact]
            public void NoModifiedInfoWhenNotPresent()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.True(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Topic.modified_author));
                Assert.True(default(DateTime) == ApiContainerInstance.Topics.First().Topic.modified_date);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_01()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Topic.ModifiedAuthor = "Georg";
                Instance.Topics.First().Markup.Topic.ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.Equal("Georg", ApiContainerInstance.Topics.First().Topic.modified_author);
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Topic.modified_date);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_02()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Topic.ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.True(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Topic.modified_author));
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Topic.modified_date);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_03()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Topic.ModifiedAuthor = "Georg";
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.Equal("Georg", ApiContainerInstance.Topics.First().Topic.modified_author);
                Assert.True(default(DateTime) == ApiContainerInstance.Topics.First().Topic.modified_date);
            }
        }


        public class CommentInformation
        {
            [Fact]
            public void NoModifiedInfoWhenNotPresent()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.True(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Comments.First().modified_author));
                Assert.True(default(DateTime) == ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_01()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                Instance.Topics.First().Markup.Comment.First().ModifiedAuthor = "Georg";
                Instance.Topics.First().Markup.Comment.First().ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.Equal("Georg", ApiContainerInstance.Topics.First().Comments.First().modified_author);
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_02()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                Instance.Topics.First().Markup.Comment.First().ModifiedDate = new DateTime(2015, 06, 06, 15, 47, 18);
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.True(string.IsNullOrWhiteSpace(ApiContainerInstance.Topics.First().Comments.First().modified_author));
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_03()
            {
                var Instance = new BCFv2Container();
                Instance.Topics.Add(new BCFTopic());
                Instance.Topics.First().Markup = new Markup();
                Instance.Topics.First().Markup.Comment.Add(new Comment());
                Instance.Topics.First().Markup.Comment.First().ModifiedAuthor = "Georg";
                var ApiContainerInstance = BCF.Converter.APIFromPhysical.Convert(Instance);
                //var ApiContainerInstance = new Dangl.BCF.Converter.APIFromPhysical(Instance).APIContainer;
                Assert.Equal("Georg", ApiContainerInstance.Topics.First().Comments.First().modified_author);
                Assert.True(default(DateTime) == ApiContainerInstance.Topics.First().Comments.First().modified_date);
            }
        }
    }
}