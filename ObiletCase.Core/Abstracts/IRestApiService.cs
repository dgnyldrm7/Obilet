using ObiletCase.Core.DTOs.BusJourney;
using ObiletCase.Core.DTOs.BusLocation;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;

namespace ObiletCase.Core.Abstracts
{
    public interface IRestApiService
    {
        Task<ServiceResult<TResponse>> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest payload,
            CancellationToken cancellationToken = default);
    }
}
