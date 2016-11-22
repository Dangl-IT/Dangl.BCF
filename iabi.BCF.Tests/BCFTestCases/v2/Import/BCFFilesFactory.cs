using System;
using System.Collections.Generic;
using System.IO;
using iabi.BCF.BCFv2;
using iabi.BCF.Tests.BCFTestCases.v2;

namespace iabi.BCF.Tests.BCFTestCases.v2.Import
{
    public static class BCFFilesFactory
    {
        private static Dictionary<BCFImportTest, BCFv2Container> _Containers;

        private static Dictionary<BCFImportTest, BCFv2Container> Containers
        {
            get { return _Containers ?? (_Containers = new Dictionary<BCFImportTest, BCFv2Container>()); }
        }

        public static BCFv2Container GetContainerForTest(BCFImportTest DesiredTest)
        {
            BCFv2Container CreatedContainer;
            switch (DesiredTest)
            {
                case BCFImportTest.Bitmap:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Bitmap);
                    break;

                case BCFImportTest.Clippingplane:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Clippingplane);
                    break;

                case BCFImportTest.CommentsWithoutViewpoints:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.comments_without_viewpoints);
                    break;

                case BCFImportTest.ComponentColoring:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.component_coloring);
                    break;

                case BCFImportTest.DecomposedObjects:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Decomposed_objects);
                    break;

                case BCFImportTest.DecomposedObjectsWithParentGuid:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Decomposed_object_with_parent_guid);
                    break;

                case BCFImportTest.DefaultComponentVisibility:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.default_component_visibility);
                    break;

                case BCFImportTest.MultipleFilesInHeader:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.multiple_files_in_header);
                    break;

                case BCFImportTest.HeaderWithNoFiles:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.header_with_no_files);
                    break;

                case BCFImportTest.HeaderWithSingleFile:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.header_with_single_file);
                    break;

                case BCFImportTest.Lines:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Lines);
                    break;

                case BCFImportTest.MultipleTopics:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.multiple_topics);
                    break;

                case BCFImportTest.MultipleViewpointsWithoutComments:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.multiple_viewpoints_without_comments);
                    break;

                case BCFImportTest.PerspectiveView:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Perspective_view);
                    break;

                case BCFImportTest.RelatedTopicsWithBothTopicsInSameFile:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.RelatedTopicsWithBothTopicsInSameFile);
                    break;

                case BCFImportTest.RelatedTopicsWithOtherTopicMissing:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.RelatedTopicsWithOtherTopicMissing);
                    break;

                case BCFImportTest.SelectedComponent:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Selected_component);
                    break;

                case BCFImportTest.SingleInvisibleWall:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.SingleInvisibleWall);
                    break;

                case BCFImportTest.SingleVisibleSpace:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.single_visible_space);
                    break;

                case BCFImportTest.SingleVisibleWall:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.single_visible_wall);
                    break;

                case BCFImportTest.UserAssignment:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.user_assignment);
                    break;

                case BCFImportTest.VisibleOpening:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.Visible_Opening);
                    break;

                case BCFImportTest.VisibleSpaceAndRestOfModelVisible:
                    CreatedContainer = ReadFromBinary(BCFTestCasesImportData.visible_space_and_the_rest_of_the_model_visible);
                    break;

                default:
                    throw new NotImplementedException();
            }
            return CreatedContainer;
        }

        private static BCFv2Container ReadFromBinary(byte[] InputData)
        {
            return BCFv2Container.ReadStream(new MemoryStream(InputData));
        }
    }

    public enum BCFImportTest
    {
        Bitmap,
        Clippingplane,
        CommentsWithoutViewpoints,
        ComponentColoring,
        DecomposedObjects,
        DecomposedObjectsWithParentGuid,
        DefaultComponentVisibility,
        MultipleFilesInHeader,
        HeaderWithNoFiles,
        HeaderWithSingleFile,
        Lines,
        MultipleTopics,
        MultipleViewpointsWithoutComments,
        PerspectiveView,
        RelatedTopicsWithBothTopicsInSameFile,
        RelatedTopicsWithOtherTopicMissing,
        SelectedComponent,
        SingleInvisibleWall,
        SingleVisibleSpace,
        SingleVisibleWall,
        UserAssignment,
        VisibleOpening,
        VisibleSpaceAndRestOfModelVisible
    }
}