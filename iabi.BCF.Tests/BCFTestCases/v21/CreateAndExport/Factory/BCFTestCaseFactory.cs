using System;
using iabi.BCF.BCFv21;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class BCFTestCaseFactory
    {
        public static BCFv21Container GetContainerByTestName(TestCaseEnum Test)
        {
            if (Test == TestCaseEnum.InternalBIMSnippet)
            {
                return InternalBIMSnippetTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.ExternalBIMSnippet)
            {
                return ExternalBIMSnippetTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.PDFFile)
            {
                return PDFFileTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.ExtensionSchema)
            {
                return ExtensionSchemaTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.MinimumInformation)
            {
                return MinimumInformationTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.MaximumInformation)
            {
                return MaximumInformationTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.PerspectiveCamera)
            {
                return PerspectiveCameraTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.OrthogonalCamera)
            {
                return OrthogonalCameraTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.DocumentReferenceExternal)
            {
                return DocumentReferenceExternalTestCase.CreateContainer();
            }
            if (Test == TestCaseEnum.ComponentSelection)
            {
                return ComponentSelectionTestCase.CreateContainer();
            }
            throw new NotImplementedException();
        }
    }

    public enum TestCaseEnum
    {
        InternalBIMSnippet,
        ExternalBIMSnippet,
        PDFFile,
        ExtensionSchema,
        MinimumInformation,
        MaximumInformation,
        PerspectiveCamera,
        OrthogonalCamera,
        DocumentReferenceExternal,
        ComponentSelection
    }
}