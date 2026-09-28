using Microsoft.AspNetCore.Mvc;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.Constants;
using ObiletCase.Core.DTOs.Session;

namespace ObiletCase.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ICookieService _cookieService;

        protected BaseController(ISessionService sessionService, ICookieService cookieService)
        {
            _sessionService = sessionService;
            _cookieService = cookieService;
        }

        protected async Task<DeviceSession?> GetOrCreateSessionAsync()
        {
            var cookieResult = _cookieService.Get<DeviceSession>(SessionCookieKey.Key);
            if (cookieResult.IsSuccess && cookieResult.Data != null)
                return cookieResult.Data;

            // HttpContext?.Connection?. şeklinde güvenli hale getiriyoruz:
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var port = HttpContext?.Connection?.RemotePort.ToString() ?? "80";

            var sessionResult = await _sessionService.CreateSessionAsync(ip, port);
            if (sessionResult.IsSuccess && sessionResult.Data != null)
            {
                _cookieService.Set(SessionCookieKey.Key, sessionResult.Data, expireDays: 1);
                return sessionResult.Data;
            }

            return null;
        }


        protected IActionResult RedirectToHomeWithError(string errorMessage)
        {
            TempData["ErrorMessage"] = errorMessage;
            return RedirectToAction("Index", "Home");
        }
    }
}