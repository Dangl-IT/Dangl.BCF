using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Extensions
{
    [JsonObject(Title = "extensions")]
    public class extensions_GET : extensions_Base
    {
        public static extensions_GET CreateFromJSON(string JSON)
        {
            return (extensions_GET) JsonConvert.DeserializeObject(JSON, typeof (extensions_GET));
        }
    }
}