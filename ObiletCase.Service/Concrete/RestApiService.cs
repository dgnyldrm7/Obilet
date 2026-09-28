using Microsoft.Extensions.Logging;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.DTOs.Common;
using ObiletCase.Core.Result;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ObiletCase.Services.Concrete
{
    public class RestApiService : IRestApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RestApiService(HttpClient httpClient, ILogger<RestApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ServiceResult<TResponse>> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest payload,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
                var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Obilet API HTTP Hatası: {StatusCode} - {Body}", response.StatusCode, rawResponse);
                    return ServiceResult<TResponse>.Fail($"API Hatası: {response.StatusCode}", response.StatusCode);
                }

                var result = JsonSerializer.Deserialize<TResponse>(rawResponse, _jsonOptions);
                if (result == null)
                {
                    return ServiceResult<TResponse>.Fail("Yanıt okunamadı veya boş döndü.", HttpStatusCode.BadGateway);
                }

                // Eğer dönen tip BaseApiResponse ise içindeki status kontrol edilir
                if (result is BaseApiResponse<object> apiBase && apiBase.Status != "Success")
                {
                    var msg = apiBase.UserMessage ?? apiBase.Message ?? "Bilinmeyen API Hatası";
                    return ServiceResult<TResponse>.Fail(msg, HttpStatusCode.BadRequest);
                }

                return ServiceResult<TResponse>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RestApiService Exception: {Message}", ex.Message);
                return ServiceResult<TResponse>.Fail($"Sunucu hatası: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }
}