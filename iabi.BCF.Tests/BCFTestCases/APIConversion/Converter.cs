using Xunit;
using System;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.APIConversion
{
     
    public class Converter
    {
        [Fact]
        [MemberData(nameof(TestCasesContainer))]
        public void ConvertAllTestCases(ContainerAndName input)
        {
            // TODO MAKE THIS WITH A MEMBERYDATA
            foreach (var CurrentContainer in TestCaseProvider.GetAllContainersFromTestCases())
            {
                    var ConvertedToApi = iabi.BCF.Converter.APIFromPhysical.Convert(CurrentContainer.Container);
                    var ConvertedBackToPhysical = iabi.BCF.Converter.PhysicalFromAPI.Convert(ConvertedToApi);
                    CompareTool.CompareContainers(CurrentContainer.Container, ConvertedBackToPhysical, null, null, true);
            }
        }

        private static object[] _TestCasesContainer;
        private static object[] TestCasesContainer
        {
            get { return _TestCasesContainer ?? (_TestCasesContainer = TestCaseProvider.GetAllContainersFromTestCases().Select(container => new[] {container}).ToArray()); }
        }
    }
}
