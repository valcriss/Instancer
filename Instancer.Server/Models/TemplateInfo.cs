using System.Text.Json.Serialization;

namespace Instancer.Server.Models
{
    public class TemplateInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("variables")]
        public List<string> Variables { get; set; }
    }
}
