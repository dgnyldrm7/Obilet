using Microsoft.AspNetCore.Mvc;
using ObiletCase.Core.Abstracts;
using ObiletCase.Web.Models;

namespace ObiletCase.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IBusLocationService _locationService;

        public HomeController(
            ISessionService sessionService,
            ICookieService cookieService,
            IBusLocationService locationService)
            : base(sessionService, cookieService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var session = await GetOrCreateSessionAsync();
            if (session == null)
            {
                TempData["ErrorMessage"] = "Oturum oluşturulamadı, lütfen sayfayı yenileyiniz.";
                return View(new HomeIndexViewModel());
            }

            var locationsResult = await _locationService.GetLocationsAsync(session);

            return View(new HomeIndexViewModel(locationsResult.Data));
        }

    }
}