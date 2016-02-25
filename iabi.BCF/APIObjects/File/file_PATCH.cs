using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.File
{
    [JsonObject(Title = "file")]
    public class file_PATCH : file_Base
    {
        public static file_PATCH CreateFromJSON(string JSON)
        {
            return (file_PATCH) JsonConvert.DeserializeObject(JSON, typeof (file_PATCH));
        }
    }
}