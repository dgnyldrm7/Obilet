using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.BusLocation
{
    public class BusLocationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("parent-id")]
        public int? ParentId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("rank")]
        public int? Rank { get; set; }
    }
}
