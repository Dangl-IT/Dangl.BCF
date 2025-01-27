﻿using System;
using System.IO;
using System.Reflection;
using Dangl.BCF.BCFv2;
using Dangl.BCF.BCFv21;
using Dangl.BCF.BCFv3;

namespace Dangl.BCF.Tests
{
    public class TestCaseResourceFactory
    {
        public const string RESOURCE_NAMESPACE = "Dangl.BCF.Tests.Resources";

        public static byte[] GetViewpointSnapshot(ViewpointSnapshots snapshot)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ViewpointSnapshots.{snapshot}.png";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return ConvertFromStream(resourceStream);
            }
        }

        public static byte[] GetIfcFile(IfcFiles ifcFile)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.IfcFiles.{ifcFile}.ifc";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return ConvertFromStream(resourceStream);
            }
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
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return ConvertFromStream(resourceStream);
            }
        }

        public static string GetReadmeForV2(BcfTestCaseV2 testCase)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ExportTestCaseReadmes.v2.{testCase}.md";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                using (var streamReader = new StreamReader(resourceStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static string GetReadmeForV21(BcfTestCaseV21 testCase)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ExportTestCaseReadmes.v21.{testCase}.md";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                using (var streamReader = new StreamReader(resourceStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static byte[] GetImportTestCase(BCFv2ImportTestCases testCase)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ImportTestCases.v2.{testCase}.bcfzip";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return ConvertFromStream(resourceStream);
            }
        }

        public static BCFv2Container GetImportTestCaseContainer(BCFv2ImportTestCases testCase)
        {
            return BCFv2Container.ReadStream(new MemoryStream(GetImportTestCase(testCase)));
        }

        public static BCFv2Container GetCustomTestContainerV2(CustomTestFilesv2 testFile)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.CustomTestFiles.v2.{testFile}.bcfzip";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return BCFv2Container.ReadStream(resourceStream);
            }
        }

        public static byte[] GetImportTestCaseV21(BCFv21ImportTestCases testCase)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ImportTestCases.v21.{testCase}.bcf";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return ConvertFromStream(resourceStream);
            }
        }

        public static byte[] GetImportTestCaseV3(BCFv3ImportTestCases testCase)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.ImportTestCases.v3.{testCase}.bcf";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return ConvertFromStream(resourceStream);
            }
        }

        public static BCFv21Container GetImportTestCaseContainerV21(BCFv21ImportTestCases testCase)
        {
            return BCFv21Container.ReadStream(new MemoryStream(GetImportTestCaseV21(testCase)));
        }

        public static BCFv3Container GetImportTestCaseContainerV3(BCFv3ImportTestCases testCase)
        {
            return BCFv3Container.ReadStream(new MemoryStream(GetImportTestCaseV3(testCase)));
        }

        public static BCFv21Container GetCustomTestContainerV21(CustomTestFilesv21 testFile)
        {
            var resourcePath = $"{RESOURCE_NAMESPACE}.CustomTestFiles.v21.{testFile}.bcf";
            using (var resourceStream = GetResourceStreamFromResourcePath(resourcePath))
            {
                return BCFv21Container.ReadStream(resourceStream);
            }
        }

        public static Stream GetResourceStreamFromResourcePath(string resourcePath)
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

    public enum CustomTestFilesv2
    {
        EmptyProject
    }

    public enum CustomTestFilesv21
    {
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

    public enum BCFv21ImportTestCases
    {
        AllComponentsAndSpacesVisible,
        RelatedTopicsWithBothTopicsInSameFile,
        RelatedTopicsWithOtherTopicMissing,
        SingleInvisibleWall,
        SingleVisibleSpace,
        SingleVisibleWall,
        UserAssignment
    }

    public enum BCFv3ImportTestCases
    {
        AllComponentsAndSpacesVisible,
        RelatedTopicsWithBothTopicsInSameFile,
        SingleInvisibleWall,
        SingleVisibleSpace,
        SingleVisibleWall,
        UserAssignment
    }
}
