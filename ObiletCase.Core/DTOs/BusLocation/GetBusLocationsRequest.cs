using ObiletCase.Core.DTOs.Session;
using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.BusLocation
{
    public class GetBusLocationsRequest
    {
        [JsonPropertyName("data")]
        public string? Data { get; set; }

        [JsonPropertyName("device-session")]
        public DeviceSession DeviceSession { get; set; } = new();

        [JsonPropertyName("date")]
        public string Date { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

        [JsonPropertyName("language")]
        public string Language { get; set; } = "tr-TR";
    }
}
