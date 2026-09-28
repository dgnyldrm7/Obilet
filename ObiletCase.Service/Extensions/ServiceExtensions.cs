using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ObiletCase.Core.Abstracts;
using ObiletCase.Core.Settings;
using ObiletCase.Services.Concrete;
using System.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;

namespace ObiletCase.Services.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServiceExtensionsDIContainer(this IServiceCollection services)
        {
            // RestApiService & HttpClient
            services.AddHttpClient<IRestApiService, RestApiService>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<ObiletApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", settings.ApiClientToken);
                client.Timeout = TimeSpan.FromSeconds(30); // Ağ kilitlenmelerine karşı genel istek zaman aşımı
            })
            // 1. Retry Politikası: 5xx sunucu hatalarında veya ağ kopmalarında kademeli bekleme (Jitter / Exponential Backoff)
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError() // 5xx hataları ve HttpRequestException (ağ kopmaları)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)) // 400ms, 800ms, 1600ms bekler
                ))
            // 2. Circuit Breaker Politikası: Obilet sunucusu tamamen çöktüyse arka arkaya gelen 5 hatadan sonra devreyi 30 saniye açar
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                ));

            // Concrete Servisler
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IBusLocationService, BusLocationService>();
            services.AddScoped<IBusJourneyService, BusJourneyService>();
            services.AddScoped<ICookieService, CookieService>();

            return services;
        }
    }
}