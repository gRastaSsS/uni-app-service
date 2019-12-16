namespace api_app.Models
{
    using Newtonsoft.Json;

    public class TaskModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "input")]
        public string Input { get; set; }
    }
}