using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.BusJourney
{
    public class JourneyRequestData
    {
        [JsonPropertyName("origin-id")]
        public int OriginId { get; set; }

        [JsonPropertyName("destination-id")]
        public int DestinationId { get; set; }

        [JsonPropertyName("departure-date")]
        public string DepartureDate { get; set; } = string.Empty; // Format: "yyyy-MM-dd"
    }
}
