using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Extensions
{
    [JsonObject(Title = "extensions")]
    public class extensions_POST : extensions_Base
    {
        public static extensions_POST CreateFromJSON(string JSON)
        {
            return (extensions_POST) JsonConvert.DeserializeObject(JSON, typeof (extensions_POST));
        }
    }
}