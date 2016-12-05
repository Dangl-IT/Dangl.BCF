//using System;
//using System.Collections.Generic;
//using System.IO;
//using iabi.BCF.BCFv2;
//using iabi.BCF.Tests.BCFTestCases.v2;

//namespace iabi.BCF.Tests.BCFTestCases.v2.Import
//{
//    public static class BCFFilesFactory
//    {
//        private static Dictionary<BCFImportTest, BCFv2Container> _Containers;

//        private static Dictionary<BCFImportTest, BCFv2Container> Containers
//        {
//            get { return _Containers ?? (_Containers = new Dictionary<BCFImportTest, BCFv2Container>()); }
//        }



//        public static byte[] GetBcfContainer(BCFImportTest testCase)
//        {
//            throw new NotImplementedException();
//        }

//        public static BCFv2Container GetContainerForTest(BCFImportTest DesiredTest)
//        {
//            BCFv2Container CreatedContainer;
//            throw new NotImplementedException();

//            //switch (DesiredTest)
//            //{
//            //    case BCFImportTest.Bitmap:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Bitmap));
//            //        break;

//            //    case BCFImportTest.Clippingplane:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Clippingplane));
//            //        break;

//            //    case BCFImportTest.CommentsWithoutViewpoints:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.comments_without_viewpoints));
//            //        break;

//            //    case BCFImportTest.ComponentColoring:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.component_coloring));
//            //        break;

//            //    case BCFImportTest.DecomposedObjects:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Decomposed_objects));
//            //        break;

//            //    case BCFImportTest.DecomposedObjectsWithParentGuid:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Decomposed_object_with_parent_guid));
//            //        break;

//            //    case BCFImportTest.DefaultComponentVisibility:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.default_component_visibility));
//            //        break;

//            //    case BCFImportTest.MultipleFilesInHeader:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.multiple_files_in_header));
//            //        break;

//            //    case BCFImportTest.HeaderWithNoFiles:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.header_with_no_files));
//            //        break;

//            //    case BCFImportTest.HeaderWithSingleFile:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.header_with_single_file));
//            //        break;

//            //    case BCFImportTest.Lines:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Lines));
//            //        break;

//            //    case BCFImportTest.MultipleTopics:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.multiple_topics));
//            //        break;

//            //    case BCFImportTest.MultipleViewpointsWithoutComments:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.multiple_viewpoints_without_comments));
//            //        break;

//            //    case BCFImportTest.PerspectiveView:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Perspective_view));
//            //        break;

//            //    case BCFImportTest.RelatedTopicsWithBothTopicsInSameFile:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.RelatedTopicsWithBothTopicsInSameFile));
//            //        break;

//            //    case BCFImportTest.RelatedTopicsWithOtherTopicMissing:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.RelatedTopicsWithOtherTopicMissing));
//            //        break;

//            //    case BCFImportTest.SelectedComponent:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Selected_component));
//            //        break;

//            //    case BCFImportTest.SingleInvisibleWall:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.SingleInvisibleWall));
//            //        break;

//            //    case BCFImportTest.SingleVisibleSpace:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.single_visible_space));
//            //        break;

//            //    case BCFImportTest.SingleVisibleWall:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.single_visible_wall));
//            //        break;

//            //    case BCFImportTest.UserAssignment:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.user_assignment));
//            //        break;

//            //    case BCFImportTest.VisibleOpening:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.Visible_Opening));
//            //        break;

//            //    case BCFImportTest.VisibleSpaceAndRestOfModelVisible:
//            //        CreatedContainer = ReadFromBinary(TestCaseResourceFactory.GetImportTestCase(BCFv2ImportTestCases.visible_space_and_the_rest_of_the_model_visible));
//            //        break;

//            //    default:
//            //        throw new NotImplementedException();
//            //}
//            return CreatedContainer;
//        }

//        private static BCFv2Container ReadFromBinary(string resourcePath)
//        {
//            throw new NotImplementedException();
//            //return BCFv2Container.ReadStream(new MemoryStream(InputData));
//        }
//    }


//}