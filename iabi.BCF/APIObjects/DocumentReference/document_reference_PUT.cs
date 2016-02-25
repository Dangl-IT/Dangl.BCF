using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.DocumentReference
{
    [JsonObject(Title = "document_reference")]
    public class document_reference_PUT : document_reference_Base
    {
        public static document_reference_PUT CreateFromJSON(string JSON)
        {
            return (document_reference_PUT) JsonConvert.DeserializeObject(JSON, typeof (document_reference_PUT));
        }
    }
}