using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Component
{
    [JsonObject(Title = "component")]
    public class component_GET : component_Base
    {
        public static component_GET CreateFromJSON(string JSON)
        {
            return (component_GET)JsonConvert.DeserializeObject(JSON, typeof(component_GET));
        }
    }
}