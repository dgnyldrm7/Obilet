using ObiletCase.Core.DTOs.BusJourney;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;

namespace ObiletCase.Core.Abstracts
{
    public interface IBusJourneyService
    {
        Task<ServiceResult<List<JourneyDto>>> GetJourneysAsync(
            DeviceSession session,
            int originId,
            int destinationId,
            DateTime departureDate,
            CancellationToken cancellationToken = default);
    }
}
