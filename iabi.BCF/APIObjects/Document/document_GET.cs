using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Document
{
    [JsonObject(Title = "document")]
    public class document_GET
    {
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public string guid { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "filename")]
        public string filename { get; set; }

        public string CreateJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static document_GET CreateFromJSON(string JSON)
        {
            return (document_GET) JsonConvert.DeserializeObject(JSON, typeof (document_GET));
        }
    }
}