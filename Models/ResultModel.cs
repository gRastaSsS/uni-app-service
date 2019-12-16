namespace api_app.Models 
{
    using Newtonsoft.Json;

    public class Result
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "output")]
        public string Output { get; set; }
    }
}