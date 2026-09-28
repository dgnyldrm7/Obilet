using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.Common
{
    public class BaseApiResponse<T>
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("user-message")]
        public string? UserMessage { get; set; }

        [JsonPropertyName("api-request-id")]
        public string? ApiRequestId { get; set; }

        [JsonPropertyName("controller")]
        public string? Controller { get; set; }
    }
}
