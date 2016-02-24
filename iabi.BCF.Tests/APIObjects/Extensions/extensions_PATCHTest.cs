using iabi.BCF.APIObjects.Extensions;
using Xunit;
using System;

namespace iabi.BCF.Test.APIObjects.Extensions
{
     
    public class extensions_PATCHTest
    {
         
        public class IsEmpty
        {
            [Fact]
            public void TrueForNewInstance()
            {
                var Instance = new extensions_PATCH();
                Assert.True(Instance.IsEmpty());
            }
        }
    }
}
