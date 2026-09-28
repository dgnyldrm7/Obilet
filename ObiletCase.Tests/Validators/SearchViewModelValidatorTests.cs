using FluentValidation.TestHelper;
using ObiletCase.Services.Validations;
using ObiletCase.Web.Models;
using Xunit;

namespace ObiletCase.Tests.Validators
{
    public class SearchViewModelValidatorTests
    {
        private readonly SearchViewModelValidator _validator;

        public SearchViewModelValidatorTests()
        {
            _validator = new SearchViewModelValidator();
        }

        [Fact]
        public void Should_Have_Error_When_OriginId_And_DestinationId_Are_Same()
        {
            var model = new SearchViewModel
            {
                OriginId = 349,
                DestinationId = 349,
                DepartureDate = DateTime.Today.AddDays(1)
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DestinationId)
                  .WithErrorMessage("Kalkış ve varış noktaları aynı olamaz.");
        }

        [Fact]
        public void Should_Have_Error_When_DepartureDate_Is_In_The_Past()
        {
            var model = new SearchViewModel
            {
                OriginId = 349,
                DestinationId = 356,
                DepartureDate = DateTime.Today.AddDays(-1)
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DepartureDate)
                  .WithErrorMessage("Geçmiş bir tarih için sefer aranamaz.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Should_Have_Error_When_OriginId_Is_Zero_Or_Negative(int originId)
        {
            var model = new SearchViewModel
            {
                OriginId = originId,
                DestinationId = 356,
                DepartureDate = DateTime.Today.AddDays(1)
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.OriginId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new SearchViewModel
            {
                OriginId = 349,
                DestinationId = 356,
                DepartureDate = DateTime.Today.AddDays(1)
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}