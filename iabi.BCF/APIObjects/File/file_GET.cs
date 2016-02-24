using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.File
{
    [JsonObject(Title = "file")]
    public class file_GET : file_Base
    {
        public static file_GET CreateFromJSON(string JSON)
        {
            return (file_GET)JsonConvert.DeserializeObject(JSON, typeof(file_GET));
        }
    }
}