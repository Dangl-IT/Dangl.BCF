using System;
using System.Collections.Generic;
using iabi.BCF.BCFv2;
using iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory;
using iabi.BCF.Tests.BCFTestCases.v2.Import;

namespace iabi.BCF.Tests.BCFTestCases.v2.APIConversion
{
    public static class TestCaseProvider
    {
        public static IEnumerable<ContainerAndName> GetAllContainersFromTestCases()
        {
            // First all created test cases
            foreach (var CurrentEnum in (TestCaseEnum[]) Enum.GetValues(typeof (TestCaseEnum)))
            {
                yield return new ContainerAndName
                {
                    Container = BCFTestCaseFactory.GetContainerByTestName(CurrentEnum),
                    TestName = CurrentEnum.ToString()
                };
            }

            // Then all imported test cases
            foreach (var CurrentEnum in (BCFv2ImportTestCases[]) Enum.GetValues(typeof (BCFv2ImportTestCases)))
            {
                yield return new ContainerAndName
                {
                    Container = TestCaseResourceFactory.GetImportTestCaseContainer(CurrentEnum),
                    TestName = CurrentEnum.ToString()
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