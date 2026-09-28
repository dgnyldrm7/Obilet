using ObiletCase.Core.Result;

namespace ObiletCase.Core.Abstracts
{
    public interface ICookieService
    {
        ServiceResult<T> Get<T>(string key);
        ServiceResult<bool> Set<T>(string key, T value, int expireDays = 1);
        ServiceResult<bool> Remove(string key);
    }
}
