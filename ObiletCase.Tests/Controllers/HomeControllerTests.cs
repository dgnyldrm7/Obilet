using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.DTOs.BusLocation;
using ObiletCase.Core.DTOs.Session;
using ObiletCase.Core.Result;
using ObiletCase.Web.Controllers;
using ObiletCase.Web.Models;
using Xunit;

namespace ObiletCase.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ISessionService> _sessionServiceMock;
        private readonly Mock<ICookieService> _cookieServiceMock;
        private readonly Mock<IBusLocationService> _locationServiceMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _sessionServiceMock = new Mock<ISessionService>();
            _cookieServiceMock = new Mock<ICookieService>();
            _locationServiceMock = new Mock<IBusLocationService>();

            _controller = new HomeController(
                _sessionServiceMock.Object,
                _cookieServiceMock.Object,
                _locationServiceMock.Object);

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
        public async Task Index_WhenSessionFails_ShouldReturnViewWithErrorMessage()
        {
            // Arrange: Cookie yok ve yeni session da üretilemedi
            _cookieServiceMock
                .Setup(c => c.Get<DeviceSession>(It.IsAny<string>()))
                .Returns(ServiceResult<DeviceSession>.Fail("Not found"));

            _sessionServiceMock
                .Setup(s => s.CreateSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<DeviceSession>.Fail("Oturum açılamadı"));

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Oturum oluşturulamadı, lütfen sayfayı yenileyiniz.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Index_WhenLocationsFound_ShouldReturnViewWithModel()
        {
            // Arrange
            var session = new DeviceSession { SessionId = "s123", DeviceId = "d123" };
            var sampleLocations = new List<BusLocationDto>
            {
                new() { Id = 349, Name = "İstanbul Avrupa" },
                new() { Id = 356, Name = "Ankara" }
            };

            _cookieServiceMock
                .Setup(c => c.Get<DeviceSession>(It.IsAny<string>()))
                .Returns(ServiceResult<DeviceSession>.Success(session));

            _locationServiceMock
                .Setup(l => l.GetLocationsAsync(session, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<List<BusLocationDto>>.Success(sampleLocations));

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<HomeIndexViewModel>(result.Model);
            Assert.Equal(2, model.Locations.Count);
            Assert.Equal(349, model.DefaultOriginId);
            Assert.Equal(356, model.DefaultDestinationId);
        }
    }
}