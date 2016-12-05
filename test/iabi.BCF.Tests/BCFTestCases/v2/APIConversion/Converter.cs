using System.Linq;
using iabi.BCF.Converter;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.v2.APIConversion
{
    public class Converter
    {
        private static object[] _TestCasesContainer;

        public static object[] TestCasesContainer
        {
            get { return _TestCasesContainer ?? (_TestCasesContainer = TestCaseProvider.GetAllContainersFromTestCases().Select(container => new[] {container}).ToArray()); }
        }

        [Theory]
        [MemberData(nameof(TestCasesContainer))]
        public void ConvertAllTestCases(ContainerAndName input)
        {
            var ConvertedToApi = APIFromPhysical.Convert(input.Container);
            var ConvertedBackToPhysical = PhysicalFromAPI.Convert(ConvertedToApi);
            CompareTool.CompareContainers(input.Container, ConvertedBackToPhysical, null, null, true);
        }
    }
}