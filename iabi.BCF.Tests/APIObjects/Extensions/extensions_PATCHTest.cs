using iabi.BCF.APIObjects.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iabi.BCF.Test.APIObjects.Extensions
{
    [TestClass]
    public class extensions_PATCHTest
    {
        [TestClass]
        public class IsEmpty
        {
            [TestMethod]
            public void TrueForNewInstance()
            {
                var Instance = new extensions_PATCH();
                Assert.IsTrue(Instance.IsEmpty());
            }
        }
    }
}