using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Component
{
    [JsonObject(Title = "component")]
    public class component_PUT : component_Base
    {
        public static component_PUT CreateFromJSON(string JSON)
        {
            return (component_PUT)JsonConvert.DeserializeObject(JSON, typeof(component_PUT));
        }
    }
}