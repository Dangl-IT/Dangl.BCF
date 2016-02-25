using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Extensions
{
    [JsonObject(Title = "extensions")]
    public class extensions_PUT : extensions_Base
    {
        public static extensions_PUT CreateFromJSON(string JSON)
        {
            return (extensions_PUT) JsonConvert.DeserializeObject(JSON, typeof (extensions_PUT));
        }
    }
}