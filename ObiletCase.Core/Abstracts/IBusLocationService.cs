using ObiletCase.Core.DTOs.BusLocation;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;

namespace ObiletCase.Core.Abstracts
{
    public interface IBusLocationService
    {
        Task<ServiceResult<List<BusLocationDto>>> GetLocationsAsync(
            DeviceSession session,
            string? search = null,
            CancellationToken cancellationToken = default);
    }
}
