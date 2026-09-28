using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.DTOs.BusJourney;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;
using ObiletCase.Web.Controllers;
using ObiletCase.Web.Models;

namespace ObiletCase.Tests.Controllers
{
    public class JourneyControllerTests
    {
        private readonly Mock<ISessionService> _sessionServiceMock;
        private readonly Mock<ICookieService> _cookieServiceMock;
        private readonly Mock<IBusJourneyService> _journeyServiceMock;
        private readonly Mock<IValidator<SearchViewModel>> _validatorMock;
        private readonly JourneyController _controller;

        public JourneyControllerTests()
        {
            _sessionServiceMock = new Mock<ISessionService>();
            _cookieServiceMock = new Mock<ICookieService>();
            _journeyServiceMock = new Mock<IBusJourneyService>();
            _validatorMock = new Mock<IValidator<SearchViewModel>>();

            _controller = new JourneyController(
                _sessionServiceMock.Object,
                _cookieServiceMock.Object,
                _journeyServiceMock.Object,
                _validatorMock.Object);

            var httpContext = new DefaultHttpContext();

            // ControllerContext ataması:
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var tempDataProvider = new Mock<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
        }

        [Fact]
        public async Task Index_WhenValidationFails_ShouldRedirectToHomeWithErrorMessage()
        {
            // Arrange
            var invalidModel = new SearchViewModel();
            var validationFailure = new ValidationResult(new[]
            {
                new ValidationFailure("OriginId", "Kalkış noktası seçiniz.")
            });

            _validatorMock
                .Setup(v => v.ValidateAsync(invalidModel, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationFailure);

            // Act
            var result = await _controller.Index(invalidModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Kalkış noktası seçiniz.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Index_WhenSessionFails_ShouldRedirectToHome()
        {
            // Arrange
            var validModel = new SearchViewModel { OriginId = 349, DestinationId = 356, DepartureDate = DateTime.Today.AddDays(1) };

            _validatorMock
                .Setup(v => v.ValidateAsync(validModel, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _cookieServiceMock
                .Setup(c => c.Get<DeviceSession>(It.IsAny<string>()))
                .Returns(ServiceResult<DeviceSession>.Fail("Not found"));

            _sessionServiceMock
                .Setup(s => s.CreateSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<DeviceSession>.Fail("Session error"));

            // Act
            var result = await _controller.Index(validModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Oturum sonlandı, lütfen yeniden arama yapınız.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Index_WhenValid_ShouldReturnViewWithJourneys()
        {
            // Arrange
            var validModel = new SearchViewModel
            {
                OriginId = 349,
                DestinationId = 356,
                DepartureDate = DateTime.Today.AddDays(1)
            };

            var session = new DeviceSession { SessionId = "s123", DeviceId = "d123" };
            var sampleJourneys = new List<JourneyDto>
            {
                new()
                {
                    Id = 1,
                    PartnerName = "Pamukkale Turizm"
                }
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(validModel, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _cookieServiceMock
                .Setup(c => c.Get<DeviceSession>(It.IsAny<string>()))
                .Returns(ServiceResult<DeviceSession>.Success(session));

            _journeyServiceMock
                .Setup(j => j.GetJourneysAsync(
                    session,
                    validModel.OriginId,
                    validModel.DestinationId,
                    validModel.DepartureDate,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<List<JourneyDto>>.Success(sampleJourneys));

            // Act
            var result = await _controller.Index(validModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<JourneyListViewModel>(result.Model);
            Assert.Single(model.Journeys);
            Assert.Equal(349, model.OriginId);
            Assert.Equal(356, model.DestinationId);
        }
    }
}