using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.BusJourney
{
    public class JourneyDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("partner-id")]
        public long? PartnerId { get; set; }

        [JsonPropertyName("partner-name")]
        public string PartnerName { get; set; } = string.Empty;

        [JsonPropertyName("bus-type")]
        public string? BusType { get; set; }

        [JsonPropertyName("total-seats")]
        public int? TotalSeats { get; set; }

        [JsonPropertyName("available-seats")]
        public int? AvailableSeats { get; set; }

        [JsonPropertyName("journey")]
        public JourneyDetail Journey { get; set; } = new();

        // Dokümanda belirtilen firma logosu URL'i (Örn: 330-sm.png)  ---> Örnek url: https://s3.eu-central-1.amazonaws.com/static.obilet.com/images/partner/330-sm.png
        public string PartnerLogoUrl => PartnerId.HasValue
            ? $"https://s3.eu-central-1.amazonaws.com/static.obilet.com/images/partner/{PartnerId}-sm.png"
            : string.Empty;
    }
}
