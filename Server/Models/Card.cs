using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Server.Models
{
    public class Card
    {
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        [Required]
        public string Image { get; set; }

        [JsonIgnore]
        [Required]
        public IFormFile ProfileImage { get; set; }
    }
}
