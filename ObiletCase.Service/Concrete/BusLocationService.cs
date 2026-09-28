using Microsoft.Extensions.Logging;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.Constants;
using ObiletCase.Core.DTOs.BusLocation;
using ObiletCase.Core.DTOs.Common;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;
using System.Net;

namespace ObiletCase.Services.Concrete
{
    public class BusLocationService : IBusLocationService
    {
        private readonly IRestApiService _restApi;
        private readonly ILogger<BusLocationService> _logger;

        public BusLocationService(
            IRestApiService restApi,
            ILogger<BusLocationService> logger)
        {
            _restApi = restApi;
            _logger = logger;
        }

        public async Task<ServiceResult<List<BusLocationDto>>> GetLocationsAsync(
            DeviceSession session,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Obilet durak listesi isteği başlatılıyor. AramaKriteri: {SearchTerm}, SessionId: {SessionId}",
                search ?? "[Tümü]",
                session.SessionId);

            var request = new GetBusLocationsRequest
            {
                Data = search,
                DeviceSession = session,
                Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Language = "tr-TR"
            };

            var apiResult = await _restApi.PostAsync<GetBusLocationsRequest, BaseApiResponse<List<BusLocationDto>>>(
                ObiletApiEndpoints.Location.GetBusLocations,
                request,
                cancellationToken);

            if (!apiResult.IsSuccess || apiResult.Data?.Data == null)
            {
                var statusCode = (HttpStatusCode)(apiResult.ProblemDetails?.Status ?? 500);
                var errorDetail = apiResult.ProblemDetails?.Detail ?? "Duraklar alınamadı.";

                _logger.LogWarning(
                    "Obilet durak listesi alınamadı. AramaKriteri: {SearchTerm}, StatusCode: {StatusCode}, Error: {ErrorDetail}",
                    search ?? "[Tümü]",
                    statusCode,
                    errorDetail);

                return ServiceResult<List<BusLocationDto>>.Fail(errorDetail, statusCode);
            }

            var locations = apiResult.Data.Data;

            _logger.LogInformation(
                "Obilet durak listesi başarıyla alındı. AramaKriteri: {SearchTerm}, AlinanDurakSayisi: {Count}",
                search ?? "[Tümü]",
                locations.Count);

            return ServiceResult<List<BusLocationDto>>.Success(locations);
        }
    }
}