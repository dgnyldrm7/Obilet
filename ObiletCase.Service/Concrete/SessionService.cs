using Microsoft.Extensions.Logging;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.Constants;
using ObiletCase.Core.DTOs.Common;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;
using ObiletCase.Core.Settings;
using System.Net;

namespace ObiletCase.Services.Concrete
{
    public class SessionService : ISessionService
    {
        private readonly IRestApiService _restApi;
        private readonly ILogger<SessionService> _logger;

        public SessionService(
            IRestApiService restApi,
            ILogger<SessionService> logger)
        {
            _restApi = restApi;
            _logger = logger;
        }

        public async Task<ServiceResult<DeviceSession>> CreateSessionAsync(
            string ipAddress,
            string port,
            CancellationToken cancellationToken = default)
        {
            var isLocalhost = string.IsNullOrEmpty(ipAddress) || ipAddress == "::1" || ipAddress == "127.0.0.1";
            var validIp = isLocalhost ? "165.114.41.21" : ipAddress;
            var validPort = string.IsNullOrEmpty(port) || port == "0" ? "5117" : port;

            if (isLocalhost)
            {
                _logger.LogDebug(
                    "Yerel IP tespit edildi, Obilet API uyumluluğu için fallback IP kullanılıyor. OriginalIp: {OriginalIp}, FallbackIp: {FallbackIp}",
                    ipAddress, validIp);
            }

            _logger.LogInformation(
                "Obilet cihaz oturumu (Session) oluşturma isteği başlatılıyor. ClientIp: {ClientIp}, ClientPort: {ClientPort}",
                validIp, validPort);

            var request = new GetSessionRequest
            {
                Type = 1,
                Connection = new ConnectionInfo { IpAddress = validIp, Port = validPort },
                Browser = new BrowserInfo { Name = "Chrome", Version = "47.0.0.12" }
            };

            var apiResult = await _restApi.PostAsync<GetSessionRequest, BaseApiResponse<SessionResponseData>>(
                ObiletApiEndpoints.Client.GetSession,
                request,
                cancellationToken);

            if (!apiResult.IsSuccess || apiResult.Data?.Data == null)
            {
                var statusCode = (HttpStatusCode)(apiResult.ProblemDetails?.Status ?? 500);
                var errorDetail = apiResult.ProblemDetails?.Detail ?? "Oturum oluşturulamadı.";

                _logger.LogError(
                    "Obilet oturum oluşturma başarısız oldu. StatusCode: {StatusCode}, Error: {ErrorDetail}",
                    statusCode, errorDetail);

                return ServiceResult<DeviceSession>.Fail(errorDetail, statusCode);
            }

            var responseData = apiResult.Data.Data;

            _logger.LogInformation(
                "Obilet oturumu başarıyla oluşturuldu. SessionId: {SessionId}, DeviceId: {DeviceId}",
                responseData.SessionId, responseData.DeviceId);

            var sessionDto = new DeviceSession
            {
                SessionId = responseData.SessionId,
                DeviceId = responseData.DeviceId
            };

            return ServiceResult<DeviceSession>.Success(sessionDto);
        }
    }
}