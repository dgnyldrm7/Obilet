using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.Session
{
    public class SessionResponseData
    {
        [JsonPropertyName("session-id")]
        public string SessionId { get; set; } = string.Empty;

        [JsonPropertyName("device-id")]
        public string DeviceId { get; set; } = string.Empty;
    }
}
