using System.Text.Json.Serialization;

namespace Client.Models
{
    public class Card
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
    }
}
