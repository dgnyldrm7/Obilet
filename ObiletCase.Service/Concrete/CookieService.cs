using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.Result;
using System.Net;
using System.Text.Json;

namespace ObiletCase.Services.Concrete
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CookieService> _logger;

        public CookieService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CookieService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private HttpContext Context => _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext bulunamadı.");

        public ServiceResult<T> Get<T>(string key)
        {
            if (Context.Request.Cookies.TryGetValue(key, out var json) && !string.IsNullOrEmpty(json))
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(json);
                    if (data != null)
                    {
                        _logger.LogDebug("Çerez başarıyla okundu ve serialize edildi. CookieKey: {CookieKey}", key);
                        return ServiceResult<T>.Success(data);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Çerez JSON formatı bozuk veya çözülemedi. CookieKey: {CookieKey}", key);
                    return ServiceResult<T>.Fail("Çerez verisi çözülemedi.", HttpStatusCode.UnprocessableEntity);
                }
            }

            _logger.LogDebug("İstenen çerez bulunamadı. CookieKey: {CookieKey}", key);
            return ServiceResult<T>.Fail("Çerez bulunamadı.", HttpStatusCode.NotFound);
        }

        public ServiceResult<bool> Set<T>(string key, T value, int expireDays = 1)
        {
            try
            {
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(expireDays)
                };

                var json = JsonSerializer.Serialize(value);
                Context.Response.Cookies.Append(key, json, options);

                _logger.LogInformation(
                    "Yeni çerez yazıldı. CookieKey: {CookieKey}, ExpireDays: {ExpireDays}",
                    key, expireDays);

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çerez yazılırken beklenmeyen bir hata oluştu. CookieKey: {CookieKey}", key);
                return ServiceResult<bool>.Fail($"Çerez yazılamadı: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public ServiceResult<bool> Remove(string key)
        {
            try
            {
                Context.Response.Cookies.Delete(key);
                _logger.LogInformation("Çerez silindi. CookieKey: {CookieKey}", key);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çerez silinirken hata oluştu. CookieKey: {CookieKey}", key);
                return ServiceResult<bool>.Fail($"Çerez silinemedi: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }
}