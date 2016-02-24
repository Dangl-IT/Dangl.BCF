using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace iabi.BCF.Test.BCFTestCases.APIConversion
{
    [TestClass]
    public class Converter
    {
        [TestMethod]
        public void ConvertAllTestCases()
        {
            foreach (var CurrentContainer in TestCaseProvider.GetAllContainersFromTestCases())
            {
                try
                {
                    var ConvertedToApi = iabi.BCF.Converter.APIFromPhysical.Convert(CurrentContainer.Container);
                    var ConvertedBackToPhysical = iabi.BCF.Converter.PhysicalFromAPI.Convert(ConvertedToApi);
                    CompareTool.CompareContainers(CurrentContainer.Container, ConvertedBackToPhysical, null, null, true);
                }
                catch (AssertFailedException FailedAssertException)
                {
                    throw new AssertFailedException("Failed in test case: " + CurrentContainer.TestName + Environment.NewLine + FailedAssertException.Message, FailedAssertException);
                }
            }
        }
    }
}