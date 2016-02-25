using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.DocumentReference
{
    [JsonObject(Title = "document_reference")]
    public class document_reference_GET : document_reference_Base
    {
        [JsonProperty(Required = Required.Always, PropertyName = "guid")]
        public override string guid { get; set; }

        public static document_reference_GET CreateFromJSON(string JSON)
        {
            return (document_reference_GET) JsonConvert.DeserializeObject(JSON, typeof (document_reference_GET));
        }
    }
}