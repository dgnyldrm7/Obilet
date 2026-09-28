using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.Session
{
    public class GetSessionRequest
    {
        [JsonPropertyName("type")]
        public int Type { get; set; } = 1;

        [JsonPropertyName("connection")]
        public ConnectionInfo Connection { get; set; } = new();

        [JsonPropertyName("browser")]
        public BrowserInfo Browser { get; set; } = new();
    }
}
