using FluentValidation;
using ObiletCase.Web.Models;

namespace ObiletCase.Services.Validations
{
    public class SearchViewModelValidator : AbstractValidator<SearchViewModel>
    {
        
        public SearchViewModelValidator()
        {
            RuleFor(x => x.OriginId)
                .GreaterThan(0).WithMessage("Lütfen geçerli bir kalkış noktası seçiniz.");

            RuleFor(x => x.DestinationId)
                .GreaterThan(0).WithMessage("Lütfen geçerli bir varış noktası seçiniz.")
                .NotEqual(x => x.OriginId).WithMessage("Kalkış ve varış noktaları aynı olamaz.");

            RuleFor(x => x.DepartureDate)
                .Must(date => date.Date >= DateTime.Today)
                .WithMessage("Geçmiş bir tarih için sefer aranamaz.");
        }
        
    }
}