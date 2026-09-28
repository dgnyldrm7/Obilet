using System.Text.Json.Serialization;

namespace ObiletCase.Core.DTOs.Session
{
    public class ConnectionInfo
    {
        [JsonPropertyName("ip-address")]
        public string IpAddress { get; set; } = "165.114.41.21";

        [JsonPropertyName("port")]
        public string Port { get; set; } = "5117";
    }
}
