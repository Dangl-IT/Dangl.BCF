using iabi.BCF.APIObjects.Extensions;
using Xunit;

namespace iabi.BCF.Tests.APIObjects.Extensions
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
