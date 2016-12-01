using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using iabi.BCF.BCFv2;

// TODO ADD TESTS THAT ALL RESOURCES CAN BE RETRIEVED

namespace iabi.BCF.Tests
{
    public class TestCaseResourceFactory
    {
        public const string RESOURCE_NAMESPACE = "iabi.BCF.Tests.Resources";

        public static byte[] GetViewpointSnapshot(ViewpointSnapshots snapshot)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ViewpointSnapshots.{snapshot}.png";
            var resourceStream = GetResourceStreamFromResourcePath(resourcePath);
            return ConvertFromStream(resourceStream);
        }

        public static byte[] GetIfcFile(IfcFiles ifcFile)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.IfcFiles.{ifcFile}.ifc";
            var resourceStream = GetResourceStreamFromResourcePath(resourcePath);
            return ConvertFromStream(resourceStream);
        }

        public static byte[] GetFileAttachment(FileAttachments file)
        {
            string resourcePath = RESOURCE_NAMESPACE + ".FileAttachments.";
            switch (file)
            {
                case FileAttachments.JsonElement:
                    resourcePath += "JsonElement.json";
                    break;
                case FileAttachments.MarkupSchemaV2:
                    resourcePath += "MarkupSchemaV2.xsd";
                    break;
                case FileAttachments.MarkupSchemaV21:
                    resourcePath += "MarkupSchemaV21.xsd";
                    break;
                case FileAttachments.RequirementsPdf:
                    resourcePath += "Requirements.pdf";
                    break;
                default:
                    throw new NotImplementedException();
            }
            var resourceStream =  GetResourceStreamFromResourcePath(resourcePath);
            return ConvertFromStream(resourceStream);
        }

        public static string GetReadmeForV2(BcfTestCaseV2 testCase)
        {
            throw new NotImplementedException();
        }

        public static string GetReadmeForV21(BcfTestCaseV21 testCase)
        {
            throw new NotImplementedException();
        }

        public static byte[] GetImportTestCase(BCFv2ImportTestCases testCase)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ImportTestCases.v2.{testCase}.bcfzip";
            var resourceStream = GetResourceStreamFromResourcePath(resourcePath);
            return ConvertFromStream(resourceStream);
        }

        public static BCFv2Container GetImportTestCaseContainer(BCFv2ImportTestCases testCase)
        {
            return BCFv2Container.ReadStream(new MemoryStream(GetImportTestCase(testCase)));
        }

        private static Stream GetResourceStreamFromResourcePath(string resourcePath)
        {
            var assembly = typeof(TestCaseResourceFactory).GetTypeInfo().Assembly;
            var resourceStream = assembly.GetManifestResourceStream(resourcePath);
            return resourceStream;
        }

        private static byte[] ConvertFromStream(Stream stream)
        {
            using (var memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                return memStream.ToArray();
            }
        }
    }

    public enum BcfTestCaseV2
    {
        ComponentSelection,
        DocumentReferenceExternal,
        ExtensionSchema,
        ExternalBIMSnippet,
        InternalBIMSnippet,
        MaximumInformation,
        MinimumInformation,
        OrthogonalCamera,
        PDFFile,
        PerspectiveCamera
    }

    public enum BcfTestCaseV21
    {
        ComponentSelection,
        DocumentReferenceExternal,
        ExtensionSchema,
        ExternalBIMSnippet,
        InternalBIMSnippet,
        MaximumInformation,
        MinimumInformation,
        OrthogonalCamera,
        PDFFile,
        PerspectiveCamera
    }

    public enum ViewpointSnapshots
    {
        ComponentCollection_Snapshot_01,
        MaximumInfo_Snapshot_01,
        MaximumInfo_Snapshot_02,
        MaximumInfo_Snapshot_03,
        PerspectiveCamera_Snapshot_01
    }

    public enum IfcFiles
    {
        Estructura,
        IfcPile
    }

    public enum FileAttachments
    {
        JsonElement,
        MarkupSchemaV2,
        MarkupSchemaV21,
        RequirementsPdf
    }

    public enum BCFv2ImportTestCases
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
