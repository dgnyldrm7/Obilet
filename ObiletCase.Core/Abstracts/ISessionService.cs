using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;

namespace ObiletCase.Core.Abstracts
{
    public interface ISessionService
    {
        Task<ServiceResult<DeviceSession>> CreateSessionAsync(
            string ipAddress,
            string port,
            CancellationToken cancellationToken = default);
    }
}
