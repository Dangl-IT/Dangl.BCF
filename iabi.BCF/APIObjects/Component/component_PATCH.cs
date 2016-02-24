using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Component
{
    [JsonObject(Title = "component")]
    public class component_PATCH : component_Base
    {
        public static component_PATCH CreateFromJSON(string JSON)
        {
            return (component_PATCH)JsonConvert.DeserializeObject(JSON, typeof(component_PATCH));
        }
    }
}