using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ObiletCase.Core.Abstracts;
using ObiletCase.Web.Models;

namespace ObiletCase.Web.Controllers
{
    public class JourneyController : BaseController
    {
        private readonly IBusJourneyService _journeyService;
        private readonly IValidator<SearchViewModel> _validator;

        public JourneyController(
            ISessionService sessionService,
            ICookieService cookieService,
            IBusJourneyService journeyService,
            IValidator<SearchViewModel> validator)
            : base(sessionService, cookieService)
        {
            _journeyService = journeyService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                return RedirectToHomeWithError(validationResult.Errors[0].ErrorMessage);

            var session = await GetOrCreateSessionAsync();
            if (session == null)
                return RedirectToHomeWithError("Oturum sonlandı, lütfen yeniden arama yapınız.");

            var journeyResult = await _journeyService.GetJourneysAsync(
                session,
                model.OriginId,
                model.DestinationId,
                model.DepartureDate);

            if (!journeyResult.IsSuccess)
                return RedirectToHomeWithError(journeyResult.ProblemDetails?.Detail ?? "Seferler alınırken bir sorun oluştu.");

            return View(new JourneyListViewModel(model, journeyResult.Data));
        }
    }
}