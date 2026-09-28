using Microsoft.Extensions.Logging;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.Constants;
using ObiletCase.Core.DTOs.BusJourney;
using ObiletCase.Core.DTOs.Common;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;
using System.Net;

namespace ObiletCase.Services.Concrete
{
    public class BusJourneyService : IBusJourneyService
    {
        private readonly IRestApiService _restApi;
        private readonly ILogger<BusJourneyService> _logger;

        public BusJourneyService(
            IRestApiService restApi,
            ILogger<BusJourneyService> logger)
        {
            _restApi = restApi;
            _logger = logger;
        }

        public async Task<ServiceResult<List<JourneyDto>>> GetJourneysAsync(
            DeviceSession session,
            int originId,
            int destinationId,
            DateTime departureDate,
            CancellationToken cancellationToken = default)
        {
            var formattedDate = departureDate.ToString("yyyy-MM-dd");

            _logger.LogInformation(
                "Obilet sefer araması başlatılıyor. OriginId: {OriginId}, DestinationId: {DestinationId}, Date: {DepartureDate}, SessionId: {SessionId}",
                originId, destinationId, formattedDate, session.SessionId);

            var request = new GetJourneysRequest
            {
                DeviceSession = session,
                Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Language = "tr-TR",
                Data = new JourneyRequestData
                {
                    OriginId = originId,
                    DestinationId = destinationId,
                    DepartureDate = formattedDate
                }
            };

            var apiResult = await _restApi.PostAsync<GetJourneysRequest, BaseApiResponse<List<JourneyDto>>>(
                ObiletApiEndpoints.Journey.GetBusJourneys,
                request,
                cancellationToken);

            if (!apiResult.IsSuccess || apiResult.Data?.Data == null)
            {
                var statusCode = (HttpStatusCode)(apiResult.ProblemDetails?.Status ?? 500);
                var errorDetail = apiResult.ProblemDetails?.Detail ?? "Sefer bilgileri alınamadı.";

                _logger.LogWarning(
                    "Obilet sefer araması başarısız oldu. OriginId: {OriginId}, DestinationId: {DestinationId}, StatusCode: {StatusCode}, Error: {ErrorDetail}",
                    originId, destinationId, statusCode, errorDetail);

                return ServiceResult<List<JourneyDto>>.Fail(errorDetail, statusCode);
            }

            var journeys = apiResult.Data.Data;

            // Saat sırasına göre sıralama
            var sortedJourneys = journeys
                .OrderBy(x => x.Journey.Departure)
                .ToList();

            _logger.LogInformation(
                "Obilet sefer araması başarıyla tamamlandı. OriginId: {OriginId}, DestinationId: {DestinationId}, BulunanSeferSayisi: {Count}",
                originId, destinationId, sortedJourneys.Count);

            return ServiceResult<List<JourneyDto>>.Success(sortedJourneys);
        }
    }
}