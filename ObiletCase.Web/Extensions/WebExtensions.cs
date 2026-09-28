using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObiletCase.Core.Settings;
using ObiletCase.Services.Validations;
using ObiletCase.Web.Middlewares;

namespace ObiletCase.Web.Extensions
{
    public static class WebExtensions
    {
        public static IServiceCollection AddWebExtensionsDIContainer(this IServiceCollection services, IConfiguration configuration)
        {
            // AppSettings
            services.Configure<ObiletApiSettings>(configuration.GetSection("ObiletApiSettings"));

            // Exception Handling
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // FluentValidation - Web katmanındaki validator'ları kaydeder
            services.AddValidatorsFromAssemblyContaining<SearchViewModelValidator>();

            return services;
        }
    }
}