using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.Session
{
    public class BrowserInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Chrome";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "47.0.0.12";
    }
}
