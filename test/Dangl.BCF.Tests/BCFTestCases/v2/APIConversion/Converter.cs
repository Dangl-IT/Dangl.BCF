using System.Linq;
using Dangl.BCF.Converter;
using Xunit;
using System.Collections.Generic;

namespace Dangl.BCF.Tests.BCFTestCases.v2.APIConversion
{
    public class Converter
    {
        private static IEnumerable<object[]> _testCasesContainer;

        public static IEnumerable<object[]> TestCasesContainer
        {
            get { return _testCasesContainer ?? (_testCasesContainer = TestCaseProvider.GetAllContainersFromTestCases().Select(container => new[] {container}).ToArray()); }
        }

        [Theory]
        [MemberData(nameof(TestCasesContainer))]
        public void ConvertAllTestCases(ContainerAndName input)
        {
            var convertedToApi = APIFromPhysical.Convert(input.Container);
            var convertedBackToPhysical = PhysicalFromApi.Convert(convertedToApi);
            CompareTool.CompareContainers(input.Container, convertedBackToPhysical, null, null, true);
        }
    }
}