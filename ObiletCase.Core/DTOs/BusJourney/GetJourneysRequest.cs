using ObiletCase.Core.DTOs.Session;
using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.BusJourney
{
    public class GetJourneysRequest
    {
        [JsonPropertyName("device-session")]
        public DeviceSession DeviceSession { get; set; } = new();

        [JsonPropertyName("date")]
        public string Date { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

        [JsonPropertyName("language")]
        public string Language { get; set; } = "tr-TR";

        [JsonPropertyName("data")]
        public JourneyRequestData Data { get; set; } = new();
    }
}
