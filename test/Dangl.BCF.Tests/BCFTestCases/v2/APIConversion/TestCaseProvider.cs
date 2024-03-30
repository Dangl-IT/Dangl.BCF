using System;
using System.Collections.Generic;
using Dangl.BCF.BCFv2;
using Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;

namespace Dangl.BCF.Tests.BCFTestCases.v2.APIConversion
{
    public static class TestCaseProvider
    {
        public static IEnumerable<ContainerAndName> GetAllContainersFromTestCases()
        {
            // First all created test cases
            foreach (var currentEnum in (TestCaseEnum[]) Enum.GetValues(typeof (TestCaseEnum)))
            {
                yield return new ContainerAndName
                {
                    Container = BcfTestCaseFactory.GetContainerByTestName(currentEnum),
                    TestName = currentEnum.ToString()
                };
            }

            // Then all imported test cases
            foreach (var currentEnum in (BCFv2ImportTestCases[]) Enum.GetValues(typeof (BCFv2ImportTestCases)))
            {
                yield return new ContainerAndName
                {
                    Container = TestCaseResourceFactory.GetImportTestCaseContainer(currentEnum),
                    TestName = currentEnum.ToString()
                };
            }
        }
    }

    public class ContainerAndName
    {
        public BCFv2Container Container { get; set; }

        public string TestName { get; set; }
    }
}