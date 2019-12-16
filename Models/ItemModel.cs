namespace api_app.Models
{
    using Newtonsoft.Json;

    public class Item 
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "session_id")]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "input")]
        public string Input { get; set; }

        [JsonProperty(PropertyName = "output")]
        public string Output { get; set; }

        [JsonProperty(PropertyName = "completed")]
        public bool Completed { get; set; }
    }
}
