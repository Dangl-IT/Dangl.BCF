using System.Linq;
using Xunit;

namespace iabi.BCF.Tests.BCFTestCases.APIConversion
{
     
    public class Converter
    {
        [Theory]
        [MemberData(nameof(TestCasesContainer))]
        public void ConvertAllTestCases(ContainerAndName input)
        {
                    var ConvertedToApi = iabi.BCF.Converter.APIFromPhysical.Convert(input.Container);
                    var ConvertedBackToPhysical = iabi.BCF.Converter.PhysicalFromAPI.Convert(ConvertedToApi);
                    CompareTool.CompareContainers(input.Container, ConvertedBackToPhysical, null, null, true);
        }

        private static object[] _TestCasesContainer;
        public static object[] TestCasesContainer
        {
            get { return _TestCasesContainer ?? (_TestCasesContainer = TestCaseProvider.GetAllContainersFromTestCases().Select(container => new [] {container}).ToArray()); }
        }
    }
}
