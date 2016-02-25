using System;
using System.Collections.Generic;
using iabi.BCF.BCFv2;
using iabi.BCF.Tests.BCFTestCases.CreateAndExport.Factory;
using iabi.BCF.Tests.BCFTestCases.Import;

namespace iabi.BCF.Tests.BCFTestCases.APIConversion
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
            foreach (var CurrentEnum in (BCFImportTest[]) Enum.GetValues(typeof (BCFImportTest)))
            {
                yield return new ContainerAndName
                {
                    Container = BCFFilesFactory.GetContainerForTest(CurrentEnum),
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