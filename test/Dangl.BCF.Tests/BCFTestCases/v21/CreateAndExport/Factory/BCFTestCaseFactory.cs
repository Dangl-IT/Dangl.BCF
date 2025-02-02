﻿using System;
using Dangl.BCF.BCFv21;

namespace Dangl.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class BcfTestCaseFactory
    {
        public static BCFv21Container GetContainerByTestName(TestCaseEnum test)
        {
            if (test == TestCaseEnum.InternalBimSnippet)
            {
                return InternalBimSnippetTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.ExternalBimSnippet)
            {
                return ExternalBimSnippetTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.PdfFile)
            {
                return PdfFileTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.ExtensionSchema)
            {
                return ExtensionSchemaTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.MinimumInformation)
            {
                return MinimumInformationTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.MaximumInformation)
            {
                return MaximumInformationTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.PerspectiveCamera)
            {
                return PerspectiveCameraTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.OrthogonalCamera)
            {
                return OrthogonalCameraTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.DocumentReferenceExternal)
            {
                return DocumentReferenceExternalTestCase.CreateContainer();
            }
            if (test == TestCaseEnum.ComponentSelection)
            {
                return ComponentSelectionTestCase.CreateContainer();
            }
            throw new NotImplementedException();
        }
    }

    public enum TestCaseEnum
    {
        InternalBimSnippet,
        ExternalBimSnippet,
        PdfFile,
        ExtensionSchema,
        MinimumInformation,
        MaximumInformation,
        PerspectiveCamera,
        OrthogonalCamera,
        DocumentReferenceExternal,
        ComponentSelection
    }
}