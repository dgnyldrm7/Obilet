using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.BusJourney
{
    public class JourneyDetail
    {
        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;

        [JsonPropertyName("destination")]
        public string Destination { get; set; } = string.Empty;

        [JsonPropertyName("departure")]
        public DateTime Departure { get; set; }

        [JsonPropertyName("arrival")]
        public DateTime Arrival { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "TRY";

        [JsonPropertyName("original-price")]
        public decimal OriginalPrice { get; set; }

        [JsonPropertyName("internet-price")]
        public decimal? InternetPrice { get; set; }
    }
}
