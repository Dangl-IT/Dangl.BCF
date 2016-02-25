using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.File
{
    [JsonObject(Title = "file")]
    public class file_PUT : file_Base
    {
        public static file_PUT CreateFromJSON(string JSON)
        {
            return (file_PUT) JsonConvert.DeserializeObject(JSON, typeof (file_PUT));
        }
    }
}